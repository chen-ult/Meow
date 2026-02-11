using UnityEngine;

public class Boss_StunnedState : BossState
{
    private Enemy_VFX vfx;
    public Boss_StunnedState(Enemy enemy, StateMachine stateMachine, Boss boss, string animBoolName) : base(enemy, stateMachine, boss, animBoolName)
    {
        vfx = enemy.GetComponent<Enemy_VFX>();
    }

    

    public override void Enter()
    {
        base.Enter();

        vfx.EnableAttackAlert(false);
        enemy.EnableCounterWindow(false);
        stateTimer = enemy.stunnedDuration;
        rb.linearVelocity = new Vector2(enemy.stunnedVelocity.x * -enemy.facingDir, enemy.stunnedVelocity.y);
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            stateMachine.ChangeState(boss.boss_idleState);
    }
}
