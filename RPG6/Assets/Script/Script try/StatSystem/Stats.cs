using System;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stats 
{
    [SerializeField] private float baseValue;//基础数值
    [SerializeField] private List<StatModifier> modifiers = new List<StatModifier>();//数值修正列表

    // optional owner reference used to notify when this stat changes
    [System.NonSerialized]
    public Entity_Stats owner;

    private bool needToCalculated = true;//是否需要重新计算最终数值
    private float finalValue;//最终数值缓存

    public float GetValue()//获取最终数值
    {
        if(needToCalculated)
        {
            finalValue = GetFinalValue();
            needToCalculated = false;
        }
        return finalValue;
    }

    public float GetBaseValue() => baseValue;

    public void AddModifier(float value, string source)//添加数值修正
    {
        StatModifier modToAdd = new StatModifier(value, source);
        modifiers.Add(modToAdd);
        needToCalculated = true;
        if (owner != null) owner.NotifyStatsChanged();
    }


    public void RemoveModifier(string source)//移除数值修正
    {
        modifiers.RemoveAll(modifier => modifier.source == source);
        needToCalculated = true;
        if (owner != null) owner.NotifyStatsChanged();
    }

    private float GetFinalValue()//计算最终数值
    {
        float finalValue = baseValue;

        foreach(var modifier in modifiers)
        {
            finalValue += modifier.value;
        }

        return finalValue;
    }

    public void SetBaseValue(float value) => baseValue = value;//设置基础数值

    public void AddToBaseValue(float delta)
    {
        baseValue += delta;
        needToCalculated = true;
        if (owner != null) owner.NotifyStatsChanged();
    }

    public void SetBaseValueAndNotify(float value)
    {
        baseValue = value;
        needToCalculated = true;
        if (owner != null) owner.NotifyStatsChanged();
    }

}


[Serializable]
public class StatModifier
{
    public float value;
    public string source;

    public StatModifier (float value, string source)
    {
        this.value = value;
        this.source = source;
    }
}
