using UnityEngine;

public class Skill_Base : MonoBehaviour
{
    public Player player { get; private set; }
    public Player_SkillManager skillManager { get; private set; }


    [Header("一般参数")]
    [SerializeField] protected SkillType skillType;
    public SkillType SkillType => skillType;
    [SerializeField] protected SkillUpgradeType upgradeType;
    [SerializeField] protected float cooldown;
    [Header("Data")]
    [Tooltip("Optional Skill Data SO. If assigned the SO's values (skillType, upgradeData.cooldown) will be applied")]
    public Skill_DataSO skillData;
    private float lastTimeUsed;

    protected virtual void Awake()
    {
        player = GetComponentInParent<Player>();
        skillManager = GetComponentInParent<Player_SkillManager>();

        // apply SO at runtime if provided
        if (skillData != null)
        {
            skillType = skillData.skillType;
            // don't call SetSkillUpgrade here — defer applying full upgrade (and UI updates)
            // until owning Player has finished initializing (see Player.Start)
            if (skillData.upgradeData != null)
                cooldown = skillData.upgradeData.cooldown;
        }

        lastTimeUsed = lastTimeUsed - cooldown;
    }

    private void OnValidate()
    {
        // apply data in editor when user assigns a Skill_DataSO
        if (skillData != null)
        {
            skillType = skillData.skillType;
            if (skillData.upgradeData != null)
                cooldown = skillData.upgradeData.cooldown;
        }
    }

    public virtual void SetSkillUpgrade(Skill_DataSO skillData)
    {
        Upgradedata upgrade = skillData.upgradeData;
        upgradeType = upgrade.upgradeType;
        cooldown = upgrade.cooldown;

        // do not update UI here; UI will update when player triggers OnAbilityUnlockedEvent
        ResetCooldown();

    }

    public bool CanUseSkill()
    {
        if (OnCooldown())
        {
            Debug.Log("冷却中");
            return false;
        }



        return true;
    }


    protected bool unlocked(SkillUpgradeType upgradeToCheck) => upgradeType == upgradeToCheck;



    private bool OnCooldown() => Time.time < lastTimeUsed + cooldown;
    public void SetSkillOnCoolDown() => lastTimeUsed = Time.time;

    public void ResetCooldownBy(float cooldownReduction) => lastTimeUsed += cooldownReduction;

    public void ResetCooldown() => lastTimeUsed = Time.time;

    // Returns remaining cooldown time in seconds (0 when ready)
    public float GetCooldownRemaining()
    {
        float remaining = (lastTimeUsed + cooldown) - Time.time;
        return remaining > 0f ? remaining : 0f;
    }

    // Returns normalized remaining cooldown in range [0,1]. 0 = ready, 1 = just used
    public float GetCooldownNormalized()
    {
        if (cooldown <= 0f) return 0f;
        return Mathf.Clamp01(GetCooldownRemaining() / cooldown);
    }
}
