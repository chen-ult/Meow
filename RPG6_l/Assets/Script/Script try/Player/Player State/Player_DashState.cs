using UnityEngine;

public class Player_DashState : PlayerState
{
    private float originalGravityScales;
    private int dashDir;
    public Player_DashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        skillManager.dash.OnStartEffect();
        player.vfx.DoImageEchoEffect(player.dashDuration);

        
        dashDir = player.moveInput.x != 0 ? ((int)player.moveInput.x) : player.facingDir;
        stateTimer = player.dashDuration;

        originalGravityScales = rb.gravityScale;
        rb.gravityScale = 0;
        // make player invulnerable during dash
        player.SetInvulnerable(true);
    }

    public override void Exit()
    {
        base.Exit();

        skillManager.dash.OnEndEffect();

        player.SetVelocity(0, 0);
        rb.gravityScale = originalGravityScales;
        // remove invulnerability when dash ends
        player.SetInvulnerable(false);
    }

    public override void Update()
    {
        base.Update();
        CancelDashIfNeeded();

        player.SetVelocity(player.dashSpeed * dashDir, 0);

        if (stateTimer <0)
        {
            if (player.groundDetected)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.fallState);
        }
    }

    private void CancelDashIfNeeded()
    {
        if(player.wallDetected)
        {
            if (player.groundDetected)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.wallSlideState);
        }
    }
}
