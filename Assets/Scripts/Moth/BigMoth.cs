using System;
using System.Collections.Generic;
using CustomToolkit.StateMachine;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum EBigMothState
{
    State_Patrol,
    State_Chase,
    State_Attack
}

public class BigMoth : MonoBehaviour
{
    [SerializeField]
    private Animator m_animator;
    public Animator Animator => m_animator;
    
    [Header("Audio")]
    [SerializeField] 
    private AudioClip[] m_footstepClips; 
    
    [SerializeField]
    private float m_maxHearDistance = 5.0f;
    
    [SerializeField]
    private AudioClip m_roarClip;
    
    [SerializeField]
    private float m_roarVolume = 1.0f;
    
    [SerializeField]
    private NavMeshAgent m_navmeshAgent;
    public NavMeshAgent NavmeshAgent => m_navmeshAgent;
    
    [SerializeField]
    private Transform m_headTransform;
    
    [SerializeField]
    private float m_turnRate = 5.0f;
    public float TurnRate => m_turnRate;
    
    [Header("Chase")]
    [SerializeField]
    private float m_chaseSpeed = 5.0f;
    public float ChaseSpeed => m_chaseSpeed;
    
    [SerializeField]
    private float m_losePlayerTime = 3.0f;
    public float LosePlayerTime => m_losePlayerTime;
    
    [SerializeField]
    private float m_updateDestinationInterval = 0.5f;
    public float UpdateDestinationInterval => m_updateDestinationInterval;
    
    [SerializeField]
    private float m_attackRange = 2.0f;
    public float AttackRange => m_attackRange;

    [Header("Patrol")]
    [SerializeField]
    private float m_patrolSpeed = 2.5f;
    public float PatrolSpeed => m_patrolSpeed;
    
    [SerializeField]
    private float m_waitTimeMin = 2.0f;
    public float WaitTimeMin => m_waitTimeMin;
    
    [SerializeField]
    private float m_waitTimeMax = 5.0f;
    public float WaitTimeMax => m_waitTimeMax;

    private Transform[] m_patrolWaypoints;
    public Transform[] PatrolWaypoints => m_patrolWaypoints;
    
    [Header("Detection")]
    [SerializeField]
    private float m_visionRange = 15f;
    public float VisionRange => m_visionRange;
    
    [SerializeField]
    private float m_visionAngle = 90f;
    public float VisionAngle => m_visionAngle;
    
    [SerializeField]
    private float m_darknessVisionReduction = 0.2f;
    public float DarknessVisionReduction => m_darknessVisionReduction;
    
    private StateMachine m_stateMachine;
    public StateMachine StateMachine => m_stateMachine;

    private bool m_canSeePlayer;
    public bool CanSeePlayer => m_canSeePlayer;
    
    private float m_stepTimer = 0.0f;
    
    private int m_velocityHash = Animator.StringToHash("Velocity");
    
    private MothAnimationEventListener m_animationEventListener;
    
    private void Awake()
    {
        m_animationEventListener = GetComponentInChildren<MothAnimationEventListener>();
        
        Dictionary<Enum, State> states = new Dictionary<Enum, State>()
        {
            {EBigMothState.State_Patrol, new State_Patrol(this)},
            {EBigMothState.State_Chase, new State_Chase(this)},
            {EBigMothState.State_Attack, new State_Attack(this)},
        };
        
        m_stateMachine = new StateMachine(states);
    }

    private void Start()
    {
        GameContext.BigMoth = this;
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
        m_canSeePlayer = IsPlayerInVision();

        if(m_stateMachine != null)
            m_stateMachine.Update();
    }
    
    private void LateUpdate()
    {
        m_animator.SetFloat(m_velocityHash, m_navmeshAgent.velocity.magnitude);
    }
    
    private bool IsPlayerInVision()
    {
        Vector3 directionToPlayer = GameContext.Player.CenterPosition - m_headTransform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        
        float effectiveVisionRange = m_visionRange;
        if (GameContext.Player.IsInDarkness())
            effectiveVisionRange *= m_darknessVisionReduction;
        
        if (distanceToPlayer > effectiveVisionRange)
            return false;

        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > m_visionAngle * 0.5f)
            return false;
        
        if (Physics.Raycast(m_headTransform.position, directionToPlayer.normalized, out RaycastHit hit, effectiveVisionRange))
        {
            if (hit.transform.CompareTag("Player"))
                return true;
        }

        return false;
    }
    
    public void RotateTowards(Vector3 targetPoint)
    {
        Vector3 dirToTargetXZ = targetPoint - transform.position;
        dirToTargetXZ.y = 0;
        
        if (dirToTargetXZ.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dirToTargetXZ);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_turnRate * Time.deltaTime);
        }
    }

    public void SetPatrolPoints(Transform[] points)
    {
        m_patrolWaypoints = points;
    }
    
    private void PlayFootstep()
    {
        if (m_footstepClips.Length == 0)
            return;

        AudioClip clip = m_footstepClips[Random.Range(0, m_footstepClips.Length)];
        SoundManager.Instance.PlaySoundFX(clip, transform, 1.0f, true, m_maxHearDistance);
    }

    public void PlayRoar()
    {
        if (m_roarClip == null)
            return;
        
        SoundManager.Instance.PlaySoundFX(m_roarClip, transform, m_roarVolume, true, m_maxHearDistance);
    }
}
