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
    private Vector3 m_targetAngelLampPos;
    
    public State_Ascending(SmallMoth owner) : base(owner.gameObject)
    {
        m_mothOwner = owner;
    }

    public override void OnEnter(State prevState)
    {
        m_mothOwner.NavmeshAgent.enabled = false;
        m_ascendTimer = 0.0f;
        m_startMothPosition = m_mothOwner.transform.position;
        m_targetAngelLampPos = m_mothOwner.TargetAngelLamp.transform.position;
    }

    public override void Update()
    {
        HandleAscension(m_targetAngelLampPos);
    }

    private void HandleAscension(Vector3 targetPos)
    {
        if(AscendProgress >= 1.0f)
        {
            m_hasReachedTarget = true;
            OnReachedTargetLight();
            return;
        }
        
        m_mothOwner.transform.position = Vector3.Lerp(m_startMothPosition, targetPos, AscendProgress);
        m_ascendTimer += Time.deltaTime;
        
        RotateHeadTowards(targetPos);
    }
    
    public override void OnExit(State nextState)
    {
        
    }

    private void OnReachedTargetLight()
    {
        m_mothOwner.OnAscended();
    }
    
    private void RotateHeadTowards(Vector3 targetPosition)
    {
        Vector3 toTarget = targetPosition - m_mothOwner.HeadTransform.position;

        if (toTarget.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
        m_mothOwner.HeadTransform.rotation = targetRot;
    }
}
