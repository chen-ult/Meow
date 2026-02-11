using System;
using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public Stat_SetupSO defaultStatSetup;

    // notify listeners when stats change (base or modifiers)
    public event Action OnStatsChanged;

    public void NotifyStatsChanged()
    {
        OnStatsChanged?.Invoke();
    }


    
    public Stat_ResourceGroup resource;//资源组
    public Stat_OffenseGroup offense;//进攻组
    public Stat_DefenseGroup defense;//防御组
    public Stat_MajorGroup major;//主要属性组

    public float GetElementalDamage(out ElementType element, float scaleFactor = 1)//获取元素伤害
    {
        float fireDamage = offense.fireDamage.GetValue();
        float iceDamage = offense.iceDamage.GetValue();
        float lightningDamage = offense.lightningDamage.GetValue();

        float bonusElementalDamage = major.intelligence.GetValue();

        float highestDamage = fireDamage;
        element = ElementType.Fire;

        if (iceDamage > highestDamage)
        {
            highestDamage = iceDamage;
            element = ElementType.Ice;
        }

        if(lightningDamage > highestDamage)
        {
            highestDamage = lightningDamage;
            element = ElementType.Lightning;
        }

        if(highestDamage <= 0)
        {
            element = ElementType.None;
            return 0;
        }

        float bonusFire = (fireDamage == highestDamage) ? 0 : fireDamage * 0.5f;
        float bonusIce = (iceDamage == highestDamage) ? 0 : iceDamage * 0.5f;
        float bonusLightning = (lightningDamage == highestDamage) ? 0 : lightningDamage * 0.5f;

        float weakerElementalDamage = bonusFire + bonusIce + bonusLightning;
        float finalElementalDamage = highestDamage + weakerElementalDamage + bonusElementalDamage;


        return finalElementalDamage * scaleFactor;
    }

    public float GetElementalResistance(ElementType element)//获取元素抗性
    {
        float baseResistance = 0;
        float bonusResistance = major.intelligence.GetValue() * 0.5f;

        switch(element)
        {
            case ElementType.Fire:
                baseResistance = defense.fireRes.GetValue();
                break;
            case ElementType.Ice:
                baseResistance = defense.iceRes.GetValue();
                break;
            case ElementType.Lightning:
                baseResistance = defense.lightningRes.GetValue();
                break;
            default:
                return 0;
        }

        float totalResistance = baseResistance + bonusResistance;
        float resistanceCap = 75f;

        float finalResistance = Mathf.Clamp(totalResistance, 0, resistanceCap) / 100;


        return finalResistance;
    }

    public float GetPhysicalDamage(out bool isCrit, float scaleFacor = 1)//获取物理伤害
    {
        float baseDamage = offense.damage.GetValue();
        float bonusDamage = major.strength.GetValue();
        float totalDamage = baseDamage + bonusDamage;
        
        float baseCritChance = offense.critChance.GetValue();
        float bonusCritChance = major.agility.GetValue() * 0.3f;
        float totalCritChance = baseCritChance + bonusCritChance;

        float baseCritPower = offense.critPower.GetValue();
        float bonusCritPower = major.strength.GetValue() * 0.5f;
        float totalCritPower = (baseCritPower + bonusCritPower) / 100;//例如 150 / 100 = 1.5倍伤害 暴击倍率

        isCrit = UnityEngine.Random.Range(0f, 100f) < totalCritChance;

        float finalDamage = isCrit ? totalDamage * totalCritPower : totalDamage;

        return finalDamage * scaleFacor;
    }

    public float GetArmorMitigation(float armorReduction)//获取护甲减伤
    {
        float baseArmor = defense.armor.GetValue();
        float bonusArmor = major.vitality.GetValue();
        float totalArmor = baseArmor + bonusArmor;

        float reductionMultiplier = Mathf.Clamp01(1 - armorReduction);
        float effectiveArmor = totalArmor * reductionMultiplier;

        float mitigation = effectiveArmor / (effectiveArmor + 100f);
        float mitigationCap = 0.85f;
        float finalMitigation = Mathf.Clamp(mitigation, 0, mitigationCap);

        return finalMitigation;
    }

    public float GetArmorReduction()//获取护甲穿透
    {
        float finalArmorReduction = offense.armorReduction.GetValue() / 100;

        return finalArmorReduction;
    }

    public float GetEvasion()//获取回避率
    {
        float baseEvasion = defense.evasion.GetValue();
        float bonusEvasion = major.agility.GetValue() * .5f;

        float totalEvasion = baseEvasion + bonusEvasion;
        float evasionCap = 80f;

        float finalEvasion = Mathf.Clamp(totalEvasion, 0, evasionCap);

        return finalEvasion;
    }


    public float GetMaxHealth()//获取最大生命值
    {
        float baseMaxHealth = resource.maxHealth.GetValue();
        float bonusMaxHealth = major.vitality.GetValue() * 5;

        float finalMaxHealth = baseMaxHealth + bonusMaxHealth;
        return finalMaxHealth;
    }

    public void IncreaseMaxHealth(float amount)
    {
        if (resource == null || resource.maxHealth == null)
            return;

        resource.maxHealth.AddToBaseValue(amount);
        OnStatsChanged?.Invoke();
    }

    private void Awake()
    {
        // assign owner on contained Stats so they can notify back
        if (resource != null)
        {
            if (resource.maxHealth != null) resource.maxHealth.owner = this;
            if (resource.healthRegen != null) resource.healthRegen.owner = this;
        }
        if (offense != null)
        {
            if (offense.attackSpeed != null) offense.attackSpeed.owner = this;
            if (offense.damage != null) offense.damage.owner = this;
            if (offense.critChance != null) offense.critChance.owner = this;
            if (offense.critPower != null) offense.critPower.owner = this;
            if (offense.armorReduction != null) offense.armorReduction.owner = this;
            if (offense.fireDamage != null) offense.fireDamage.owner = this;
            if (offense.iceDamage != null) offense.iceDamage.owner = this;
            if (offense.lightningDamage != null) offense.lightningDamage.owner = this;
        }
        if (defense != null)
        {
            if (defense.armor != null) defense.armor.owner = this;
            if (defense.evasion != null) defense.evasion.owner = this;
            if (defense.fireRes != null) defense.fireRes.owner = this;
            if (defense.iceRes != null) defense.iceRes.owner = this;
            if (defense.lightningRes != null) defense.lightningRes.owner = this;
        }
        if (major != null)
        {
            if (major.strength != null) major.strength.owner = this;
            if (major.agility != null) major.agility.owner = this;
            if (major.intelligence != null) major.intelligence.owner = this;
            if (major.vitality != null) major.vitality.owner = this;
        }
    }

    public Stats GetStatsByType(StatType type)//通过类型获取属性
    {
        switch(type)
        {
            case StatType.MaxHealth: return resource.maxHealth;
            case StatType.HealthRegen: return resource.healthRegen;

            case StatType.Strength: return major.strength;
            case StatType.Agility: return major.agility;
            case StatType.Vitality: return major.vitality; 
            case StatType.Intelligence: return major.intelligence;

            case StatType.AttackSpeed: return offense.attackSpeed;
            case StatType.Damage: return offense.damage;
            case StatType.CritChance: return offense.critChance;
            case StatType.CritPower: return offense.critPower;
            case StatType.ArmorReduction: return offense.armorReduction;

            case StatType.FireDamage: return offense.fireDamage;
            case StatType.IceDamage: return offense.iceDamage;
            case StatType.LightningDamage: return offense.lightningDamage;

            case StatType.Armor: return defense.armor;
            case StatType.Evasion: return defense.evasion;
            case StatType.IceResistance: return defense.iceRes;
            case StatType.FireResistance: return defense.fireRes;
            case StatType.LightningResistance: return defense.lightningRes;

            default:
                Debug.LogWarning($"StatType {type} not implemented yet.");
                return null;
        }
    }

    [ContextMenu("更新默认数值分配")]
    public void ApplyDefaultStatSetup()//应用默认数值分配
    {
        if(defaultStatSetup == null)
        {
            Debug.Log("没有默认数值分配");
            return;
        }

        resource.maxHealth.SetBaseValue(defaultStatSetup.maxHealth);
        resource.healthRegen.SetBaseValue(defaultStatSetup.healthRegen);

        major.strength.SetBaseValue(defaultStatSetup.strength);
        major.agility.SetBaseValue(defaultStatSetup.agility);
        major.intelligence.SetBaseValue(defaultStatSetup.intelligence);
        major.vitality.SetBaseValue(defaultStatSetup.vitality);

        offense.attackSpeed.SetBaseValue(defaultStatSetup.attackSpeed);
        offense.damage.SetBaseValue(defaultStatSetup.damage);
        offense.critChance.SetBaseValue(defaultStatSetup.critChance);
        offense.critPower.SetBaseValue(defaultStatSetup.critpower);
        offense.armorReduction.SetBaseValue(defaultStatSetup.armorReduction);

        offense.fireDamage.SetBaseValue(defaultStatSetup.fireDamage);
        offense.iceDamage.SetBaseValue(defaultStatSetup.iceDamage);
        offense.lightningDamage.SetBaseValue(defaultStatSetup.lightningDamage);

        defense.armor.SetBaseValue(defaultStatSetup.armor);
        defense.evasion.SetBaseValue(defaultStatSetup.evasion);

        defense.fireRes.SetBaseValue(defaultStatSetup.fireResistance);
        defense.iceRes.SetBaseValue(defaultStatSetup.iceResistance);
        defense.lightningRes.SetBaseValue(defaultStatSetup.lightningResistance);
    }
}
