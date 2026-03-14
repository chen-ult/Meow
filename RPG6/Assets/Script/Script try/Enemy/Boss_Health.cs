using UnityEngine;

public class Boss_Health : Enemy_Health
{
    private Boss boss;

    protected override void Awake()
    {
        base.Awake();
        // cache Boss reference on the same object or parent
        boss = GetComponent<Boss>() ?? GetComponentInParent<Boss>();
    }

    protected override void RegenerateHealth()
    {
        // when boss已经进入第二阶段，则不再进行回血
        if (boss != null && boss.hasEnteredPhase2)
            return;

        if (!enableOutOfCombatRegen || outOfCombatTimer < outOfCombatTime)
            return;

        if (!canRegenerateHealth)
            return;

        float regenAmount = entityStats.resource.healthRegen.GetValue();
        IncreaseHealth(regenAmount);
    }

    
}
