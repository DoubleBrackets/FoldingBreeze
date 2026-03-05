namespace Input.SerialComms
{
    public interface IArduinoCom
    {
        public void SetSerialPort(string portName);
        public void Connect();
    }
}