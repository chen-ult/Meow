using Unity.VisualScripting;
using UnityEngine;

public class BossState : EnemyState
{
    protected Boss boss;
    public BossState(Enemy enemy, StateMachine stateMachine,Boss boss, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        this.boss = boss;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

       
    }
     public override void Exit()
    {
        base.Exit();
    }
}
