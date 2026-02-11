using System.Collections;
using UnityEngine;


[System.Serializable]
public class Buff
{
    public StatType type;//属性类型
    public float value;//属性值
}

public class Apply_Buff : Skill_Base
{
    private Entity_Stats statsToModify; // 要修改的实体属性
    private BuffGlobalCooldownManager globalManager;
    // internal unique id used as modifier source to avoid name collisions
    private string modifierSourceId;

    [Header("Buff参数")]
    [SerializeField] private Buff[] buffs; // 属性数组
    [SerializeField] private string buffName; // 属性名称（用于 modifier 源）
    [SerializeField] private float buffDuration = 4; // 属性持续时间
    [SerializeField] private bool canBeUsed = true; // 是否可以使用（每个技能独立）

    [Header("Input")]
    public KeyCode activationKey = KeyCode.None;
    private bool isBuffActive = false;

    protected override void Awake()
    {
        base.Awake();

        statsToModify = GetComponentInParent<Entity_Stats>() ?? player?.stats;

        // build a unique source id; if designer did not set buffName, fall back to object name + instance id
        modifierSourceId = string.IsNullOrEmpty(buffName)
            ? gameObject.name + "_" + GetInstanceID()
            : buffName + "_" + GetInstanceID();

        // register with global manager if exists
        globalManager = FindFirstObjectByType<BuffGlobalCooldownManager>();
        if (globalManager != null)
        {
            globalManager.RegisterBuff(this);
            // do not change this.skill cooldown here; personal cooldown will be started when buff ends
        }
    }

    private void Update()
    {
        // only react to input if a key is assigned and this is attached under the player
        if (activationKey == KeyCode.None) return;
        if (player == null) return;

        // check ability unlocked on player
        if (!player.abilityUnlocked.ContainsKey(skillType) || !player.abilityUnlocked[skillType])
            return;

        if (Input.GetKeyDown(activationKey))
            UseBuff();

        // if this skill is currently disabled and not active, re-enable it when global cooldown finishes
        if (!canBeUsed && !isBuffActive && globalManager != null && globalManager.CanUseAnyBuff())
        {
            canBeUsed = true;
        }
    }

    public void UseBuff()
    {
        // prevent starting multiple buff coroutines while one is active
        if (isBuffActive)
            return;

        // per-skill availability
        if (!canBeUsed)
            return;

        // global cooldown check
        if (globalManager != null && !globalManager.CanUseAnyBuff())
            return;

        // mark as active and used so same skill can't be reused while active
        isBuffActive = true;
        canBeUsed = false;

        // trigger global cooldown immediately so UI shows it right away
        globalManager?.TriggerGlobalCooldown();

        StartCoroutine(BuffCo(buffDuration));
    }

    private IEnumerator BuffCo(float duration) // Buff 协程
    {
        ApplyBuff(true);

        yield return new WaitForSeconds(duration);

        ApplyBuff(false);

        // After buff ends, do not retrigger global cooldown (already triggered at use)
        if (globalManager != null)
        {
            // just mark buff as inactive; Update() will re-enable when global cooldown finishes
            isBuffActive = false;
        }
        else
        {
            // no global manager -> allow reuse immediately
            isBuffActive = false;
            canBeUsed = true;
        }
    }

    private void ApplyBuff(bool apply) // 应用/移除 Buff
    {
        if (statsToModify == null) return;

        foreach (var buff in buffs)
        {
            var stat = statsToModify.GetStatsByType(buff.type);
            if (stat == null) continue;

            if (apply)
                stat.AddModifier(buff.value, modifierSourceId);
            else
                stat.RemoveModifier(modifierSourceId);
        }
    }

    // expose activation for external callers (Skill_Base uses UseBuff)
    public void ActivateBuff()
    {
        UseBuff();
    }
}
