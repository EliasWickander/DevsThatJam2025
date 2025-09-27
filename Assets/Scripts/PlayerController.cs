using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField] 
    private CharacterController m_characterController;

    [SerializeField]
    private Transform m_headTransform;
    public Transform HeadTransform => m_headTransform;
    
    [SerializeField]
    private Transform m_handTransform;
    
    [SerializeField]
    private Vector3 m_handPositionOffset = new Vector3(0.3f, -0.3f, 0.5f);
    
    public Vector3 CenterPosition => m_characterController.bounds.center;
    
    [SerializeField]
    private Flashlight m_flashLight;
    
    [SerializeField] 
    private float m_moveSpeed = 8;
    
    private Vector3 m_currentMoveDirection = Vector3.zero;
    private Vector3 m_currentVelocity = Vector3.zero;
    public Vector3 CurrentVelocity => m_currentVelocity;

    private void OnValidate()
    {
        m_characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        GameContext.Player = this;
    }

    private void Update()
    {
        HandleMovement();
    }

    private void LateUpdate()
    {
        HandleTurning();
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
        Transform cameraTransform = CameraManager.Instance.CurrentCamera.transform;
        Vector3 cameraEulerAngles = cameraTransform.eulerAngles;
        float cameraPitch = cameraEulerAngles.x;
        float cameraYaw = cameraEulerAngles.y;

        transform.rotation = Quaternion.Euler(0f, cameraYaw, 0f);
        
        Quaternion targetHandRotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
        m_handTransform.rotation = Quaternion.Slerp(m_handTransform.rotation, targetHandRotation, Time.deltaTime * 15f);

        Vector3 targetHandPosition = cameraTransform.position + cameraTransform.TransformVector(m_handPositionOffset);
        m_handTransform.position = Vector3.Lerp(m_handTransform.position, targetHandPosition, Time.deltaTime * 15f);
    }
    
    public void OnMoveInput(Vector2 input)
    {
        Vector3 moveDirection = new Vector3(input.x, 0, input.y);
        
        m_currentMoveDirection = transform.TransformDirection(moveDirection);
    }
    
    public void OnToggleFlashlightInput()
    {
        m_flashLight.Toggle();
    }

    public bool IsInDarkness()
    {
        float lightIntensity = LightManager.Instance.GetLightIntensityAtPosition(CenterPosition);
        return lightIntensity < 0.1f;
    }
}
