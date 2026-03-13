using System;
using UnityEngine;

namespace Saving
{
    /// <summary>
    ///     Simple serializable class to hold all game persistent data
    /// </summary>
    [Serializable]
    public class SaveModel
    {
        public string LastHandFanConnectedSerialPortName;
        public string LastBoxFanConnectedSerialPortName;
        public int HighScore;
        public Quaternion DefaultZeroedOrientation = Quaternion.identity;
    }
}