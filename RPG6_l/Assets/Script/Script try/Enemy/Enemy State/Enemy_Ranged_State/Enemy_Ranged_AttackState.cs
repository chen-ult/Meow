using UnityEngine;

public class Enemy_Ranged_AttackState : Enemy_RangedState
{
    public Enemy_Ranged_AttackState(Enemy enemy, StateMachine stateMachine, Enemy_Mage mage, string animBoolName) : base(enemy, stateMachine, mage, animBoolName)
    {
    }


    public override void Enter()
    {
        base.Enter();

        SyncAttackSpeed();
        
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
        {
            stateMachine.ChangeState(mage.ranged_BattleState);
            return;
        }

        
    }
}
