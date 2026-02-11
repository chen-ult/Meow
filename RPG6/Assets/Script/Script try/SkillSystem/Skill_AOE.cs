using UnityEngine;

public class Skill_AOE : Skill_Base 
{
    private Entity_VFX vfx;

    [Header("AOE目标探测参数")]
    [SerializeField] private Transform targetCheck_AOE;//目标检测点
    [SerializeField] private float targetCheckRadius_AOE = 1;//目标检测半径
    [SerializeField] private LayerMask whatIsTarget_AOE;//目标图层


    [Header("AOE参数")]
    [SerializeField] private float damage_AOE;
    [SerializeField] private float aoe_duration;

    protected override void Awake()
    {
        base.Awake();
        vfx = GetComponentInParent<Entity_VFX>();
    }

    public void PerformAOE()
    {
        foreach(var target in GetDetectedColliders())
        {
            IDamagable damagable = target.GetComponent<IDamagable>();
           ICanBeStunned canBeStunned = target.GetComponent<ICanBeStunned>();

            if (damagable == null)
                continue;

            

            bool targetGotHit = damagable.TakeDamage(damage_AOE, transform, 0, ElementType.None);

            if (targetGotHit)
            {
                vfx.UpdateOnHitColor(ElementType.None);
                vfx.CreatOnHitVfx(target.transform, false);
                canBeStunned.GetIntoStunned();
            }


           

        }


    }



    





    protected  Collider2D[] GetDetectedColliders()//获取检测到的碰撞体(执行攻击的子程序)
    {
        return Physics2D.OverlapCircleAll(targetCheck_AOE.position, targetCheckRadius_AOE, whatIsTarget_AOE);
    }

    private void OnDrawGizmos()//绘制检测敌人范围_AOE
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetCheck_AOE.position, targetCheckRadius_AOE);
    }

    public float GetAoeDuration() => aoe_duration;

}
