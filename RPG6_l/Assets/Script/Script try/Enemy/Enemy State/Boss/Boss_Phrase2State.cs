using UnityEngine;

public class Boss_Phrase2State : BossState
{
    public Boss_Phrase2State(Enemy enemy, StateMachine stateMachine, Boss boss, string animBoolName)
        : base(enemy, stateMachine, boss, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("11111");

        boss.SetInvulnerable(true);

        enemy.SetVelocity(0, 0);
        
    }

    public override void Update()
    {
        base.Update();

        // 二阶段过渡动画结束时，由动画事件设置 triggerCalled = true
        // 然后这里切回战斗状态，使用新的二阶段行为（伤害加倍等）
        if (triggerCalled)
        {
            boss.EnterPhase2Combat();
        }
    }

    public override void Exit()
    {
        base.Exit();
        boss.SetInvulnerable(false);
    }
}
