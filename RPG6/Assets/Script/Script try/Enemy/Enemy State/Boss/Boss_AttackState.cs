using UnityEngine;

public class Boss_AttackState : BossState
{
    public Boss_AttackState(Enemy enemy, StateMachine stateMachine, Boss boss, string animBoolName) : base(enemy, stateMachine, boss, animBoolName)
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
            stateMachine.ChangeState(boss.boss_battleState);
    }
}
