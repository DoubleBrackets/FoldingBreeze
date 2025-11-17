using System;
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
    public class BoxFanArduinoComm : MonoBehaviour, IArduinoCom
    {
        public static BoxFanArduinoComm Instance;

        [Header("Debug")]

        [SerializeField]
        private string _portName;

        [SerializeField]
        private bool _logPackets;

        [SerializeField]
        private int _baudRate;

        public UnityEvent<string> OnStatusChange;

        private SerialPort _serialPort;

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

            _serialPort.DiscardInBuffer();
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
            while (_serialPort.BytesToRead > 0)
            {
                int read = _serialPort.ReadByte();
                if (_logPackets)
                {
                    Debug.Log(read);
                }
            }
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

        [ContextMenu("Test")]
        public void TestWrite()
        {
            WriteFanOn(true);
        }

        public void WriteFanOn(bool fanOn)
        {
            if (_serialPort == null || !_serialPort.IsOpen)
            {
                return;
            }

            Write(fanOn ? new byte[] { 255 } : new byte[] { 0 });
        }

        public void Write(byte[] data)
        {
            try
            {
                Debug.Log($"Wrote {data.Length} bytes from ${_serialPort.PortName}");
                _serialPort.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
        }
    }
}