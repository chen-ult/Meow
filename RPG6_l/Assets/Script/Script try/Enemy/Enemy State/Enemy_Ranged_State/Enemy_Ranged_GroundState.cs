using UnityEngine;

public class Enemy_Ranged_GroundState : Enemy_RangedState
{
    public Enemy_Ranged_GroundState(Enemy enemy, StateMachine stateMachine, Enemy_Mage mage, string animBoolName) : base(enemy, stateMachine, mage, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerDetected())
            stateMachine.ChangeState(mage.ranged_BattleState);
    }
}
