using System;
using System.Collections.Generic;
using CustomToolkit.StateMachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum ESmallMothState
{
    State_Idle,
    State_MoveTowardsLight,
    State_Ascending,
    State_Descending
}

public class SmallMoth : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] 
    private AudioClip[] m_footstepClips; 
    
    [SerializeField]
    private float m_maxHearDistance = 5.0f;
    
    [SerializeField]
    private Animator m_animator;
    public Animator Animator => m_animator;
    
    [SerializeField]
    private Transform m_headTransform;
    public Transform HeadTransform => m_headTransform;
    
    [SerializeField]
    private float m_timeToAscend = 5.0f;
    public float TimeToAscend => m_timeToAscend;
    
    [SerializeField]
    private float m_turnRate = 5.0f;
    public float TurnRate => m_turnRate;
    
    [SerializeField]
    private float m_lightDetectionRadius = 10f;
    public float LightDetectionRadius => m_lightDetectionRadius;
    
    [SerializeField]
    private float m_lightFollowDistanceThreshold = 1.0f;
    public float LightFollowDistanceThreshold => m_lightFollowDistanceThreshold;
    
    [SerializeField]
    private NavMeshAgent m_navmeshAgent;
    public NavMeshAgent NavmeshAgent => m_navmeshAgent;
    
    private StateMachine m_stateMachine;
    public StateMachine StateMachine => m_stateMachine;

    private Light m_currentLightTarget;
    public Light CurrentLightTarget => m_currentLightTarget;
    
    private AngelLamp m_targetAngelLamp;
    public AngelLamp TargetAngelLamp => m_targetAngelLamp;
    
    private Rigidbody m_rigidbody;
    public Rigidbody Rigidbody => m_rigidbody;
    
    private int m_velocityHash = Animator.StringToHash("Velocity");

    private MothAnimationEventListener m_animationEventListener;
    
    private void Awake()
    {
        m_animationEventListener = GetComponentInChildren<MothAnimationEventListener>();
        
        m_navmeshAgent.updateRotation = false;
        
        Dictionary<Enum, State> states = new Dictionary<Enum, State>()
        {
            {ESmallMothState.State_Idle, new State_Idle(this)},
            {ESmallMothState.State_MoveTowardsLight, new State_MoveTowardsLight(this)},
            {ESmallMothState.State_Ascending, new State_Ascending(this)},
        };
        
        m_stateMachine = new StateMachine(states);
    }

    private void OnEnable()
    {
        if (m_animationEventListener != null)
        {
            m_animationEventListener.OnStepEvent += PlayFootstep;
        }
    }

    private void OnDisable()
    {
        if (m_animationEventListener != null)
        {
            m_animationEventListener.OnStepEvent -= PlayFootstep;
        }
    }

    private void Update()
    {
        if(m_stateMachine == null)
            return;
        
        if ((ESmallMothState)m_stateMachine.CurrentStateType == ESmallMothState.State_Ascending)
        {
            if (m_targetAngelLamp == null || !m_targetAngelLamp.IsOn)
            {
                m_targetAngelLamp = null;
                m_stateMachine.SetState(ESmallMothState.State_Idle);
            }
        }
        else
        {
            UpdateLightTarget();   
        }
        
        m_stateMachine.Update();
    }

    private void LateUpdate()
    {
        m_animator.SetFloat(m_velocityHash, m_navmeshAgent.velocity.magnitude);
    }

    private void UpdateLightTarget()
    {
        Light targetLight = null;
        float bestScore = 0f;
        
        foreach (Light activeLight in LightManager.Instance.ActiveLights)
        {
            if (!activeLight.enabled || activeLight.intensity <= 0f)
                continue;

            Vector3 dirToLightXZ = activeLight.transform.position - transform.position;
            dirToLightXZ.y = 0;
            if (dirToLightXZ.sqrMagnitude > m_lightDetectionRadius * m_lightDetectionRadius)
                continue;
            
            float distanceToLight = dirToLightXZ.magnitude;
            float score = activeLight.intensity / (distanceToLight + 1f);

            if (score > bestScore)
            {
                bestScore = score;
                targetLight = activeLight;
            }
        }
        
        m_currentLightTarget = targetLight;
    }

    public void OnEnterAngelLight(AngelLamp angelLamp)
    {
        m_targetAngelLamp = angelLamp;
        m_stateMachine.SetState(ESmallMothState.State_Ascending);
    }
    
    private void PlayFootstep()
    {
        if (m_footstepClips.Length == 0)
            return;

        AudioClip clip = m_footstepClips[Random.Range(0, m_footstepClips.Length)];
        SoundManager.Instance.PlaySoundFX(clip, transform, 1.0f, true, m_maxHearDistance);
    }
}
