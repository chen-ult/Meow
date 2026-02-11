using UnityEngine;

public class Boss_BattleState : BossState
{
    private Transform player;
    
    
    public Boss_BattleState(Enemy enemy, StateMachine stateMachine, Boss boss, string animBoolName) : base(enemy, stateMachine, boss, animBoolName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();

        if (boss.hasEnteredPhase2 && !WithinAttackRange())
            boss.SummonFirestorm();

        player ??= enemy.GetPlayerReference();

        if (ShouldRetreat())
        {
            rb.linearVelocity = new Vector2(enemy.retreatVelocity.x * -DirectionToPlayer(), enemy.retreatVelocity.y);
            enemy.HandleFlip(DirectionToPlayer());
        }

    }

    public override void Update()
    {
        base.Update();



        if (player == null)
        {
            stateMachine.ChangeState(boss.boss_idleState);
            return;
        }


        if (WithinAttackRange() && enemy.PlayerDetected())
        {
            stateMachine.ChangeState(boss.boss_attackState);

            return;
        }
        else if (player.position.y < enemy.transform.position.y + 5)
        { 
            enemy.SetVelocity(enemy.battleMoveSpeed * DirectionToPlayer(), rb.linearVelocity.y);
        }

    }

    

    private bool WithinAttackRange() => DistanceToPlayer() < enemy.attackDistance;

    private bool ShouldRetreat() => DistanceToPlayer() < enemy.minRetreatDistance;

    private float DistanceToPlayer()
    {
        if (player == null)
            return float.MaxValue;

        return Mathf.Abs(player.position.x - enemy.transform.position.x);
    }

    private int DirectionToPlayer()
    {
        if (player == null)
            return 0;

        return player.position.x > enemy.transform.position.x ? 1 : -1;
    }

}
