using UnityEngine;

[CreateAssetMenu(menuName = "游戏数据/属性/属性配置" , fileName = "默认属性设置")]
public class Stat_SetupSO : ScriptableObject
{
    [Header("资源")]
    public float maxHealth = 100f;//最大生命值
    public float healthRegen;//生命回复

    [Header("物理攻击")]
    public float attackSpeed = 1;//攻击速度
    public float damage = 10;//伤害
    public float critChance;//暴击率
    public float critpower = 150;//暴击伤害
    public float armorReduction;//护甲穿透

    [Header("法术攻击")]
    public float fireDamage;//火焰伤害
    public float iceDamage;//冰霜伤害
    public float lightningDamage;//闪电伤害

    [Header("物理防御")]
    public float armor;//护甲
    public float evasion;//回避率

    [Header("法术防御")]
    public float fireResistance;//火焰抗性
    public float iceResistance;//冰霜抗性
    public float lightningResistance;//闪电抗性

    [Header("基础属性")]
    public float strength;//力量
    public float agility;//敏捷
    public float intelligence;//智力
    public float vitality;//体力
}
