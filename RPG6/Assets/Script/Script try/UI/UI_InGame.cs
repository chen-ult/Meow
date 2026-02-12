using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_InGame : MonoBehaviour
{
    private Player player;
    private UI_SkillSlot[] skillSlots;
    [Header("Buff GCD")]
    [SerializeField] private UnityEngine.UI.Image buffGcdImage;
    private BuffGlobalCooldownManager buffGcdManager;

    [SerializeField] private RectTransform healthRect;
    [SerializeField] private Slider healthSlider;

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        if (player != null && player.stats != null)
        {
            player.stats.OnStatsChanged += UpdateHealthBar;
            // initialize UI immediately
            UpdateHealthBar();
        }

        if (player != null && player.health != null)
        {
            player.health.OnHealthUpdate += UpdateHealthBar;
            UpdateHealthBar();
        }

        skillSlots = GetComponentsInChildren<UI_SkillSlot>(true);
        // subscribe to player unlock events so UI updates only when player unlocks
        if (player != null)
        {
            player.OnAbilityUnlockedEvent += OnPlayerUnlockedAbility;
            // initialize skill slots from already-unlocked abilities
            foreach (var kv in player.abilityUnlocked)
            {
                if (kv.Value)
                {
                    // try to find matching Skill_Base to get its Skill_DataSO
                    var sk = player.skillManager?.GetSkillByType(kv.Key);
                    var data = sk != null ? sk.skillData : null;
                    if (data != null)
                        OnPlayerUnlockedAbility(kv.Key, data);
                }
            }
        }
    }

    public UI_SkillSlot GetSkillSlot(SkillType skillType)
    {
        foreach (var slot in skillSlots)
        {
            if (slot.skillType == skillType)
                return slot;
        }

        return null;

    }
    private void OnDestroy()
    {
        if (player != null && player.stats != null)
            player.stats.OnStatsChanged -= UpdateHealthBar;
        if (player != null && player.health != null)
            player.health.OnHealthUpdate -= UpdateHealthBar;
        if (player != null)
            player.OnAbilityUnlockedEvent -= OnPlayerUnlockedAbility;
    }

    private void OnPlayerUnlockedAbility(SkillType type, Skill_DataSO skillData)
    {
        // find the matching slot and update it
        var slot = GetSkillSlot(type);
        if (slot != null && skillData != null)
        {
            slot.SetUpSkillSlot(skillData);
        }
    }

    private void UpdateHealthBar()
    {
        float currentHealth = Mathf.RoundToInt(player.health.GetCurrentHealth());
        float maxHealth = player.stats.GetMaxHealth();
        float sizeDiffrnece = Mathf.Abs(maxHealth - healthRect.sizeDelta.x);


        if(sizeDiffrnece > .1f)
            healthRect.sizeDelta = new Vector2(maxHealth, healthRect.sizeDelta.y);




        healthSlider.value = player.health.GetHealthPercent();
    }
}
