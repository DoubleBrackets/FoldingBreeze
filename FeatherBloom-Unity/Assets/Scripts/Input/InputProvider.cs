using System;
using UnityEngine;

namespace Input
{
    public abstract class InputProvider : MonoBehaviour
    {
        public Action<GameplayInputService.FanState> DesiredFanStateChanged;
        public Action<GameplayInputService.AimInput> AimInputChanged;
        public Action ToggleFanState;
        public Action UpdraftInput;
        public Action GustInput;
        public Action SliceInput;
        public Action FanSelfInput;
    }
}