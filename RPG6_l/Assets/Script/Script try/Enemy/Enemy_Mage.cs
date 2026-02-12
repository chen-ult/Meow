using UnityEngine;

public class Enemy_Mage : Enemy, ICanBeStunned
{
    [Header("发射参数")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    [Space]
    public float attackCooldown = 2f;
    private float attackTimer;
    [Header("传送参数")]
    public float teleportDistance = 5f; // 传送水平距离
    public float teleportCooldown = 5f; // 传送冷却时间
    public float teleportDuration = 0.15f; // 传送动作时长（停留时间）
    private float teleportTimer = 0f;
    [Tooltip("当玩家靠近到该距离时，法师会尝试传送（需满足 CanTeleport()）")]
    public float teleportTriggerDistance = 2f;


    public Enemy_Ranged_IdleState ranged_IdleState;
    public Enemy_Ranged_MoveState ranged_MoveState;
    public Enemy_Ranged_AttackState ranged_AttackState;
    public Enemy_Ranged_DeadState ranged_DeadState;
    public Enemy_Ranged_StunnedState ranged_stunnedState;
    public Enemy_Ranged_BattleState ranged_BattleState;
    public Enemy_Ranged_TeleportState ranged_TeleportState;

    protected override void Awake()
    {
        base.Awake();

        attackTimer = attackCooldown;

        ranged_IdleState = new Enemy_Ranged_IdleState(this, stateMachine, this, "idle");
        ranged_MoveState = new Enemy_Ranged_MoveState(this, stateMachine, this, "move");
        ranged_AttackState = new Enemy_Ranged_AttackState(this, stateMachine, this, "attack");
        ranged_DeadState = new Enemy_Ranged_DeadState(this, stateMachine, this, "dead");
        ranged_stunnedState = new Enemy_Ranged_StunnedState(this, stateMachine, this, "stunned");
        ranged_BattleState = new Enemy_Ranged_BattleState(this, stateMachine, this, "battle");
        ranged_TeleportState = new Enemy_Ranged_TeleportState(this, stateMachine, this, "teleport");

        // ranged-specific states are stored separately; Mage will use them directly when changing state
    }

    protected override void Start()
    {
        base.Start();

        
        stateMachine.Initialize(ranged_IdleState);
    }

    protected override void Update()
    {
        base.Update();

        attackTimer += Time.deltaTime;
        teleportTimer += Time.deltaTime;
    }

    public void FireBullet()
    {
        if (bulletPrefab == null || firePoint == null)
            return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        // set bullet direction to match mage facing direction if bullet supports it
        // Bullet component may be on the root or a child of the prefab, so search both
        var bl = bullet.GetComponent<Bullet>() ?? bullet.GetComponentInChildren<Bullet>();
        if (bl != null)
            bl.SetMoveDirection(facingDir);
        // also ensure visual/transform faces the same direction
        bullet.transform.right = new Vector3(facingDir, 0f, 0f);
        attackTimer = 0;
    }

    public bool CanTeleport()
    {
        return teleportTimer >= teleportCooldown;
    }

    public void UseTeleport()
    {
        teleportTimer = 0f;
    }

    public bool CanAttack()
    {
        if(attackTimer >= attackCooldown)
            return true;
        else
            return false;
    }

    public override void TryEnterBattleState(Transform player)
    {
        base.TryEnterBattleState(player);

        stateMachine.ChangeState(ranged_BattleState);
    }

    public void GetIntoStunned()
    {
        stateMachine.ChangeState(ranged_stunnedState);
    }

    public override void EntityDeath()
    {
        // Ensure mage uses its ranged dead state instead of trying to use base.deadState
        if (ranged_DeadState != null)
            stateMachine.ChangeState(ranged_DeadState);
        else
            base.EntityDeath();
    }
}
