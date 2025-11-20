using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class ConventionalInputProvider : InputProvider
    {
        public void HandleAiming(InputAction.CallbackContext context)
        {
            AimInputChanged?.Invoke(new GameplayInputService.AimInput
            {
                FinalAimInput = context.ReadValue<Vector2>()
            });
        }

        public void HandleToggleFanState(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                ToggleFanState?.Invoke();
            }
        }

        public void HandleGustInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                GustInput?.Invoke();
            }
        }

        public void HandleUpdraftInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                UpdraftInput?.Invoke();
            }
        }

        public void HandleSliceInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                SliceInput?.Invoke();
            }
        }

        public void HandleSelfFan(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                FanSelfInput?.Invoke();
            }
        }

        public void HandleMouse(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var mousePosition = context.ReadValue<Vector2>();
                float normalizedY = (Screen.height - mousePosition.y) / Screen.height;
                float normalizedX = (Screen.width - mousePosition.x) / Screen.width;

                AimInputChanged?.Invoke(new GameplayInputService.AimInput
                {
                    FinalAimInput = new Vector2(normalizedX, normalizedY)
                });
            }
        }
    }
}