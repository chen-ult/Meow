using System.Collections;
using UnityEngine;

public class Entity_StatusHandler : MonoBehaviour
{
    private Entity entity;
    private Entity_VFX entityVfx;
    private Entity_Stats entityStats;
    private Entity_Health entityHealth;
    private ElementType currentEffect = ElementType.None;

    [Header("闪电效果参数")]
    [SerializeField] private GameObject lightningStrikeVfx;//闪电特效
    [SerializeField] private float currentCharge;//当前电荷值
    [SerializeField] private float maxCharge = 1;//最大电荷值
    private Coroutine electrifyCo;//闪电效果协程


    private void Awake()
    {
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
        entityStats = GetComponent<Entity_Stats>();
        entityHealth = GetComponent<Entity_Health>();
    }


    public void ApplyElectrifyEffect(float duration, float damage, float charge)//应用闪电效果(相当于总管理)
    {
        float lightningResistance = entityStats.GetElementalResistance(ElementType.Lightning);
        float finalCharge = charge*(1- lightningResistance);

        currentCharge += finalCharge;

        if(currentCharge >= maxCharge)
        {
            DoLightningStrike(damage);
            StopElectrifyEffect();
            return;
        }

        if(electrifyCo != null)
            StopCoroutine(electrifyCo);

        electrifyCo = StartCoroutine(ElectrifyEffectCo(duration));
    }

    private void StopElectrifyEffect()//停止闪电效果(应用闪电效果的执行子程序)
    {
        currentEffect = ElementType.None;
        currentCharge = 0;
        entityVfx.StopAllVfxx();
    }

    private void DoLightningStrike(float damage)//执行闪电打击(应用闪电效果的执行子程序)
    {
        Instantiate(lightningStrikeVfx, transform.position, Quaternion.identity);
        entityHealth.ReduceHealth(damage);
    }

    private IEnumerator ElectrifyEffectCo(float duration)//闪电效果协程(应用闪电效果的执行子程序)
    {
        currentEffect = ElementType.Lightning;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Lightning);
        yield return new WaitForSeconds(duration);
        StopElectrifyEffect();
    }




    public void ApplyBurnEffect(float duration, float fireDamage)//应用燃烧效果(相当于总管理)
    {
        float fireResistance = entityStats.GetElementalResistance(ElementType.Fire);
        float finalDamage = fireDamage * (1- fireResistance);
        StartCoroutine(BurnEffectCo(duration, finalDamage));
    }

    private IEnumerator BurnEffectCo(float duration,float totalDamage)//燃烧效果协程(应用燃烧效果的执行子程序)
    {
        currentEffect = ElementType.Fire;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Fire);

        int ticksPerSecond = 2;
        int tickCount = Mathf.RoundToInt(ticksPerSecond * duration);

        if (tickCount <= 0)
        {
            // duration too short, apply all damage instantly
            entityHealth.ReduceHealth(totalDamage);
            currentEffect = ElementType.None;
            yield break;
        }

        float damagePerTick = totalDamage / tickCount;
        // keep a fixed number of ticks per second so total duration ~= duration
        float tickInterval = 1f / ticksPerSecond;

        for (int i = 0; i < tickCount; i++)
        {
            entityHealth.ReduceHealth(damagePerTick);
            yield return new WaitForSeconds(tickInterval);
        }

        currentEffect = ElementType.None;

    }




    public void ApplyChillEffect(float duration, float slowMultiplier)//应用冰冻效果(相当于总管理)
    {
        float iceResistance = entityStats.GetElementalResistance(ElementType.Ice);
        float finalDuration = duration * (1 - iceResistance);

        StartCoroutine(ChillEffectCo(duration, slowMultiplier));
    }


    private IEnumerator ChillEffectCo(float duration, float slowMutiplier)//冰冻效果协程(应用冰冻效果的执行子程序)
    {
        entity.SlowDownEntity(slowMutiplier, duration);
        currentEffect = ElementType.Ice;
        entityVfx.PlayOnStatusVfx(duration ,ElementType.Ice);

        yield return new WaitForSeconds(duration);

        currentEffect = ElementType.None;
    }





    public bool CanBeApplied(ElementType element)//检查状态效果是否可以被应用
    {
        if(element == ElementType.Lightning && currentEffect == ElementType.Lightning)
            return true;

        return currentEffect == ElementType.None;
    }
}
