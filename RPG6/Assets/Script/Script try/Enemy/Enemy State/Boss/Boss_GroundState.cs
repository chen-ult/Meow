using Unity.VisualScripting;
using UnityEngine;

public class Boss_GroundState : BossState
{
    
    public Boss_GroundState(Enemy enemy, StateMachine stateMachine, Boss boss, string animBoolName) : base(enemy, stateMachine, boss, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerDetected())
            stateMachine.ChangeState(boss.boss_battleState);
    }
}
