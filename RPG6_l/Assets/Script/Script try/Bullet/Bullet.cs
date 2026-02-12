using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("子弹参数")]
    public float speed = 8f;
    public float destroyTime = 3f;

    [Header("生成动画")]
    public bool hasSpawnAnim = true;
    public float spawnAnimDuration = 0.2f; // 出现动画时长

    [Header("销毁动画")]
    public bool hasHitAnim = true;
    public float hitAnimDuration = 0.3f;  // 消失动画时长
    private bool isDying = false; // 防止重复销毁

    private Vector2 moveDir;
    private Animator anim;
    private bool isFlying = false;


    private Entity_VFX vfx;
    private Entity_Stats stats;

    [Header("目标探测参数")]
    [SerializeField] private Transform targetCheck;//目标检测点
    [SerializeField] private float targetCheckRadius = 1;//目标检测半径
    [SerializeField] private LayerMask whatIsTarget;//目标图层

    [Header("状态效果参数")]
    [SerializeField] private float defaultDuration = 3f;//状态效果默认持续时间
    [SerializeField] private float chillSlowMultiplier = 0.2f;//冰冻减速倍率
    [SerializeField] private float electrifyChargeBuildUp = .4f;//闪电效果充能值
    [Space]
    [SerializeField] private float fireScale = .8f;//火焰效果倍率
    [SerializeField] private float lightningScale = 2.5f;//闪电效果倍率

    private void Awake()
    {
        vfx = GetComponent<Entity_VFX>();
        stats = GetComponent<Entity_Stats>();
        anim = GetComponentInChildren<Animator>();
    }

    // 可由生成者调用以设置发射方向（使用 Vector2 方向）
    public void SetMoveDirection(Vector2 dir)
    {
        if (dir == Vector2.zero) return;
        moveDir = dir.normalized;
    }

    // 可由生成者调用以使用朝向（-1 或 1）设置方向
    public void SetMoveDirection(int facingDir)
    {
        SetMoveDirection(new Vector2(facingDir, 0));
    }

    // 播放命中特效（供动画事件调用，不执行伤害）
    public void PlayHitVfxFallback()
    {
        if (vfx == null) return;
        // 使用默认参数：无元素、非暴击
        vfx.UpdateOnHitColor(ElementType.None);
        vfx.CreatOnHitVfx(transform, false);
    }

    void Start()
    {
        // if moveDir wasn't set externally (e.g. by spawner), use transform.right
        if (moveDir == Vector2.zero)
            moveDir = transform.right; // 2D用right，3D用forward

        // 先播生成动画，期间不移动
        if (hasSpawnAnim && anim != null)
        {
            anim.SetTrigger("Spawn");
            Invoke(nameof(StartFly), spawnAnimDuration);
        }
        else
        {
            StartFly(); // 没动画直接飞
        }

        // 超时销毁（走销毁动画）
        Invoke(nameof(PlayDestroyAnim), destroyTime);
    }

    void StartFly()
    {
        isDying = false;
        isFlying = true;
    }

    void Update()
    {
        // 仅在已开始飞行且未销毁期间移动
        if (isDying || !isFlying)
            return;

        // move in world space so direction vector is interpreted consistently
        transform.Translate(moveDir * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDying) return;
        // 如果碰到玩家并且玩家处于无敌状态（比如 Dash），则不触发命中与销毁
        if (other.CompareTag("Player"))
        {
            var playerEntity = other.GetComponentInParent<Entity>();
            if (playerEntity != null && playerEntity.IsInvulnerable)
                return;
        }

        // 触碰到目标或地面时执行攻击和销毁流程
        if (other.CompareTag("Player") || other.CompareTag("Ground") || ((1 << other.gameObject.layer) & whatIsTarget.value) != 0)
        {
            // 对碰到的 other 进行伤害
            PerformAttack(other);
            PlayDestroyAnim();
        }
    }

    public void PlayDestroyAnim()
    {
        if (isDying) return;
        isDying = true;

        // 取消自动销毁调用，停止移动并禁用碰撞
        CancelInvoke(nameof(PlayDestroyAnim));
        moveDir = Vector2.zero;
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (hasHitAnim && anim != null)
        {
            anim.SetTrigger("Hit");
            // 动画播完再真正销毁
            Invoke(nameof(DestroyBullet), hitAnimDuration);
        }
        else
        {
            DestroyBullet();
        }
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }



    // 对单个碰撞体执行伤害（更高效）
    public virtual void PerformAttack(Collider2D targetCollider)
    {
        if (targetCollider == null) return;

        IDamagable damagable = targetCollider.GetComponent<IDamagable>() ?? targetCollider.GetComponentInParent<IDamagable>();
        if (damagable == null)
            return;

        float elementalDamage = stats.GetElementalDamage(out ElementType element, .6f);
        float damage = stats.GetPhysicalDamage(out bool isCrit);

        bool targetGotHit = damagable.TakeDamage(damage, transform, elementalDamage, element);

        if (element != ElementType.None)
        {
            ApplyStatusEffect(targetCollider.transform, element);
        }

        if (targetGotHit)
        {
            vfx.UpdateOnHitColor(element);
            vfx.CreatOnHitVfx(targetCollider.transform, isCrit);
        }
    }

    public void ApplyStatusEffect(Transform target, ElementType element, float scaleFactor = 1)//应用状态效果
    {
        Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();

        if (statusHandler == null)
            return;

        if (element == ElementType.Ice && statusHandler.CanBeApplied(ElementType.Ice))
        {
            statusHandler.ApplyChillEffect(defaultDuration, chillSlowMultiplier * scaleFactor);
        }
        if (element == ElementType.Fire && statusHandler.CanBeApplied(ElementType.Fire))
        {
            scaleFactor = fireScale;
            float fireDamage = stats.offense.fireDamage.GetValue() * scaleFactor;
            statusHandler.ApplyBurnEffect(defaultDuration, fireDamage);
        }
        if (element == ElementType.Lightning && statusHandler.CanBeApplied(ElementType.Lightning))
        {
            scaleFactor = lightningScale;
            float lightningDamage = stats.offense.lightningDamage.GetValue() * scaleFactor;
            statusHandler.ApplyElectrifyEffect(defaultDuration, lightningDamage, electrifyChargeBuildUp);
        }
    }


    protected Collider2D[] GetDetectedColliders()//获取检测到的碰撞体(执行攻击的子程序)
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }

    private void OnDrawGizmos()//绘制检测敌人范围
    {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
