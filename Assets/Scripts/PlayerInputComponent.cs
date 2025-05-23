using UnityEngine;
using UnityEngine.InputSystem;


    public class PlayerInputComponent : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }
        public Vector2 LastMoveInput { get; private set; }
        public Vector2 CameraInput { get; private set; }
        public bool JumpInput { get; private set; }
        public bool HasMoveInput { get; private set; }

        public void OnMoveEvent(InputAction.CallbackContext context)
        {
            if (GameManager.Instance.MatchState != MatchStateEnum.Active) return;
            
            Vector2 moveInput = context.ReadValue<Vector2>();

            bool hasMoveInput = moveInput.sqrMagnitude > 0.0f;
            if (HasMoveInput && !hasMoveInput)
            {
                LastMoveInput = MoveInput;
            }

            MoveInput = moveInput;
            HasMoveInput = hasMoveInput;
        }

        public void OnLookEvent(InputAction.CallbackContext context)
        {
            if (GameManager.Instance.MatchState != MatchStateEnum.Active) return;

            CameraInput = context.ReadValue<Vector2>();
        }

        public void OnJumpEvent(InputAction.CallbackContext context)
        {
            if (GameManager.Instance.MatchState != MatchStateEnum.Active) return;

            if (context.started || context.performed)
            {
                JumpInput = true;
            }
            else if (context.canceled)
            {
                JumpInput = false;
            }
        }
    }

