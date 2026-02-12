using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
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
    }

    public virtual void PerformAttack()//执行攻击
    {
        foreach(var target in GetDetectedColliders())
        {
            IDamagable damagable = target.GetComponent<IDamagable>();

            if(damagable == null) 
                continue;

            float elementalDamage = stats.GetElementalDamage(out ElementType element , .6f);
            float damage = stats.GetPhysicalDamage(out bool isCrit);


            bool targetGotHit = damagable.TakeDamage(damage, transform, elementalDamage, element);

            if(element != ElementType.None)
            {
                ApplyStatusEffect(target.transform, element);
            }

            if (targetGotHit)
            {
                vfx.UpdateOnHitColor(element);
                vfx.CreatOnHitVfx(target.transform, isCrit);
            }
        }
    }

    public void ApplyStatusEffect(Transform target, ElementType element, float scaleFactor = 1)//应用状态效果
    {
        Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();

        if(statusHandler == null) 
            return; 

        if(element == ElementType.Ice && statusHandler.CanBeApplied(ElementType.Ice))
        {
            statusHandler.ApplyChillEffect(defaultDuration, chillSlowMultiplier * scaleFactor);
        }
        if(element == ElementType.Fire && statusHandler.CanBeApplied(ElementType.Fire))
        {
            scaleFactor = fireScale;
            float fireDamage = stats.offense.fireDamage.GetValue() * scaleFactor;
            statusHandler.ApplyBurnEffect(defaultDuration, fireDamage);
        }
        if(element == ElementType.Lightning && statusHandler.CanBeApplied(ElementType.Lightning))
        {
            scaleFactor = lightningScale;
            float lightningDamage = stats.offense.lightningDamage.GetValue() * scaleFactor;
            statusHandler.ApplyElectrifyEffect(defaultDuration, lightningDamage, electrifyChargeBuildUp);
        }
    }


    protected  Collider2D[] GetDetectedColliders()//获取检测到的碰撞体(执行攻击的子程序)
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }

    private void OnDrawGizmos()//绘制检测敌人范围
    {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
