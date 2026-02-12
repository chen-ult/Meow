using System.Collections;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{

    protected SpriteRenderer sr;
    private Entity entity;

    [Header("被伤害视觉效果 VFX")]
    [SerializeField] private Material onDamageMaterial;//受伤材质
    [SerializeField] private float onDamageVfxDuration = .2f;//受伤视觉效果持续时间
    private Material originalMaterial;//原始材质
    private Coroutine onDamageVfxCoroutine;//受伤视觉效果协程

    [Header("攻击视觉特效")]
    [SerializeField] private Color hitVfxColor = Color.white;//命中视觉特效颜色
    [SerializeField] private GameObject hitVfx;//命中视觉特效预制体
    [SerializeField] private GameObject crithitVfx;//暴击命中视觉特效预制体


    [Header("元素命中视觉特效")]
    [SerializeField] private Color chillVfx = Color.cyan;//冰冻视觉特效颜色
    [SerializeField] private Color burnVfx = Color.red;//燃烧视觉特效颜色
    [SerializeField] private Color electrifyVfx = Color.yellow;//电击视觉特效颜色
    private Color originalHitVfxColor;//原始命中视觉特效颜色
    private Coroutine statusVfxCo;//状态视觉特效协程


    private void Awake()
    {
        entity = GetComponent<Entity>();
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material;
        originalHitVfxColor = hitVfxColor;
    }

    public void PlayOnStatusVfx(float duration, ElementType element)//播放状态视觉特效
    {
        if(element == ElementType.Ice)
            StartCoroutine(PlayStatusVfxCo(duration, chillVfx));

        if(element == ElementType.Fire)
            StartCoroutine(PlayStatusVfxCo(duration, burnVfx));

        if(element == ElementType.Lightning)
            StartCoroutine(PlayStatusVfxCo(duration, electrifyVfx));

    }

    public void StopAllVfxx()//停止所有视觉特效
    {
        StopAllCoroutines();
        sr.color = Color.white;
        sr.material = originalMaterial;
    }

    private IEnumerator PlayStatusVfxCo(float duration, Color effectcColor)//播放状态视觉特效协程(播放状态视觉特效的执行子程序)
    {
        float tickInterval = .25f;
        float timeHasPassed = 0;

        Color lightColor = effectcColor * 1.2f;
        Color darkColor = effectcColor * 0.8f;

        bool taggle = false;

        while(timeHasPassed < duration)
        {
            sr.color = taggle ? darkColor : lightColor;
            taggle = !taggle;

            yield return new WaitForSeconds(tickInterval);
            timeHasPassed += tickInterval;
        }

        sr.color = Color.white;

    }

    public void UpdateOnHitColor(ElementType element)//更新命中视觉特效颜色
    {
        if(element == ElementType.Ice)
            hitVfxColor = chillVfx;
        

        if(element == ElementType.None)
            hitVfxColor = originalHitVfxColor;
    }

    public void CreatOnHitVfx(Transform target, bool isCrit)//创建命中视觉特效
    {
        GameObject hitPrefab = isCrit ? crithitVfx : hitVfx;
        if (hitPrefab == null)
        {
            Debug.LogWarning("Entity_VFX.CreatOnHitVfx: hit prefab is null, cannot spawn VFX.", this);
            return;
        }

        Vector3 spawnPos = (target != null) ? target.position : transform.position;
        GameObject vfxInstance = Instantiate(hitPrefab, spawnPos, Quaternion.identity);
        if (vfxInstance == null)
        {
            Debug.LogWarning("Entity_VFX.CreatOnHitVfx: failed to instantiate hit VFX.", this);
            return;
        }

        var sprite = vfxInstance.GetComponentInChildren<SpriteRenderer>();
        if (sprite != null)
            sprite.color = hitVfxColor;
        else
            Debug.LogWarning("Entity_VFX.CreatOnHitVfx: spawned VFX has no SpriteRenderer to tint.", vfxInstance);

        if (entity != null && entity.facingDir == -1 && isCrit)
            vfxInstance.transform.Rotate(0, 180, 0);
    }

    public void PlayOnDamageVfx()//播放受伤视觉效果
    {
        if(onDamageVfxCoroutine != null)
            StopCoroutine(onDamageVfxCoroutine);

        onDamageVfxCoroutine = StartCoroutine(OnDamageVfxCo());
    }

    private IEnumerator OnDamageVfxCo()//受伤视觉效果协程(播放受伤视觉效果的执行子程序)
    {
        sr.material = onDamageMaterial;

        yield return new WaitForSeconds(onDamageVfxDuration);

        sr.material = originalMaterial;
    }
}
