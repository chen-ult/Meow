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
        outOfCombatTimer = outOfCombatTime;
        outOfCombatRegenTriggered = false;
    }

    private void Update()
    {
        // 不满足条件则停止计时：死亡/全局禁止回血/关闭脱战回血/血量已满
        if (isDead || !canRegenerateHealth || !enableOutOfCombatRegen || currentHealth >= entityStats.GetMaxHealth())
            return;

        // 累加计时器，限制最大值（避免数值过大）
        outOfCombatTimer += Time.deltaTime;
        outOfCombatTimer = Mathf.Min(outOfCombatTimer, outOfCombatTime);

        // When timer reaches the threshold, trigger an immediate regen (once) so we don't wait
        // for the next InvokeRepeating tick. Reset in TakeDamage.
        if (!outOfCombatRegenTriggered && outOfCombatTimer >= outOfCombatTime)
        {
            outOfCombatRegenTriggered = true;
            // call RegenerateHealth directly to attempt immediate regen
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

        outOfCombatTimer = 0;
        outOfCombatRegenTriggered = false;
        
    
    }


    protected override void RegenerateHealth()
    {
        // 敌人回血额外条件：开启脱战回血 + 计时器达到脱战时间
        if (!enableOutOfCombatRegen || outOfCombatTimer < outOfCombatTime)
            return;

        // 满足所有条件，执行父类的基础回血逻辑
        base.RegenerateHealth();
    }
}
