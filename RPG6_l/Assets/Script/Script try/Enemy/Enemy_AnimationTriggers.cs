using System.Xml.Serialization;
using UnityEngine;

public class Enemy_AnimationTriggers : Entity_AnimationTriggers
{
    private Boss boss;
    private Enemy enemy;//敌人引用
    private Enemy_VFX enemyVfx;//敌人视觉特效引用
    private Enemy_Mage mage;


    protected override void Awake()
    {
        base.Awake();

        enemy = GetComponentInParent<Enemy>();
        enemyVfx = GetComponentInParent<Enemy_VFX>();
        // 尝试获取特定的 Mage 实例（若该敌人为远程法师）
        mage = GetComponentInParent<Enemy_Mage>();
boss = GetComponentInParent<Boss>();
    }

    private void EnableCounterWindow()//启用反击窗口
    {
        enemyVfx.EnableAttackAlert(true);
        enemy.EnableCounterWindow(true);
    }

    private void DisableCounterWindow()//禁用反击窗口
    {
        enemyVfx.EnableAttackAlert(false);
        enemy.EnableCounterWindow(false);
    }
    

    private void Fire()
    {
        if (mage != null)
            mage.FireBullet();
        else if (enemy != null)
            Debug.LogWarning("Enemy_AnimationTriggers.Fire called but no Enemy_Mage found on parent.", enemy);
    }

    // ===== Boss 专用动画事件回调 =====

    // 轻攻击伤害，由 Boss 攻击动画事件调用
    private void Boss_LightAttack()
    {
        boss?.LightAttack();
    }

    // 重攻击伤害，由 Boss 攻击动画事件调用
    private void Boss_HeavyAttack()
    {
        boss?.HeavyAttack();
    }

    // 二阶段火焰飓风召唤，由二阶段施法动画事件调用
    private void Boss_SummonFirestorm()
    {
        boss?.SummonFirestorm();
    }

    private void Open_Boss_Invulnerable()
    {
        boss?.SetInvulnerable(true);
    }

    private void Close_Boss_Invulnerable()
    {
        boss?.SetInvulnerable(false);
    }
}
