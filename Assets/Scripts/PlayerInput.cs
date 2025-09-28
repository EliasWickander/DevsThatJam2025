using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerInput_Actions m_inputActions;
    
    public event Action<Vector2> OnMoveInputEvent;
    public event Action OnInteractInputEvent;
    public event Action OnToggleFlashlightInputEvent;
    
    private void OnEnable()
    {
        m_inputActions = new PlayerInput_Actions();
        m_inputActions.Enable();

        m_inputActions.Player.Move.performed += OnMoveInput;
        m_inputActions.Player.Move.canceled += OnMoveInputCancelled;
        m_inputActions.Player.Interact.performed += OnInteractInput;
        m_inputActions.Player.ToggleFlashlight.performed += OnToggleFlashlightInput;
    }

    private void OnDisable()
    {
        m_inputActions.Player.Move.performed -= OnMoveInput;
        m_inputActions.Player.Move.canceled -= OnMoveInputCancelled;
        m_inputActions.Player.Interact.performed -= OnInteractInput;
        m_inputActions.Player.ToggleFlashlight.performed -= OnToggleFlashlightInput;
        
        m_inputActions.Disable();
    }
    
    private void OnMoveInput(InputAction.CallbackContext context)
    {
        OnMoveInputEvent?.Invoke(context.ReadValue<Vector2>());
    }
    
    private void OnMoveInputCancelled(InputAction.CallbackContext context)
    {
        OnMoveInputEvent?.Invoke(Vector2.zero);
    }
    
    private void OnInteractInput(InputAction.CallbackContext context)
    {
        OnInteractInputEvent?.Invoke();
    }
    
    private void OnToggleFlashlightInput(InputAction.CallbackContext context)
    {
        OnToggleFlashlightInputEvent?.Invoke();
    }
}
