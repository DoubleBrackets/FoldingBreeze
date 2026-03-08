using System;
using System.IO.Ports;
using System.Threading;
using Cysharp.Threading.Tasks;
using DevTools;
using UnityEngine;
using ValueSO.Core;

namespace Input.SerialComms
{
    /// <summary>
    ///     Handles serial IO and parsing from hand-fan controller arduino
    /// </summary>
    public class BoxFanArduinoComm : MonoBehaviour, IArduinoCom
    {
        [Header("ValueSO (Write)")]

        [SerializeField]
        private StringValueSO _status;

        [Header("Debug")]

        [SerializeField]
        private string _portName;

        [SerializeField]
        private bool _logPackets;

        [SerializeField]
        private int _baudRate;

        private SerialPort _serialPort;

        private void Awake()
        {
            _status.SetValue("Not Connected");
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
            OnGUIHook.SetElement("Connected to Box Arduino", _status.Value);
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

            _status.SetValue($"Connected to {arduinoPort}");

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
            _status.SetValue($"{e}");
        }

        private void HandleErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Debug.Log(e.ToString());
            _status.SetValue($"{e.EventType.ToString()} : {e}");
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