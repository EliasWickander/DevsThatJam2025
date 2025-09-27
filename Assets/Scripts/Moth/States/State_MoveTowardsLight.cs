using UnityEngine;
using CustomToolkit.StateMachine;
using UnityEngine.AI;

public class State_MoveTowardsLight : State
{
    private SmallMoth m_mothOwner;
    public State_MoveTowardsLight(SmallMoth owner) : base(owner.gameObject)
    {
        m_mothOwner = owner;
    }

    public override void OnEnter(State prevState)
    {
        
    }

    public override void Update()
    {
        Light targetLight = m_mothOwner.CurrentLightTarget;

        if (targetLight != null)
        {
            MoveToLight(targetLight);
        }
        else
        {
            m_mothOwner.StateMachine.SetState(ESmallMothState.State_Idle);
        }
    }

    public override void OnExit(State nextState)
    {
        m_mothOwner.NavmeshAgent.ResetPath();
    }

    private void MoveToLight(Light light)
    {
        Vector3 targetPosition = CalculateTargetPosition(light);
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, 2f, NavMesh.AllAreas))
        {
            m_mothOwner.NavmeshAgent.SetDestination(hit.position);
        }
        else
        {
            //Stop if no valid path
            m_mothOwner.NavmeshAgent.ResetPath();
            m_mothOwner.StateMachine.SetState(ESmallMothState.State_Idle);
        }
    }

    private Vector3 CalculateTargetPosition(Light light)
    {
        return light.transform.position;
    }
}
