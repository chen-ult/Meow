using UnityEngine;

public class Enemy_Ranged_BattleState : EnemyState
{
    private Transform player;
    private float lastTimeWasInBattle;
    private Enemy_Mage mage;
    private float playerSearchTimer = 0f;
    private const float playerSearchInterval = 1f; // only attempt FindWithTag this often when player reference missing
    public Enemy_Ranged_BattleState(Enemy enemy, StateMachine stateMachine,Enemy_Mage mage, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        this.mage = mage;
    }

    public override void Enter()
    {
        base.Enter();

        UpdateBattleTimer();
        player ??= enemy.GetPlayerReference();

        if (ShouldRetreat())
        {
            stateMachine.ChangeState(mage.ranged_TeleportState);
            enemy.HandleFlip(DirectionToPlayer());
        }

    }

    public override void Update()
    {
        base.Update();

        // ensure we have a player reference to chase (try cached enemy.player first)
        if (player == null)
        {
            // only try to FindWithTag occasionally to avoid per-frame overhead
            player = enemy.player;
            if (player == null)
            {
                playerSearchTimer -= Time.deltaTime;
                if (playerSearchTimer <= 0f)
                {
                    var found = GameObject.FindWithTag("Player");
                    player = found != null ? found.transform : null;
                    playerSearchTimer = playerSearchInterval;
                }
            }
        }

        bool playerCurrentlyDetected = enemy.PlayerDetected();
        if (playerCurrentlyDetected)
            UpdateBattleTimer();

        // Always face the cached player if we have one so mage can react when player moves around it
        if (player != null)
            enemy.HandleFlip(DirectionToPlayer());

        if (BattleTimeIsOver())
        {
            stateMachine.ChangeState(mage.ranged_IdleState);
        }

        // Only enter attack state when within range, player detected and cooldown ready
        if (WithinAttackRange() && enemy.PlayerDetected() && mage.CanAttack())
        {
            stateMachine.ChangeState(mage.ranged_AttackState);
            return;
        }

        // Remain in battle state and reposition/approach if needed while waiting for cooldown
        // Do not move when already within attack range
        if (player != null && player.position.y < enemy.transform.position.y + 5 && !WithinAttackRange())
        {
            // ensure facing direction matches movement intent before applying velocity
            enemy.HandleFlip(DirectionToPlayer());
            enemy.SetVelocity(enemy.battleMoveSpeed * DirectionToPlayer(), rb.linearVelocity.y);
        }

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
