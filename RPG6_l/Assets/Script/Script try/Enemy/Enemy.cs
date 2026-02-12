using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    public Enemy_IdleState idleState;//敌人空闲状态
    public Enemy_MoveState moveState;//敌人移动状态
    public Enemy_AttackState attackState;//敌人攻击状态
    public Enemy_BattleState battleState;//敌人战斗状态
    public Enemy_DeadState deadState;//敌人死亡状态
    public Enemy_StunnedState stunnedState;//敌人眩晕状态



    [Header("战斗状态参数")]
    public float battleMoveSpeed = 3f;//战斗移动速度
    public float attackDistance = 2f;//攻击距离
    public float battleTimeDuration = 5f;//战斗时间持续时间
    public float minRetreatDistance = 1;//最小撤退距离
    public Vector2 retreatVelocity;//撤退速度

    [Header("反击参数(stunned state)")]
    public float stunnedDuration = 1;//眩晕持续时间
    public Vector2 stunnedVelocity = new Vector2(7, 7);//眩晕速度
    [SerializeField] protected bool canBeStunned;//是否可以被眩晕

    [Header("移动参数")]
    public float idleTime = 2f;//空闲时间
    public float moveSpeed = 1.4f;//移动速度
    [Range(0,2)]
    public float moveAnimSpeedMultiplier = 1f;//移动动画速度倍率

    [Header("检测玩家参数")]
    [SerializeField] private LayerMask whatIsPlayer;//玩家图层
    [SerializeField] private Transform playerCheck;//玩家检测点
    [SerializeField] private float playerCheckDistance = 10f;//玩家检测距离
    public Transform player {  get; private set; }//玩家引用


    protected override IEnumerator SlowDownEntityCo(float slowMultiplier, float duration)//减速实体协程(减速实体的子程序，这是重写)
    {
        float originalMoveSpeed = moveSpeed;
        float originalBattleMoveSpeed = battleMoveSpeed;
        float originalAnimSpeed = anim.speed;

        float speedMutiplier = 1 - slowMultiplier;

        moveSpeed *= speedMutiplier;
        battleMoveSpeed *= speedMutiplier;
        anim.speed *= speedMutiplier;

        yield return new WaitForSeconds(duration);


        moveSpeed = originalMoveSpeed;
        battleMoveSpeed = originalMoveSpeed;
        anim.speed = originalAnimSpeed;

    }

    public void EnableCounterWindow(bool enable) => canBeStunned = enable;//启用反击窗口(启用/禁用是否可以被眩晕)


    public override void EntityDeath()//实体死亡
    {
        base.EntityDeath();

        stateMachine.ChangeState(deadState);
    }

    private void HandlePlayerDeath()//玩家死亡后的行为逻辑
    {
        stateMachine.ChangeState(idleState);
    }

    public virtual void TryEnterBattleState(Transform player)//尝试进入战斗状态
    {
        if (stateMachine.currentState == battleState || stateMachine.currentState == attackState)
            return;

        this.player = player;
        
    }

    public Transform GetPlayerReference()//获取玩家引用
    {
        if (player == null)
            player = PlayerDetected().transform;

        return player;
    }

    public RaycastHit2D PlayerDetected()//检测玩家
    {
        RaycastHit2D hit =
            Physics2D.Raycast(playerCheck.position, Vector2.right * facingDir, playerCheckDistance, whatIsPlayer | whatIsGround);

        if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
            return default;

        return hit;
    }

    protected override void OnDrawGizmos()//绘制玩家检测线和攻击距离线
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDir * playerCheckDistance), playerCheck.position.y));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDir * attackDistance), playerCheck.position.y));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDir * minRetreatDistance), playerCheck.position.y));
        

    }

    private void OnEnable()//订阅玩家死亡事件
    {
        Player.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()//取消订阅玩家死亡事件
    {
        Player.OnPlayerDeath -= HandlePlayerDeath;
    }
}
