using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace DevTools
{
    public class DevHotkeys : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent _onReset;

        [SerializeField]
        private InputAction _resetAction;

        [SerializeField]
        private InputAction _toggleFullscreen;

        private bool _fullscreen = true;

        private void Awake()
        {
            _resetAction.Enable();
            _resetAction.performed += _ => _onReset?.Invoke();

            _toggleFullscreen.Enable();
            _toggleFullscreen.performed += HandleToggleFullscreen;
        }

        private void HandleToggleFullscreen(InputAction.CallbackContext context)
        {
            _fullscreen = !_fullscreen;

            if (_fullscreen)
            {
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            }

            else
            {
                Screen.fullScreenMode = FullScreenMode.Windowed;
            }
        }
    }
}