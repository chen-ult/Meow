using UnityEngine;

public class Boss : Enemy, ICounterable, ICanBeStunned
{

    public Boss_IdleState boss_idleState;
    public Boss_MoveState boss_moveState;
    public Boss_AttackState boss_attackState;
    public Boss_BattleState boss_battleState;
    public Boss_DeadState boss_deadState;
    public Boss_StunnedState boss_stunnedState;
    public Boss_Phrase2State boss_phrase2State;

    [Header("Boss Phase 2 Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float phase2HealthPercentThreshold = 0.5f; // 血量百分比阈值，低于此进入二阶段
    [SerializeField] private string phase2AnimBool = "phrase2";        // 进入二阶段时播放的动画 bool 名

    public bool hasEnteredPhase2;

    private Entity_Health health;

    [Header("Boss Phase 2 Firestorm Settings")]
    [SerializeField] private Firestorm firestormPrefab;   // 二阶段火焰飓风预制体
    [SerializeField] private float firestormSpeed = 4f;   // 覆盖 Firestorm.speed（可选）
    [SerializeField] private float firestormOffsetX = 3f; // 从 Boss 左右生成时的水平偏移
    [SerializeField] private float roomHalfWidth = 10f;   // Boss 房间半宽，用于两端生成
    [SerializeField] private int maxFirestormPattern = 3; // 循环的模式数
    [SerializeField] private float firestormCooldown = 2f; // 生成火焰飓风冷却时间

    private int phase2FirestormCastCount;
    private float nextFirestormTime;

    [Header("Boss Attack Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float lightAttackChance = 0.7f; // 概率：轻攻击
    [SerializeField] private float lightPhysicalScale = 1f;   // 轻攻击物理伤害倍率
    [SerializeField] private float lightElementScale = 0.5f;  // 轻攻击元素伤害倍率
    [SerializeField] private float heavyPhysicalScale = 1.5f; // 重攻击物理伤害倍率
    [SerializeField] private float heavyElementScale = 1.2f;  // 重攻击元素伤害倍率
    [SerializeField] private string attackTypeParam = "attackType"; // Animator 中控制轻/重攻的参数名

    [Header("Boss Phase 2 Damage Scale")] 
    [SerializeField] private float phase2LightPhysicalScale = 1.3f;   // 二阶段轻攻击物理伤害倍率
    [SerializeField] private float phase2LightElementScale = 0.8f;    // 二阶段轻攻击元素伤害倍率
    [SerializeField] private float phase2HeavyPhysicalScale = 2.0f;   // 二阶段重攻击物理伤害倍率
    [SerializeField] private float phase2HeavyElementScale = 1.6f;    // 二阶段重攻击元素伤害倍率

    // cache phase 1 values so they can be inspected / adjusted if needed
    private float baseLightPhysicalScale;
    private float baseLightElementScale;
    private float baseHeavyPhysicalScale;
    private float baseHeavyElementScale;

    protected override void Awake()
    {
        base.Awake();

        health = GetComponent<Boss_Health>();
        if (health != null)
        {
            health.OnHealthUpdate += OnHealthUpdated;
        }

        // remember initial (phase 1) attack scales
        baseLightPhysicalScale = lightPhysicalScale;
        baseLightElementScale = lightElementScale;
        baseHeavyPhysicalScale = heavyPhysicalScale;
        baseHeavyElementScale = heavyElementScale;

        boss_idleState = new Boss_IdleState(this, stateMachine,this, "idle");
        boss_moveState = new Boss_MoveState(this, stateMachine,this, "move");
        boss_attackState = new Boss_AttackState(this, stateMachine,this, "attack");
        boss_battleState = new Boss_BattleState(this, stateMachine,this, "battle");
        boss_deadState = new Boss_DeadState(this, stateMachine,this, "dead");
        boss_stunnedState = new Boss_StunnedState(this, stateMachine,this, "stunned");
        boss_phrase2State = new Boss_Phrase2State(this, stateMachine,this, "phrase2");

        // phase 2 currently driven only by Animator using phase2AnimBool; state
        // machine Boss_Phrase2State is not used to avoid interfering with
        // existing animation flow.
        // boss_phrase2State = new Boss_Phrase2State(this, stateMachine,this, "phrase2");


    }



    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(boss_idleState);
    }


    public override void EntityDeath()
    {
        base.EntityDeath();

        // when boss dies, switch to its dedicated dead state so death
        // animation/logic can play correctly
        if (stateMachine != null && boss_deadState != null)
        {
            stateMachine.ChangeState(boss_deadState);
        }
    }






    
    // called at the end of the phase 2 intro (e.g. by Boss_Phrase2State) to
    // boost boss damage and go back to normal battle behaviour
    public void EnterPhase2Combat()
    {
        // switch attack scales to phase 2 values
        lightPhysicalScale = phase2LightPhysicalScale;
        lightElementScale = phase2LightElementScale;
        heavyPhysicalScale = phase2HeavyPhysicalScale;
        heavyElementScale = phase2HeavyElementScale;

        stateMachine.ChangeState(boss_battleState);
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.OnHealthUpdate -= OnHealthUpdated;
        }
    }

    // 监听血量变化，一旦低于阈值切入二阶段
    private void OnHealthUpdated()
    {
        if (hasEnteredPhase2 || health == null)
            return;

        float healthPercent = health.GetHealthPercent();
        if (healthPercent <= phase2HealthPercentThreshold)
        {
            hasEnteredPhase2 = true;
            phase2FirestormCastCount = 0; // 进入二阶段时重置施法计数
            nextFirestormTime = Time.time + firestormCooldown;

            if (anim != null && !string.IsNullOrEmpty(phase2AnimBool))
            {
                anim.SetBool(phase2AnimBool, true);
            }

            if (boss_phrase2State != null)
            {
                stateMachine.ChangeState(boss_phrase2State);
            }
        }
    }





 //   // 监听血量变化，一旦低于阈值切入二阶段
 //   private void OnHealthUpdated()
 //   {
	//	if (hasEnteredPhase2 || health == null)
	//		return;

	//	float healthPercent = health.GetHealthPercent();
	//	if (healthPercent <= phase2HealthPercentThreshold)
	//	{
	//		hasEnteredPhase2 = true;
	//		phase2FirestormCastCount = 0; // 进入二阶段时重置施法计数
 //           IsInPhase2 = true;




 //       }
	//}







    // Called by animation event to perform an attack with random choice between light and heavy
    public void PerformRandomAttack()
    {
        // 这里只负责根据概率生成一个结果，再通过 Animator 参数让状态机决定播哪个动画
        float roll = Random.value;
        bool isLight = roll < lightAttackChance;

        // 约定：0 = heavy, 1 = light（可根据 Animator 设置调整）
        int attackType = isLight ? 1 : 0;

        if (anim != null && !string.IsNullOrEmpty(attackTypeParam))
        {
            anim.SetInteger(attackTypeParam, attackType);
        }
    }

    // 二阶段火焰飓风，由二阶段施法动画事件调用
    public void SummonFirestorm()
    {
        if (!hasEnteredPhase2 || firestormPrefab == null)
            return;

        if (Time.time < nextFirestormTime)
            return;

        nextFirestormTime = Time.time + firestormCooldown;

        phase2FirestormCastCount++;
        int patternIndex = ((phase2FirestormCastCount - 1) % maxFirestormPattern) + 1; // 1..max

        switch (patternIndex)
        {
            case 1:
                SummonFirestorm_FromBossOutwards();
                break;
            case 2:
                SummonFirestorm_FromRoomEdgesInwards();
                break;
            case 3:
            default:
                SummonFirestorm_Random();
                break;
        }
    }

    // 模式 1：从 Boss 位置左右生成，水平向外移动
    private void SummonFirestorm_FromBossOutwards()
    {
        Vector3 bossPos = transform.position;

        SpawnFirestorm(bossPos + Vector3.left * firestormOffsetX, Vector2.left);
        SpawnFirestorm(bossPos + Vector3.right * firestormOffsetX, Vector2.right);
    }

    // 模式 2：从房间两端向 Boss 方向移动
    private void SummonFirestorm_FromRoomEdgesInwards()
    {
        Vector3 bossPos = transform.position;

        Vector3 leftPos = new Vector3(bossPos.x - roomHalfWidth, bossPos.y, bossPos.z);
        Vector3 rightPos = new Vector3(bossPos.x + roomHalfWidth, bossPos.y, bossPos.z);

        SpawnFirestorm(leftPos, Vector2.right);
        SpawnFirestorm(rightPos, Vector2.left);
    }

    // 模式 3：随机出现在几种典型模式中
    private void SummonFirestorm_Random()
    {
        Vector3 bossPos = transform.position;
        int rand = Random.Range(0, 3);

        switch (rand)
        {
            case 0:
                // 从左右房间边缘向内
                SummonFirestorm_FromRoomEdgesInwards();
                break;
            case 1:
                // 从 Boss 左右向外
                SummonFirestorm_FromBossOutwards();
                break;
            case 2:
            default:
                // 随机一个侧面，向内
                bool fromLeft = Random.value < 0.5f;
                Vector3 pos = bossPos + (fromLeft ? Vector3.left : Vector3.right) * roomHalfWidth;
                Vector2 dir = fromLeft ? Vector2.right : Vector2.left;
                SpawnFirestorm(pos, dir);
                break;
        }
    }

    private void SpawnFirestorm(Vector3 position, Vector2 dir)
    {
        Firestorm instance = Instantiate(firestormPrefab, position, Quaternion.identity);

        // 覆盖它的参数以符合 Boss 技能手感
        if (instance != null)
        {
            instance.SetMoveDirection(dir);
            instance.speed = firestormSpeed;
        }
    }

    // Light attack uses full physical damage but reduced elemental contribution
    public void LightAttack()
    {
        if (stats == null || player == null)
            return;

        float physicalDamage = stats.GetPhysicalDamage(out bool isCrit, lightPhysicalScale);
        float elementalDamage = stats.GetElementalDamage(out ElementType element, lightElementScale);

        DoAttackDamage(physicalDamage, elementalDamage, isCrit, element);
    }

    // Heavy attack scales up both physical and elemental damage
    public void HeavyAttack()
    {
        if (stats == null || player == null)
            return;

        float physicalDamage = stats.GetPhysicalDamage(out bool isCrit, heavyPhysicalScale);
        float elementalDamage = stats.GetElementalDamage(out ElementType element, heavyElementScale);

        DoAttackDamage(physicalDamage, elementalDamage, isCrit, element);
    }

    // Centralized damage application that reuses the global damage pipeline
    private void DoAttackDamage(float physicalDamage, float elementalDamage, bool isCrit, ElementType element)
    {
        var targetHealth = player.GetComponent<Entity_Health>() ?? player.GetComponentInParent<Entity_Health>();
        if (targetHealth == null)
            return;

        bool targetGotHit = targetHealth.TakeDamage(physicalDamage, transform, elementalDamage, element);

        // You can hook boss-specific VFX/SFX here if needed when targetGotHit is true and/or isCrit
    }


    public bool CanBeCountered { get => canBeStunned; }
    public override void TryEnterBattleState(Transform player)
    {
        base.TryEnterBattleState(player);

        stateMachine.ChangeState(boss_battleState);
    }

    public void HandleCounter()//处理反击
    {
        if (!CanBeCountered)
            return;

        stateMachine.ChangeState(boss_stunnedState);
    }

    public void GetIntoStunned()
    {
        stateMachine.ChangeState(boss_stunnedState);
    }


}
