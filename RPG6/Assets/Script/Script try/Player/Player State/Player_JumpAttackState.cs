using UnityEngine;

public class Player_JumpAttackState : PlayerState
{
    private bool touchedGround;
    private int jumpAttackDir;
    public Player_JumpAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        jumpAttackDir = player.facingDir;

        player.SetVelocity(player.jumpAttackVelocity.x * jumpAttackDir, player.jumpAttackVelocity.y);
        touchedGround = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(player.groundDetected && !touchedGround )
        {
            touchedGround = true;
            anim.SetTrigger("jumpAttackTrigger");
            player.SetVelocity(0, rb.linearVelocity.y);
        }

        if (triggerCalled && player.groundDetected)
            stateMachine.ChangeState(player.idleState);
    }
}
