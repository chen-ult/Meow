using UnityEngine;

public class Enemy_Ranged_MoveState : Enemy_Ranged_GroundState
{
    public Enemy_Ranged_MoveState(Enemy enemy, StateMachine stateMachine, Enemy_Mage mage, string animBoolName) : base(enemy, stateMachine, mage, animBoolName)
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
            stateMachine.ChangeState(mage.ranged_IdleState);

    }
}
