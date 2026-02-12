using UnityEngine;

public class Boss_DeadState : BossState
{
    private Collider2D col;
    public Boss_DeadState(Enemy enemy, StateMachine stateMachine, Boss boss, string animBoolName) : base(enemy, stateMachine, boss, animBoolName)
    {
        col = enemy.GetComponent<Collider2D>();
    }

    

    public override void Enter()
    {
        base.Enter();
        boss.SetVelocity(0, 0);



        stateMachine.SwitchOffStateMachine();
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
        {
            Object.Destroy(boss.gameObject);
        }
    }
}
