using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [SerializeField] 
    public CharacterController m_characterController;
    
    [Header("Audio")]
    [SerializeField] 
    private AudioClip[] m_footstepClips; 
    
    [SerializeField] 
    private float m_stepInterval = 0.5f;
    
    [SerializeField]
    private List<AudioClip> m_jumpScareClips;
    
    [SerializeField]
    private float m_jumpScareVolume = 1.0f;
    
    [SerializeField]
    private float m_howLongUntilNextJumpScare = 15.0f;

    [SerializeField]
    private List<AudioClip> m_monsterAmbienceClip;
    
    [SerializeField]
    private float m_ambienceRange = 15.0f;
    
    [SerializeField]
    private float m_ambienceRegularityMin = 30.0f;
    
    [SerializeField]
    private float m_ambienceRegularityMax = 60.0f;
    
    [SerializeField]
    private PlayerInput m_playerInput;
    
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
    
    public Flashlight FlashLight => m_flashLight;
    
    [SerializeField] 
    private float m_moveSpeed = 8;

    private Vector2 m_currentMoveInput;
    private Vector3 m_currentVelocity = Vector3.zero;
    public Vector3 CurrentVelocity => m_currentVelocity;

    private float m_stepTimer = 0.0f;
    
    [SerializeField]
    private LayerMask m_obstructionLayerMask;
    
    private float m_timeSinceLastJumpscared = Mathf.Infinity;
    
    private bool m_wasMonsterSeenLastFrame = false;
    
    private float m_timeSinceLastAmbience = Mathf.Infinity;
    private float m_nextAmbienceTime = 0.0f;
    

    private void OnValidate()
    {
        m_characterController = GetComponent<CharacterController>();
        m_playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        m_playerInput.OnMoveInputEvent += OnMoveInput;
        m_playerInput.OnToggleFlashlightInputEvent += OnToggleFlashlightInput;
        GameManager.Instance.OnBeginPlayerAscension += OnBeginPlayerAscension;
    }

    private void OnBeginPlayerAscension()
    {
        m_characterController.enabled = false;
        m_playerInput.enabled = false;
    }

    private void OnDisable()
    {
        m_playerInput.OnMoveInputEvent -= OnMoveInput;
        m_playerInput.OnToggleFlashlightInputEvent -= OnToggleFlashlightInput;
        GameManager.Instance.OnBeginPlayerAscension -= OnBeginPlayerAscension;
    }

    private void Start()
    {
        GameContext.Player = this;
        m_timeSinceLastJumpscared = Mathf.Infinity;
    }

    private void Update()
    {
        if (m_characterController.enabled)
        {
            HandleMovement();
            HandleFootsteps();
            HandleJumpscares();
            HandleMonsterAmbience();
        }
    }

    private void HandleMonsterAmbience()
    {
        if(GameContext.BigMoth == null)
            return;
        
        if(Vector3.Distance(GameContext.BigMoth.transform.position, transform.position) < m_ambienceRange)
        {
            if(m_timeSinceLastAmbience >= m_nextAmbienceTime)
            {
                for(int i = 0; i < m_monsterAmbienceClip.Count; i++)
                {
                    AudioClip clip = m_monsterAmbienceClip[i];
                    SoundManager.Instance.PlaySoundFX(clip, transform, 1.0f, false);
                }
                
                m_timeSinceLastAmbience = 0.0f;
                m_nextAmbienceTime = Random.Range(m_ambienceRegularityMin, m_ambienceRegularityMax);
            }
        }
        
        m_timeSinceLastAmbience += Time.deltaTime;
    }

    private void LateUpdate()
    {
        HandleTurning();
    }
    
    private void HandleMovement()
    {
        Vector3 moveDirection = new Vector3(m_currentMoveInput.x, 0, m_currentMoveInput.y);
        moveDirection = transform.TransformDirection(moveDirection);
        
        m_currentVelocity.x = moveDirection.x * m_moveSpeed;
        m_currentVelocity.z = moveDirection.z * m_moveSpeed;
        
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
    
    private void HandleJumpscares()
    {
        bool isMonsterSeen = IsMonsterSeenByFlashlight();
        if (isMonsterSeen && !m_wasMonsterSeenLastFrame)
        {
            if (m_timeSinceLastJumpscared >= m_howLongUntilNextJumpScare)
            {
                if (m_jumpScareClips.Count > 0)
                {
                    AudioClip clip = m_jumpScareClips[Random.Range(0, m_jumpScareClips.Count)];
                    SoundManager.Instance.PlaySoundFX(clip, transform, m_jumpScareVolume, false);
                }
                    
                m_timeSinceLastJumpscared = 0.0f;
            }
        }
        
        m_timeSinceLastJumpscared += Time.deltaTime;
        m_wasMonsterSeenLastFrame = isMonsterSeen;
    }
    
    public void OnMoveInput(Vector2 input)
    {
        m_currentMoveInput = input;
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

    public void Kill()
    {
        GameManager.Instance.GameOver();
    }

    private void HandleFootsteps()
    {
        if(m_currentVelocity.magnitude > 0.1f)
        {
            m_stepTimer += Time.deltaTime;
            if(m_stepTimer >= m_stepInterval)
            {
                PlayFootstep();
                m_stepTimer = 0f;
            }
        }
        else
        {
            m_stepTimer = m_stepInterval;
        }
    }
    private void PlayFootstep()
    {
        if (m_footstepClips.Length == 0)
            return;

        AudioClip clip = m_footstepClips[Random.Range(0, m_footstepClips.Length)];
        SoundManager.Instance.PlaySoundFX(clip, transform, 1.0f);
    }
    
    bool IsMonsterSeenByFlashlight()
    {
        BigMoth bigMoth = GameContext.BigMoth;
        if(bigMoth == null)
            return false;
        
        Light spotlight = m_flashLight.LightObject;
        
        Vector3 spotlightPos = spotlight.transform.position;
        Vector3 spotlightForward = spotlight.transform.forward;
        float maxDistance = spotlight.range;
        float halfSpotAngle = spotlight.spotAngle * 0.5f;
        
        Vector3 targetPos = bigMoth.transform.position;
        Vector3 directionToTarget = (targetPos - spotlightPos).normalized;
        float distanceToTarget = Vector3.Distance(spotlightPos, targetPos);
        
        if (distanceToTarget > maxDistance)
            return false;
        
        float angleToTarget = Vector3.Angle(spotlightForward, directionToTarget);
        if (angleToTarget > halfSpotAngle)
            return false;
        
        if (Physics.Raycast(spotlightPos, directionToTarget, out RaycastHit hit, distanceToTarget, m_obstructionLayerMask))
            return false;

        return true;
    }
}
