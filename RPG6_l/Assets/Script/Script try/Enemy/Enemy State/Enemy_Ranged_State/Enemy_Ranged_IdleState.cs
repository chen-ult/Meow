using UnityEngine;

public class Enemy_Ranged_IdleState : Enemy_Ranged_GroundState
{
    public Enemy_Ranged_IdleState(Enemy enemy, StateMachine stateMachine, Enemy_Mage mage, string animBoolName) : base(enemy, stateMachine, mage, animBoolName)
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
            stateMachine.ChangeState(mage.ranged_MoveState);
    }
}
