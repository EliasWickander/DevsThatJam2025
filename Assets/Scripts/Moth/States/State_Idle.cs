using UnityEngine;
using CustomToolkit.StateMachine;

public class State_Idle : State
{
    private SmallMoth m_mothOwner;
    public State_Idle(SmallMoth owner) : base(owner.gameObject)
    {
        m_mothOwner = owner;
    }

    public override void OnEnter(State prevState)
    {
        
    }

    public override void Update()
    {
        if(m_mothOwner.CurrentLightTarget != null)
            m_mothOwner.StateMachine.SetState(ESmallMothState.State_MoveTowardsLight);
    }

    public override void OnExit(State nextState)
    {
        
    }
}
