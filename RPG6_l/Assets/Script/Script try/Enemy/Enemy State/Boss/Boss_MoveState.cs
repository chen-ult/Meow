using UnityEngine;

public class Boss_MoveState : Boss_GroundState
{
    public Boss_MoveState(Enemy enemy, StateMachine stateMachine, Boss boss, string animBoolName) : base(enemy, stateMachine, boss, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();


    }


    public override void Update()
    {
        base.Update();

        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, rb.linearVelocity.y);

        if (!enemy.groundDetected || enemy.wallDetected)
            stateMachine.ChangeState(boss.boss_idleState);

    }
}
