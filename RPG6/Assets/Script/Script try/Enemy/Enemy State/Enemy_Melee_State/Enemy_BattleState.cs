using Unity.VisualScripting;
using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    private Transform player;
    private float lastTimeWasInBattle;
    private Entity_Health health;

    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        health = enemy.GetComponent<Entity_Health>();
        if (health != null)
        {
            health.OnTakingDamage += OnEnemyDamaged;
            // do not change canRegenerateHealth here; Enemy_Health's damage timer will control regen
        }

        UpdateBattleTimer();
        player ??= enemy.GetPlayerReference();

        if (ShouldRetreat())
        {
            rb.linearVelocity = new Vector2(enemy.retreatVelocity.x * -DirectionToPlayer(), enemy.retreatVelocity.y);
            enemy.HandleFlip(DirectionToPlayer());
        }
        
    }

    public override void Exit()
    {
        base.Exit();
        if (health != null)
        {
            health.OnTakingDamage -= OnEnemyDamaged;
            health = null;
        }
    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerDetected())
            UpdateBattleTimer();

        // timer is driven by damage events (player attacks). If timer expires, switch out of battle.
        if (BattleTimeIsOver())
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        if (WithinAttackRange() && enemy.PlayerDetected())
            stateMachine.ChangeState(enemy.attackState);
        else if (player != null && player.position.y < enemy.transform.position.y + 5)
            enemy.SetVelocity(enemy.battleMoveSpeed * DirectionToPlayer(), rb.linearVelocity.y);

    }

    private void OnEnemyDamaged()
    {
        UpdateBattleTimer();
    }

    private void UpdateBattleTimer() => lastTimeWasInBattle = Time.time;

    private bool BattleTimeIsOver() => Time.time > lastTimeWasInBattle + enemy.battleTimeDuration;

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
