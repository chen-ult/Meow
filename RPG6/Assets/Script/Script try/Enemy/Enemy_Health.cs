using UnityEditor;
using UnityEngine;

public class Enemy_Health : Entity_Health
{
    // prefer parent lookup in case Health is placed on a child object of the enemy prefab
    Enemy enemy => GetComponent<Enemy>() ?? GetComponentInParent<Enemy>();

    [Header("敌人脱战回血专属设置")]
    [SerializeField] private float outOfCombatTime = 3f; // 敌人受击后，多久没挨打开始回血（秒，面板可调节）
    public bool enableOutOfCombatRegen = true; // 是否开启敌人脱战回血（可单独关闭）

    private float outOfCombatTimer; // 敌人专属脱战计时器（记录最后一次受击到现在的时间）
    private bool outOfCombatRegenTriggered = false;

    protected override void Awake()
    {
        base.Awake(); 
        // start as eligible for regen by default
        outOfCombatTimer = outOfCombatTime;
        outOfCombatRegenTriggered = false;
    }

    private void Update()
    {
        // stop counting in these terminal states
        if (isDead || !enableOutOfCombatRegen || currentHealth >= entityStats.GetMaxHealth())
            return;

        // accumulate timer regardless of canRegenerateHealth so that damage-based timer
        // controls regen independently of other systems (e.g. AI disabling canRegenerateHealth)
        outOfCombatTimer += Time.deltaTime;
        outOfCombatTimer = Mathf.Min(outOfCombatTimer, outOfCombatTime);

        // trigger immediate first tick when timer reaches threshold
        if (!outOfCombatRegenTriggered && outOfCombatTimer >= outOfCombatTime)
        {
            outOfCombatRegenTriggered = true;
            RegenerateHealth();
        }
    }

    public override bool TakeDamage(float damage, Transform damageDealer, float elementalDamage, ElementType element)//造成伤害(敌人版的重写，主要关于敌人AI)
    {
        bool wasHit = base.TakeDamage(damage, damageDealer, elementalDamage, element);

        if (!wasHit)
            return false;

        if (damageDealer.GetComponent<Player>() != null)
            enemy.TryEnterBattleState(damageDealer);

        return true;
    }

    public override void ReduceHealth(float damage)
    {
        base.ReduceHealth(damage);

        // reset out-of-combat timer on any health reduction
        outOfCombatTimer = 0f;
        outOfCombatRegenTriggered = false;
    }

    protected override void RegenerateHealth()
    {
        // only allow enemy out-of-combat regen when timer reached and feature enabled
        if (!enableOutOfCombatRegen || outOfCombatTimer < outOfCombatTime)
            return;

        // perform actual regen regardless of Entity_Health.canRegenerateHealth state
        float regenAmount = entityStats.resource.healthRegen.GetValue();
        IncreaseHealth(regenAmount);
    }
}
