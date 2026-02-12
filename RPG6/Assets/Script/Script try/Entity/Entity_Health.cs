using System;
using UnityEngine.UI;
using UnityEngine;

public class Entity_Health : MonoBehaviour , IDamagable
{
    // Invoked when this entity is about to take damage (before knockback/health reduction)
    public event Action OnTakingDamage;
    public event Action OnHealthUpdate;

    public Slider healthBar;
    private Entity_VFX entityVfx;
    private Entity entity;
    protected Entity_Stats entityStats;

    

    [SerializeField] protected float currentHealth;//当前生命值
    [SerializeField] protected bool isDead;//是否死亡
    [Header("生命再生")]
    [SerializeField] private float regenInterval = 1;//再生间隔
    [SerializeField] protected bool canRegenerateHealth = true;//是否可以再生生命

    [Header("死亡设置")]
    [SerializeField] private float destroyDelay = 3f; // 死亡后销毁延迟（秒）

    public float lastDamageTake { get; private set; }
    protected bool canTakeDamage = true;

    [Header("攻击击退值")]
    [SerializeField] private float knockbackDuration = .2f;//击退时间
    [SerializeField] private Vector2 knockbackPower = new Vector2(1.5f, 2.5f);//击退力
    [Header("重攻击击退值")]
    [Range(0f, 1f)]
    [SerializeField] private float heavyDamageThreshold = .3f;//重击阈值
    [SerializeField] private float heavyKnockbackDuration = .5f;//重击击退时间
    [SerializeField] private Vector2 heavyKnockbackPower = new Vector2(7, 7);//重击击退力

    protected virtual void Awake()
    {
        entityStats = GetComponent<Entity_Stats>();
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();


        SetUpHealth();
    }

    private void SetUpHealth()
    {

        if (entityStats == null)
        {
            Debug.LogError($"Entity_Stats component not found on {gameObject.name}");
            return;
        }


        currentHealth = entityStats.GetMaxHealth();
        OnHealthUpdate += UpdateHealthBar;

        UpdateHealthBar();
        InvokeRepeating(nameof(RegenerateHealth), 0, regenInterval);
    }

    public virtual bool TakeDamage(float damage, Transform damageDealer, float elementalDamage, ElementType element)   //造成伤害
    {
        if (isDead || !canTakeDamage) 
            return false;

        // Respect invulnerability flag on the entity (e.g. player dash)
        if (entity != null && entity.IsInvulnerable)
            return false;

        if (AttackEvaded())
        {
            Debug.Log($"{gameObject.name} 回避了攻击!");       
            return false;
        }

        Entity_Stats attackerStats = damageDealer.GetComponent<Entity_Stats>();
        float armorReduction = attackerStats != null ? attackerStats.GetArmorReduction() : 0f;

        float mitigation = entityStats != null ? entityStats.GetArmorMitigation(armorReduction) : 0;
        float resistance = entityStats != null ? entityStats.GetElementalResistance(element) : 0;

        float physicalDamageTaken = damage * (1 - mitigation);
        float elementalDamageTaken = elementalDamage * (1 - resistance);


        TakeKnockBack(damageDealer, physicalDamageTaken);

        ReduceHealth(physicalDamageTaken + elementalDamageTaken);

        lastDamageTake = physicalDamageTaken + elementalDamageTaken;
        OnTakingDamage?.Invoke();

        return true;
    }    

    public void SetCanTakeDamage(bool state)
    {
        canTakeDamage = state;
    }


    private bool AttackEvaded() => UnityEngine.Random.Range(0, 100) < entityStats.GetEvasion();   //回避率


    protected virtual void RegenerateHealth()//回复生命
    {
        if (!canRegenerateHealth)
            return;

        float regenAmount = entityStats.resource.healthRegen.GetValue();
        IncreaseHealth(regenAmount);
    }      

    public void IncreaseHealth(float healAmount)//增加生命（回复生命的主要执行子程序）
    {
        if (isDead)
            return;

        float newHealth = currentHealth + healAmount;
        float maxHealth = entityStats.GetMaxHealth();


        currentHealth = Mathf.Min(newHealth, maxHealth);
        OnHealthUpdate?.Invoke();
    }


    public virtual void ReduceHealth(float damage)//减少生命（受伤的执行子程序）
    {
        currentHealth -= damage;

        
        entityVfx?.PlayOnDamageVfx();
        OnHealthUpdate?.Invoke();

        if (currentHealth <= 0)
            Die();
    }

    protected virtual void Die()//死亡
    {
        isDead = true;
        entity?.EntityDeath();
        CancelInvoke(nameof(RegenerateHealth));
        // drop items if a DropOnDeath component is attached
        var dropper = GetComponent<DropOnDeath>() ?? GetComponentInChildren<DropOnDeath>();
        dropper?.DropItems();

        // destroy after delay if this is an enemy (players should not be destroyed)
        var enemy = GetComponent<Enemy>();
        if (enemy != null)
        {
            Destroy(enemy.gameObject, destroyDelay);
        }
    }

    public float GetHealthPercent() => currentHealth / entityStats.GetMaxHealth();//获取生命百分比

    public void SetHealthToPercent(float percent)//设置生命百分比
    {
        percent = Mathf.Clamp01(percent);
        currentHealth = entityStats.GetMaxHealth() * percent;
        OnHealthUpdate?.Invoke();
    }

    public void IncreaseMaxHealth(float amout)
    {
        float newMaxHealth = entityStats.GetMaxHealth() + amout;
        entityStats.resource.maxHealth.SetBaseValue(newMaxHealth);
        IncreaseHealth(amout);
        OnHealthUpdate?.Invoke();

    }

    public float GetCurrentHealth() => currentHealth;//获取当前生命值

    protected void UpdateHealthBar()//更新血条
    {
        if (healthBar == null)
            return;

        healthBar.value = currentHealth / entityStats.GetMaxHealth();
    }
    

    private void TakeKnockBack(Transform damageDealer, float finalDamage)//击退(受伤的执行子程序)
    {
        float duration = CalculateDuration(finalDamage);
        Vector2 knockback = CalculateKnockback(finalDamage, damageDealer);

        entity?.ReciveKnockback(knockback, duration);
    }
    private Vector2 CalculateKnockback(float damage, Transform damageDealer)//计算击退力(击退的执行子程序)
    {
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;

        Vector2 knockback = IsHeavyDamage(damage) ? heavyKnockbackPower : knockbackPower;

        knockback.x *= direction;
        return knockback;
    }

    private float CalculateDuration(float damage) => IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;//计算击退时间(击退的执行子程序)


    private bool IsHeavyDamage(float damage) => damage / entityStats.GetMaxHealth() > heavyDamageThreshold;//是否为重击

    public void SetCanRegenerateHealth(bool state)
    {
        canRegenerateHealth = state;
    }

    public void Revive()
    {
        isDead = false;
        canTakeDamage = true;
        if (!IsInvoking(nameof(RegenerateHealth)))
            InvokeRepeating(nameof(RegenerateHealth), 0, regenInterval);
    }


}
