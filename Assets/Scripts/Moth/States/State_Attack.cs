using UnityEngine;
using CustomToolkit.StateMachine;

public class State_Attack : State
{
    private BigMoth m_mothOwner;
    public State_Attack(BigMoth owner) : base(owner.gameObject)
    {
        m_mothOwner = owner;
    }

    public override void OnEnter(State prevState)
    {
        GameContext.Player.Kill();
    }

    public override void Update()
    {

    }

    public override void OnExit(State nextState)
    {
        
    }
}
