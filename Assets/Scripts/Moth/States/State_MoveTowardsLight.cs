using UnityEngine;
using CustomToolkit.StateMachine;
using UnityEngine.AI;

public class State_MoveTowardsLight : State
{
    private SmallMoth m_mothOwner;

    private bool m_hasReachedTarget = false;
    public State_MoveTowardsLight(SmallMoth owner) : base(owner.gameObject)
    {
        m_mothOwner = owner;
    }

    public override void OnEnter(State prevState)
    {
        
    }

    public override void Update()
    {
        Light targetLightToMoveTo = m_mothOwner.CanSeeFlashlight ? m_mothOwner.LightFromFlashlight : m_mothOwner.CurrentFascinationLightTarget;
        Light targetLightToLookAt = m_mothOwner.CanSeeFlashlight ? m_mothOwner.LightFromFlashlight : m_mothOwner.CurrentFascinationLightTarget;

        Vector3 headRotationTarget = targetLightToLookAt.transform.position;
        Vector3 bodyRotationTarget = targetLightToLookAt.transform.position;
        
        if (targetLightToLookAt == null && targetLightToMoveTo == null)
        {
            m_mothOwner.StateMachine.SetState(ESmallMothState.State_Idle);
            return;
        }
            
        Vector3 dirToLightXZ = targetLightToMoveTo.transform.position - m_mothOwner.transform.position;
        dirToLightXZ.y = 0; 
        float sqrDistanceToLight = dirToLightXZ.sqrMagnitude;
            
        if (sqrDistanceToLight > m_mothOwner.LightFollowDistanceThreshold * m_mothOwner.LightFollowDistanceThreshold)
        {
            MoveToLight(targetLightToMoveTo);
        }
        else
        {
            if (m_mothOwner.CanSeeFlashlight)
                headRotationTarget = GameContext.Player.HeadTransform.position;

            m_mothOwner.NavmeshAgent.ResetPath();
            m_hasReachedTarget = true;
        }
            
        RotateBodyTowards(bodyRotationTarget);
        RotateHeadTowards(headRotationTarget);
    }

    public override void OnExit(State nextState)
    {
        m_mothOwner.NavmeshAgent.ResetPath();
    }

    private void MoveToLight(Light light)
    {
        Vector3 targetPosition = CalculateTargetPosition(light);
        m_hasReachedTarget = false;

        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
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
    
    private void RotateBodyTowards(Vector3 targetPosition)
    {
        Vector3 toTarget = targetPosition - m_mothOwner.transform.position;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);

        if (toTargetXZ.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(toTargetXZ.normalized, Vector3.up);
        m_mothOwner.transform.rotation = Quaternion.Slerp(m_mothOwner.transform.rotation, targetRot, m_mothOwner.TurnRate * Time.deltaTime);
    }

    private void RotateHeadTowards(Vector3 targetPosition)
    {
        Vector3 toTarget = targetPosition - m_mothOwner.HeadTransform.position;

        if (toTarget.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
        m_mothOwner.HeadTransform.rotation = Quaternion.Slerp(m_mothOwner.HeadTransform.rotation, targetRot, m_mothOwner.TurnRate * Time.deltaTime);
    }
}
