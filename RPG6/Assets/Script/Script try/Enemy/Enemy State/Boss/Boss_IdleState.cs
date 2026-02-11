using UnityEngine;

public class Boss_IdleState : Boss_GroundState
{
    public Boss_IdleState(Enemy enemy, StateMachine stateMachine, Boss boss, string animBoolName) : base(enemy, stateMachine, boss, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.idleTime;
    }

    public override void Exit()
    {
        base.Exit();

        if (!enemy.groundDetected || enemy.wallDetected)
            enemy.Flip();
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            stateMachine.ChangeState(boss.boss_moveState);
    }
}
