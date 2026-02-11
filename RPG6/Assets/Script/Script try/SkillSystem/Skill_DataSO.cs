using System;
using UnityEngine;

[CreateAssetMenu(menuName = "游戏数据/技能属性", fileName = "技能属性")]
public class Skill_DataSO : ScriptableObject
{
    public int cost; //技能消耗
    public SkillType skillType;
    public Upgradedata upgradeData;

    [Header("技能数据")]
    public string displayName;//技能名称
    [TextArea]
    public string description;//技能描述
    public Sprite icon;//技能图标
}

[Serializable]
public class Upgradedata
{
    public SkillUpgradeType upgradeType;
    public float cooldown;
}
