using System;
using UnityEngine;

[Serializable]
public class Stat_DefenseGroup 
{
    //物理防御
    public Stats armor;        //护甲
    public Stats evasion;      //闪避率

    //魔法防御
    public Stats fireRes;     //火焰抗性
    public Stats iceRes;      //冰霜抗性
    public Stats lightningRes; //闪电抗性
}
