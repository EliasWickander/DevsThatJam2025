using UnityEngine;
using CustomToolkit.StateMachine;
using UnityEngine.AI;

public class State_Ascending : State
{
    private SmallMoth m_mothOwner;
    
    private bool m_hasReachedTarget = false;

    private float m_ascendTimer = 0.0f;
    
    private float AscendProgress => Mathf.Clamp01(m_ascendTimer / m_mothOwner.TimeToAscend);
    
    private Vector3 m_startMothPosition;
    
    public State_Ascending(SmallMoth owner) : base(owner.gameObject)
    {
        m_mothOwner = owner;
    }

    public override void OnEnter(State prevState)
    {
        m_mothOwner.NavmeshAgent.enabled = false;
        m_ascendTimer = 0.0f;
        m_startMothPosition = m_mothOwner.transform.position;
    }

    public override void Update()
    {
        AngelLamp targetAngelLamp = m_mothOwner.TargetAngelLamp;

        if (targetAngelLamp == null)
        {
            m_mothOwner.StateMachine.SetState(ESmallMothState.State_Idle);
            return;
        }

        HandleAscension(targetAngelLamp);
    }

    private void HandleAscension(AngelLamp targetAngelLamp)
    {
        if(AscendProgress >= 1.0f)
        {
            m_hasReachedTarget = true;
            OnReachedTargetLight();
            return;
        }
        
        m_mothOwner.transform.position = Vector3.Lerp(m_startMothPosition, targetAngelLamp.transform.position, AscendProgress);
        m_ascendTimer += Time.deltaTime;
        
        RotateHeadTowards(m_mothOwner.TargetAngelLamp.transform.position);
    }
    
    public override void OnExit(State nextState)
    {
        
    }

    private void OnReachedTargetLight()
    {
        
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
