using System;
using UnityEngine;


[Serializable]
public class Stat_OffenseGroup 
{
    public Stats attackSpeed; //攻击速度

    //物理伤害
    public Stats damage;     //攻击力
    public Stats critPower;  //暴击强度
    public Stats critChance; //暴击率
    public Stats armorReduction; //护甲穿透

    //法术伤害
    public Stats fireDamage;  //火焰伤害
    public Stats iceDamage;   //冰霜伤害
    public Stats lightningDamage; //闪电伤害
}
