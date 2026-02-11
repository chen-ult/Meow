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
        // when boss已经进入第二阶段，则不再进行脱战回血
        if (boss != null )
            return;

        // otherwise use the normal Enemy_Health out-of-combat regen rules
        base.RegenerateHealth();
    }

    
}
