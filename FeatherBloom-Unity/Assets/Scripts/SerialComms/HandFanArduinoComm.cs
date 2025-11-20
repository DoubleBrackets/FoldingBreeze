using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace SerialComms
{
    /// <summary>
    ///     Handles serial IO and parsing from hand-fan controller arduino
    /// </summary>
    public class HandFanArduinoComm : MonoBehaviour, IArduinoCom
    {
        public struct SerialReadResult
        {
            public bool OpenFanSwitch;
            public bool CloseFanSwitch;
            public Quaternion Orientation;
        }

        /// <summary>
        ///     1 byte for buttons, 16 bytes for quaternion
        /// </summary>
        private const int PacketByteCount = 17;

        public static HandFanArduinoComm Instance;

        [Header("Debug")]

        [SerializeField]
        private string _portName;

        [SerializeField]
        private bool _logPackets;

        [SerializeField]
        private int _baudRate;

        [Header("Rate Counting")]

        [SerializeField]
        private int _rateCountWindow;

        [Header("Events (Out)")]

        [SerializeField]
        private UnityEvent<SerialReadResult> OnSerialReadResult;

        public UnityEvent<string> OnStatusChange;

        private readonly byte[] _packetBuffer = new byte[PacketByteCount];

        private readonly Queue<float> _intervalQueue = new();

        private SerialPort _serialPort;
        private float _intervalSum;
        private float _prevPacketTime;
        private float _currentPacketRate;

        private int _toDiscard;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            try
            {
                CleanUp();
            }
            catch (Exception e)
            {
                ShowException(e);
            }
        }

        private void OnGUI()
        {
            DrawDebugGUI();
        }

        public UnityEvent<string> OnStatusChangeEvent => OnStatusChange;

        public void SetSerialPort(string port)
        {
            _portName = port;
        }

        /// <summary>
        ///     Try to connect the serial port and begin serial read loop
        /// </summary>
        public void Connect()
        {
            try
            {
                CleanUp();
            }
            catch (Exception e)
            {
                ShowException(e);
            }

            try
            {
                _toDiscard = 10;

                InitializeSerialPort(_portName);

                _serialPort.WriteTimeout = 1000;
                ReadLoop(this.GetCancellationTokenOnDestroy()).Forget();
            }
            catch (Exception e)
            {
                ShowException(e);
            }
        }

        private void DrawDebugGUI()
        {
            GUILayout.Label("Connected to arduino: " + (_serialPort == null ? "No" : _serialPort.IsOpen));
            GUILayout.Label("Serial Packet Rate: " + _currentPacketRate);
        }

        private void InitializeSerialPort(string arduinoPort)
        {
            Debug.Log($"Connecting to {arduinoPort}");

            _serialPort = new SerialPort(arduinoPort, _baudRate);

            // Disable Rts since we don't use handshaking
            // Doesn't work on Mac unless we do this
            _serialPort.RtsEnable = true;

            // We don't need to enable Dtr to get it to work on mac, but leaving it here in case
            // _serialPort.DtrEnable = true;

            _serialPort.Open();
            _serialPort.ErrorReceived += HandleErrorReceived;

            Debug.Log($"Connected to {arduinoPort}");

            OnStatusChange?.Invoke($"Connected to {arduinoPort}");
        }

        private void CleanUp()
        {
            if (_serialPort == null)
            {
                return;
            }

            _serialPort.ErrorReceived -= HandleErrorReceived;
            _serialPort.Close();
            _serialPort.Dispose();
        }

        private async UniTaskVoid ReadLoop(CancellationToken cancellationToken)
        {
            try
            {
                Debug.Log($"Bytes to read: {_serialPort.BytesToRead}");
                // Discard any junk in the buffer
                _serialPort.DiscardInBuffer();

                // Starting ack
                Write(new byte[] { 255 });

                while (true)
                {
                    await UniTask.Yield();
                    cancellationToken.ThrowIfCancellationRequested();
                    ProcessAllFromPort();
                }
            }
            catch (Exception e)
            {
                ShowException(e);
            }
            finally
            {
                CleanUp();
            }
        }

        private void ProcessAllFromPort()
        {
            int bytesToRead = _serialPort.BytesToRead;
            int packetsToRead = bytesToRead / PacketByteCount;

            // Don't read partial packets
            if (packetsToRead == 0)
            {
                return;
            }

            // Read into buffer
            var byteIndex = 0;
            while (byteIndex < PacketByteCount)
            {
                var readByte = (byte)_serialPort.ReadByte();
                _packetBuffer[byteIndex] = readByte;
                byteIndex++;
            }

            // Calculate the packet rate
            SamplePacketInterval();

            SerialReadResult serialReadResult = ParsePacket(_packetBuffer);

            if (_toDiscard > 0)
            {
                _toDiscard--;
            }
            else
            {
                try
                {
                    OnSerialReadResult?.Invoke(serialReadResult);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }

            if (_logPackets)
            {
                var packet = string.Empty;
                foreach (byte b in _packetBuffer)
                {
                    // log byte as binary
                    packet += Convert.ToString(b, 2).PadLeft(8, '0') + " ";
                }

                Debug.Log(packetsToRead);
                Debug.Log(JsonUtility.ToJson(serialReadResult));
                Debug.Log(packet);
            }

            // Only read one packet per frame (because of handshake there should only be one)
            _serialPort.DiscardInBuffer();

            // Write ack to arduino for next packet
            Write(new byte[] { 255 });
        }

        private SerialReadResult ParsePacket(byte[] packetBuffer)
        {
            var quatOffset = 1;
            var i = BitConverter.ToSingle(packetBuffer, quatOffset);
            var j = BitConverter.ToSingle(packetBuffer, quatOffset + 4);
            var k = BitConverter.ToSingle(packetBuffer, quatOffset + 8);
            var w = BitConverter.ToSingle(packetBuffer, quatOffset + 12);

            var orientation = new Quaternion(i, j, k, w);

            var serialReadResult = new SerialReadResult
            {
                OpenFanSwitch = (packetBuffer[0] & 1) == 1,
                CloseFanSwitch = (packetBuffer[0] & 2) == 2,
                Orientation = orientation
            };

            return serialReadResult;
        }

        private void ShowException(Exception e)
        {
            Debug.Log(e.Message);
            OnStatusChange?.Invoke($"{e}");
        }

        private void HandleErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Debug.Log(e.ToString());
            OnStatusChange?.Invoke($"{e.EventType.ToString()} : {e}");
        }

        private void Write(byte[] data)
        {
            try
            {
                _serialPort.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
        }

        private void SamplePacketInterval()
        {
            float interval = Time.time - _prevPacketTime;

            _intervalSum += interval;
            _intervalQueue.Enqueue(interval);

            if (_intervalQueue.Count > _rateCountWindow)
            {
                _intervalSum -= _intervalQueue.Dequeue();
            }

            _currentPacketRate = _rateCountWindow / _intervalSum;

            _prevPacketTime = Time.time;
        }
    }
}