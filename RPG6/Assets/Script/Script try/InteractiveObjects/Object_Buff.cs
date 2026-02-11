using System.Collections;
using UnityEngine;




public class Object_Buff : MonoBehaviour
{
    private SpriteRenderer sr;//精灵渲染器
    private Entity_Stats statsToModify;//要修改的实体属性


    [Header("Buff参数")]
    [SerializeField] private Buff[] buffs;//属性数组
    [SerializeField] private string buffName;//属性名称
    [SerializeField] private float buffDuration = 4;//属性持续时间
    [SerializeField] private bool canBeUsed = true;//是否可以使用

    [Header("动态参数")]
    [SerializeField] private float floatSpeed = 1f;//浮动速度
    [SerializeField] private float floatRange = .1f;//浮动范围
    private Vector3 startPosition;//起始位置

    private void Awake()
    {
        startPosition = transform.position;
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
        transform.position = startPosition + new Vector3(0, yOffset, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)//触发碰撞
    {
        if (!canBeUsed)
            return;

        statsToModify  = collision.GetComponent<Entity_Stats>();
        StartCoroutine(BuffCo(buffDuration));
    }


    private IEnumerator BuffCo(float duration)//Buff协程
    {
        canBeUsed = false;
        sr.color = Color.clear;

        ApplyBuff(true);

        yield return new WaitForSeconds(duration);

        ApplyBuff(false);

        Destroy(gameObject);
    }

    private void ApplyBuff(bool apply)//应用Buff
    {
        
        foreach (var buff in buffs)
        {
            if(apply)
                statsToModify.GetStatsByType(buff.type).AddModifier(buff.value, buffName);
            else
                statsToModify.GetStatsByType(buff.type).RemoveModifier(buffName);
        }

        
    }
}
