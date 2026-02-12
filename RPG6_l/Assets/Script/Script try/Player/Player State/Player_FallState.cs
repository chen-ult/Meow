using UnityEngine;

public class Player_FallState : Player_AiredState
{
    public Player_FallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (player.groundDetected)
            stateMachine.ChangeState(player.idleState);

        if (player.wallDetected && player.abilityUnlocked[SkillType.WallSlide])
            stateMachine.ChangeState(player.wallSlideState);
    }
}
