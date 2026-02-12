using UnityEngine;

public class Boss_DeadState : BossState
{
    private Collider2D col;
    private bool gameOverShown;

    public Boss_DeadState(Enemy enemy, StateMachine stateMachine, Boss boss, string animBoolName) : base(enemy, stateMachine, boss, animBoolName)
    {
        col = enemy.GetComponent<Collider2D>();
    }

    public override void Enter()
    {
        base.Enter();
        boss.SetVelocity(0, 0);

        gameOverShown = false;
        stateMachine.SwitchOffStateMachine();
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
        {
            if (!gameOverShown)
            {
                gameOverShown = true;
                UI.Instance?.ShowGameOver();
            }

            Object.Destroy(boss.gameObject);
        }
    }
}
