using UnityEngine.Events;

namespace SerialComms
{
    public interface IArduinoCom
    {
        UnityEvent<string> OnStatusChangeEvent { get; }
        public void SetSerialPort(string portName);
        public void Connect();
    }
}