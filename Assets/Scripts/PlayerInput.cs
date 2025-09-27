using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private PlayerController m_playerController;
    
    private PlayerInput_Actions m_inputActions;
    
    private void OnEnable()
    {
        m_inputActions = new PlayerInput_Actions();
        m_inputActions.Enable();

        m_inputActions.Player.Move.performed += OnMoveInput;
        m_inputActions.Player.Move.canceled += OnMoveInput;
        m_inputActions.Player.ToggleFlashlight.performed += OnToggleFlashlightInput;
    }

    private void OnDisable()
    {
        m_inputActions.Player.Move.performed -= OnMoveInput;
        m_inputActions.Player.Move.canceled -= OnMoveInput;
        m_inputActions.Player.ToggleFlashlight.performed -= OnToggleFlashlightInput;
        
        m_inputActions.Disable();
    }
    
    private void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        
        m_playerController.OnMoveInput(moveInput);
    }
    
    private void OnToggleFlashlightInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Flashlight toggled");
        }
    }
}
