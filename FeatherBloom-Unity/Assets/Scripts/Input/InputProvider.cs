using System;
using Input.DataTypes;
using UnityEngine;

namespace Input
{
    public abstract class InputProvider : MonoBehaviour
    {
        public Action<FanState> DesiredFanStateChanged;
        public Action<AimInput> AimInputChanged;
        public Action ToggleFanState;
        public Action UpdraftInput;
        public Action GustInput;
        public Action SliceInput;
        public Action FanSelfInput;
        public abstract void SetDefaultToCurrent();
    }
}