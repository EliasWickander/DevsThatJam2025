using UnityEngine;
using CustomToolkit.StateMachine;
using UnityEngine.AI;

public class State_Chase : State
{
    private BigMoth m_mothOwner;
    private NavMeshAgent m_navAgent;
    private float m_losePlayerTimer = 0f;
    private float m_updateDestinationInterval = 0.5f; // How often to update player position
    private float m_destinationUpdateTimer = 0f; // Timer for destination updates

    private Transform m_targetPlayer;
    public State_Chase(BigMoth owner) : base(owner.gameObject)
    {
        m_mothOwner = owner;
        m_navAgent = owner.GetComponent<NavMeshAgent>();
    }

    public override void OnEnter(State prevState)
    {
        m_navAgent.updateRotation = false;
        m_targetPlayer = GameContext.Player.transform;
        m_navAgent.speed = m_mothOwner.ChaseSpeed;
        m_losePlayerTimer = m_mothOwner.LosePlayerTime;
        m_destinationUpdateTimer = 0f;
        
        UpdateDestination();
    }

    public override void Update()
    {
        if (IsInAttackRange())
        {
            m_mothOwner.StateMachine.SetState(EBigMothState.State_Attack);
            return;
        }
        
        m_destinationUpdateTimer -= Time.deltaTime;
        if (m_destinationUpdateTimer <= 0f)
        {
            UpdateDestination();
            m_destinationUpdateTimer = m_updateDestinationInterval;
        }
        
        if (m_mothOwner.CanSeePlayer)
        {
            m_losePlayerTimer = m_mothOwner.LosePlayerTime;
        }
        else
        {
            m_losePlayerTimer -= Time.deltaTime;
            if (m_losePlayerTimer <= 0f)
            {
                m_mothOwner.StateMachine.SetState(EBigMothState.State_Patrol);
                return;
            }
        }
        
        m_mothOwner.RotateTowards(m_targetPlayer.position);
    }

    public override void OnExit(State nextState)
    {
        m_navAgent.ResetPath();
        m_losePlayerTimer = 0f;
        m_destinationUpdateTimer = 0f;
    }
    
    private void UpdateDestination()
    {
        // Set destination to player's current position
        m_navAgent.SetDestination(m_targetPlayer.position);
    }

    private bool IsInAttackRange()
    {
        Vector3 dirToPlayerXZ = m_targetPlayer.position - m_mothOwner.transform.position;
        dirToPlayerXZ.y = 0;
        float sqrDistance = dirToPlayerXZ.sqrMagnitude;
        
        return sqrDistance <= m_mothOwner.AttackRange * m_mothOwner.AttackRange;
    }
}
