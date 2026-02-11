using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player_Health : Entity_Health
{
    [Header("玩家专属回血设置")]
    [SerializeField] private float healCooldown = 5f; // 回血冷却时间（秒）
    [SerializeField] private float healDuration = 5f;
    [SerializeField] private KeyCode healKey = KeyCode.R; // 主动回血按键
    [Header("冷却UI可选")]
    [SerializeField] private Image cooldownImage; // 冷却进度图（可选，需设置为Filled类型）
    [SerializeField] private TextMeshProUGUI cooldownText; // 可选：显示剩余秒数或 Ready 文本

    private float healCooldownTimer; // 冷却计时器（0=冷却中，healCooldown=冷却完成）
    // 可回血判定：冷却完成 + 未死亡 + 非满血
    private bool CanHeal => healCooldownTimer >= healCooldown && !isDead && currentHealth < entityStats.GetMaxHealth();

    protected override void Awake()
    {
        base.Awake();

        canRegenerateHealth = false;
        // 初始冷却完成：计时器设为冷却时间
        healCooldownTimer = healCooldown;
        // 防止冷却时间为0导致的逻辑异常
        if (healCooldown <= 0)
        {
            Debug.LogWarning("玩家回血冷却时间不能小于等于0，已自动设为1秒", this);
            healCooldown = 1f;
        }

        // initialize optional UI
        if (cooldownImage != null)
        {
            cooldownImage.type = Image.Type.Filled;
            cooldownImage.fillAmount = Mathf.Clamp01(healCooldownTimer / healCooldown);
            cooldownImage.enabled = true;
        }

        if (cooldownText != null)
            cooldownText.text = "";
    }

    protected virtual void Update()
    {
        if (isDead) return;

        // 冷却计时器更新：冷却中时递减，最大不超过冷却时间
        if (healCooldownTimer < healCooldown)
        {
            healCooldownTimer += Time.deltaTime;
            if (healCooldownTimer > healCooldown) healCooldownTimer = healCooldown;
            UpdateHealCooldownUI();
        }

        // 主动回血：按按键且满足条件时触发
        if (Input.GetKeyDown(healKey) && CanHeal)
        {
            PerformActiveHeal();
        }
    }

    // 主动回血核心方法：复用父类IncreaseHealth，减少冗余
    private void PerformActiveHeal()
    {

        StartCoroutine(RegenCo(healDuration));
       
        Debug.Log($"玩家主动回血 ，当前血量：{currentHealth:F1}/{entityStats.GetMaxHealth()}");

        // 触发冷却：重置计时器
        healCooldownTimer = 0;
        UpdateHealCooldownUI();
    }

    // 冷却进度条更新（Slider需放在Canvas下，手动赋值）
    private void UpdateHealCooldownUI()
    {
        if (cooldownImage != null)
        {
            float normalized = healCooldown > 0 ? Mathf.Clamp01(healCooldownTimer / healCooldown) : 1f;
            cooldownImage.fillAmount = normalized;
        }

        if (cooldownText != null)
        {
            float remaining = Mathf.Max(0f, healCooldown - healCooldownTimer);
            if (remaining <= 0f)
                cooldownText.text = ""; // ready, hide text
            else
                cooldownText.text = remaining.ToString("F1") + "s";
        }
    }

    private IEnumerator RegenCo(float duration)
    {
        canRegenerateHealth = true;

        yield return new WaitForSeconds(duration);

        canRegenerateHealth = false;
    }


    
}
