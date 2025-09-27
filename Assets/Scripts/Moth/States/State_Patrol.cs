using UnityEngine;
using CustomToolkit.StateMachine;
using UnityEngine.AI;

public class State_Patrol : State
{
    private BigMoth m_mothOwner;
    private NavMeshAgent m_navmeshAgent;
    
    private Vector3 m_targetWaypoint;
    private float m_waitTimer = 0.0f;
    private bool m_isWaiting;
    private float m_tolerance = 1.0f;
    private Transform m_lastWaypoint;
    public State_Patrol(BigMoth owner) : base(owner.gameObject)
    {
        m_mothOwner = owner;
        m_navmeshAgent = owner.NavmeshAgent;
    }

    public override void OnEnter(State prevState)
    {
        m_navmeshAgent.speed = m_mothOwner.PatrolSpeed;
        m_navmeshAgent.isStopped = false;
        m_isWaiting = false;
        
        UpdatePatrolPoint();
    }

    public override void Update()
    {
        if (m_mothOwner.CanSeePlayer)
        {
            m_mothOwner.StateMachine.SetState(EBigMothState.State_Chase);
            return;
        }

        if (m_isWaiting)
        {
            m_waitTimer -= Time.deltaTime;
            if (m_waitTimer <= 0f)
            {
                m_isWaiting = false;
                UpdatePatrolPoint();
            }
            return;
        }

        // Reached waypoint, decide to wait or move to next
        float sqrDistance = (m_mothOwner.transform.position - m_targetWaypoint).sqrMagnitude;
        if (sqrDistance <= m_tolerance * m_tolerance)
        {
            if (Random.value < 0.4f)
            {
                m_isWaiting = true;
                m_waitTimer = Random.Range(m_mothOwner.WaitTimeMin, m_mothOwner.WaitTimeMax);
                m_navmeshAgent.isStopped = true;
            }
            else
            {
                UpdatePatrolPoint();
            }
        }
        
        m_mothOwner.RotateTowards(m_targetWaypoint);
    }

    public override void OnExit(State nextState)
    {
        m_navmeshAgent.isStopped = false;
        m_waitTimer = 0f;
        m_isWaiting = false;
    }

    private void UpdatePatrolPoint()
    {
        if (m_mothOwner.PatrolWaypoints == null)
            return;
        
        Transform[] availablePoints = m_mothOwner.PatrolWaypoints;
        if (m_lastWaypoint != null && availablePoints.Length > 1)
            availablePoints = System.Array.FindAll(m_mothOwner.PatrolWaypoints, p => p != m_lastWaypoint);
        
        int randomIndex = Random.Range(0, availablePoints.Length);
        m_lastWaypoint = availablePoints[randomIndex];
        m_targetWaypoint = m_lastWaypoint.position;

        m_navmeshAgent.SetDestination(m_targetWaypoint);
        m_navmeshAgent.isStopped = false;
    }
}
