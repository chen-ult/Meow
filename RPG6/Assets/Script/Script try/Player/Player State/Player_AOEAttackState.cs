using UnityEngine;

public class Player_AOEAttackState : PlayerState
{

    public Player_AOEAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    { 
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = skillManager.aoe.GetAoeDuration();
        skillManager.aoe.PerformAOE();

    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(0, rb.linearVelocity.y);



        if(stateTimer < 0)
            stateMachine.ChangeState(player.idleState);
    }

}
