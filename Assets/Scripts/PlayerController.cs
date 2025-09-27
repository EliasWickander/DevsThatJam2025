using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] 
    private CharacterController m_characterController;
    
    [SerializeField] 
    private float m_moveSpeed = 8;
    
    private Vector3 m_currentMoveDirection = Vector3.zero;
    private Vector3 m_currentVelocity = Vector3.zero;
    public Vector3 CurrentVelocity => m_currentVelocity;

    private void OnValidate()
    {
        m_characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleTurning();
        HandleMovement();
    }

    private void HandleMovement()
    {
        m_currentVelocity.x = m_currentMoveDirection.x * m_moveSpeed;
        m_currentVelocity.z = m_currentMoveDirection.z * m_moveSpeed;
        
        if(m_characterController.isGrounded && m_currentVelocity.y < 0)
            m_currentVelocity.y = -1.0f; // Small value to ensure we stay grounded
        
        m_characterController.Move(m_currentVelocity * Time.deltaTime);
    }

    private void HandleTurning()
    {
        Vector3 cameraEulerAngles = CameraManager.Instance.CurrentCamera.transform.eulerAngles;
        float cameraYaw = cameraEulerAngles.y;

        transform.rotation = Quaternion.Euler(0f, cameraYaw, 0f);
    }
    
    public void OnMoveInput(Vector2 input)
    {
        Vector3 moveDirection = new Vector3(input.x, 0, input.y);
        
        m_currentMoveDirection = transform.TransformDirection(moveDirection);
    }
}
