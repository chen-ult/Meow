using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillSlot : MonoBehaviour
{
    private UI ui;
    private Image skillIcon;
    private RectTransform rect;
    private Button button;

    private Skill_DataSO skillData;
    private Skill_Base boundSkill;
    private BuffGlobalCooldownManager buffGcdManager;

    public SkillType skillType;
    [SerializeField] private Image cooldownImage;
    [SerializeField] private string inputKeyName;
    [SerializeField] private TextMeshProUGUI inputKeyText;

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        skillIcon = GetComponent<Image>();
        button = GetComponent<Button>();
        if (cooldownImage != null)
            cooldownImage.type = Image.Type.Filled;
        // default visual: slot appears dim / disabled until a skill is assigned
        if (skillIcon != null)
            skillIcon.color = new Color(1f, 1f, 1f, 0.5f);
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 0f;
            cooldownImage.enabled = false;
        }
        if (button != null)
            button.interactable = false;
       
    }

    public void SetUpSkillSlot(Skill_DataSO selectedSkill)
    {
        this.skillData = selectedSkill;

        Color color = Color.black;color.a = 0.6f;
        cooldownImage.color = color;

        inputKeyText.text = inputKeyName;
        skillIcon.sprite = selectedSkill.icon;
        // enable visuals now skill is assigned
        if (skillIcon != null)
            skillIcon.color = Color.white;
        if (cooldownImage != null)
            cooldownImage.enabled = true;
        if (button != null)
            button.interactable = true;
        // try to bind to runtime Skill_Base on player if exists
        var player = FindFirstObjectByType<Player>();
        if (player != null && player.skillManager != null)
        {
            boundSkill = player.skillManager.GetSkillByType(skillType);
        }

        // find global buff gcd manager if present
        buffGcdManager = FindFirstObjectByType<BuffGlobalCooldownManager>();
    }

    private void Update()
    {
        if (cooldownImage == null) return;

        float fill = 0f;
        // ensure we have a reference to the bound runtime skill (try recover if null)
        if (boundSkill == null)
        {
            var player = FindFirstObjectByType<Player>();
            if (player != null)
            {
                if (player.skillManager != null)
                    boundSkill = player.skillManager.GetSkillByType(skillType);

                // fallback: search any Skill_Base on the player (this handles buff skills implemented outside Player_SkillManager)
                if (boundSkill == null)
                {
                    var skills = player.GetComponentsInChildren<Skill_Base>(true);
                    foreach (var s in skills)
                    {
                        if (s != null && s.SkillType == skillType)
                        {
                            boundSkill = s;
                            break;
                        }
                    }
                }
            }
        }

        // If this slot represents a buff-type skill, show the global buff GCD when active
        if (IsBuffSkillType(skillType) && buffGcdManager != null && buffGcdManager.GetRemainingCooldown() > 0f)
        {
            if (buffGcdManager.globalCooldown > 0f)
                fill = Mathf.Clamp01(buffGcdManager.GetRemainingCooldown() / buffGcdManager.globalCooldown);
        }
        else if (boundSkill != null)
        {
            // otherwise show the individual skill cooldown
            fill = boundSkill.GetCooldownNormalized();
        }

        // show/hide cooldown image based on whether there's something to display
        cooldownImage.enabled = fill > 0f || skillData != null;

        cooldownImage.fillAmount = fill;
    }

    private bool IsBuffSkillType(SkillType type)
    {
        // treat elemental power skill types as buff slots
        return type == SkillType.FirePower || type == SkillType.IcePower || type == SkillType.LightningPower;
    }
}
