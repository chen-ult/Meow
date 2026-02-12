using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public static Player Instance { get; private set; }
    public static event Action OnPlayerDeath;//玩家死亡事件
    public event Action<SkillType, Skill_DataSO> OnAbilityUnlockedEvent;

    private Apply_Buff applyBuff;//应用Buff组件
    public UI ui;
    public PlayerInputSet input { get; private set; }
    public Player_SkillManager skillManager { get; private set; }
    public Player_VFX vfx {  get; private set; }
    public Entity_Health health { get; private set; }

    #region State
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_WallSlideState wallSlideState { get; private set; }
    public Player_WallJumpState wallJumpState { get; private set; }
    public Player_DashState dashState { get; private set; }
    public Player_BasicAttackState basicAttackState { get; private set; }
    public Player_JumpAttackState jumpAttackState { get; private set; }
    public Player_DeadState deadState { get; private set; }
    public Player_CounterAttackState counterAttackState { get; private set; }
    public Player_AOEAttackState aoeAttackState { get; private set; }

    public Dictionary<SkillType, bool> abilityUnlocked = new Dictionary<SkillType, bool>()
    {
        {SkillType.WallSlide, false},
        {SkillType.Dash, false },
        {SkillType.AOE, false },
        {SkillType.JumpAttack, false },
        {SkillType.FirePower, false },
        {SkillType.IcePower, false },
        {SkillType.LightningPower, false }
    };

    #endregion 

    [Header("攻击参数")]
    public Vector2[] attackVelocity;//攻击速度数组
    public Vector2 jumpAttackVelocity;//跳跃攻击速度
    public float attackVelocityDuration = .1f;//攻击速度持续时间
    public float comboResetTime = 1;//连击重置时间
    private Coroutine queuedAttackCo;//排队攻击协程




    [Header("移动参数")]
    public float movespeed;//移动速度
    public float jumpforce = 5;//跳跃力
    public Vector2 wallJumpForce;//墙壁跳跃力

    [Range(0f, 1f)]
    public float inAirMoveMultiplier = .7f;//空中移动倍率
    [Range(0f, 1f)]
    public float wallSlideMultiplier = .3f;//墙壁滑行倍率

    [Header("冲刺参数")]
    public float dashDuration = .25f;//冲刺持续时间
    public float dashSpeed = 20;//冲刺速度

    public Vector2 moveInput { get; private set; }//移动输入

    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        base.Awake();

        ui = FindAnyObjectByType<UI>();
        input = new PlayerInputSet();
        skillManager = GetComponent<Player_SkillManager>();
        vfx = GetComponent<Player_VFX>();
        applyBuff = GetComponentInChildren<Apply_Buff>();
        health = GetComponentInChildren<Entity_Health>();

        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        fallState = new Player_FallState(this, stateMachine, "jumpFall");
        wallSlideState = new Player_WallSlideState(this, stateMachine, "wallSlide");
        wallJumpState = new Player_WallJumpState(this, stateMachine, "jumpFall");
        dashState = new Player_DashState(this, stateMachine, "dash");
        basicAttackState = new Player_BasicAttackState(this, stateMachine, "basicAttack");
        jumpAttackState = new Player_JumpAttackState(this, stateMachine, "jumpAttack");
        deadState = new Player_DeadState(this, stateMachine, "dead");
        counterAttackState = new Player_CounterAttackState(this, stateMachine, "counterAttack");
        aoeAttackState = new Player_AOEAttackState(this, stateMachine, "aoe");

    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    

    protected override IEnumerator SlowDownEntityCo(float slowMultiplier, float duration)//减速实体协程(减速实体的子程序，重写)
    {
        float originalMoveSpeed = movespeed;
        float originalJumpForce = jumpforce;
        float originalAnimSpeed = anim.speed;
        Vector2 originalWallJump = wallJumpForce;
        Vector2 originalJumpAttack = jumpAttackVelocity;
        Vector2[] originalAttackVelocity = attackVelocity;

        float speedMutiplier = 1 - slowMultiplier;

        movespeed *= speedMutiplier;
        jumpforce *= speedMutiplier;
        anim.speed *= speedMutiplier;
        wallJumpForce *= speedMutiplier;
        jumpAttackVelocity *= speedMutiplier;

        for(int i = 0; i < attackVelocity.Length; i++)
        {
            attackVelocity[i] *= speedMutiplier;
        }

        yield return new WaitForSeconds(duration);

        movespeed = originalMoveSpeed;
        jumpforce = originalJumpForce;
        anim.speed = originalAnimSpeed;
        wallJumpForce = originalWallJump;
        jumpAttackVelocity = originalJumpAttack;

        for (int i = 0; i < attackVelocity.Length; i++)
        {
            attackVelocity[i] = originalAttackVelocity[i];
        }
    }

    public override void EntityDeath()//实体死亡，重写
    {
        base.EntityDeath();

        OnPlayerDeath?.Invoke();
        stateMachine.ChangeState(deadState);
    }

    public bool RespawnFromSave()
    {
        if (SaveManager.Instance == null)
            return false;

        if (!SaveManager.Instance.TryRespawn(this))
            return false;

        health?.Revive();
        rb.simulated = true;
        input.Enable();
        rb.linearVelocity = Vector2.zero;
        stateMachine.ChangeState(idleState);
        return true;
    }

    public void EnterAttackStateWithDelay()//延迟进入攻击状态
    {
        if (queuedAttackCo != null)
        {
            StopCoroutine(queuedAttackCo);
        }

        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCo());
    }

    private IEnumerator EnterAttackStateWithDelayCo()//延迟进入攻击状态协程(延迟进入攻击状态的执行子程序)
    {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(basicAttackState);
    }

    private void OnEnable()//启用输入系统
    {
        input.Enable();

        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;

        input.Player.ToggleSkillTreeUI.performed += ctx => ui.ToggleSkillTreeUI();
    }

    private void OnDisable()//禁用输入系统
    {
        input.Disable();
    }

    public void UnlockAbility(SkillType skillType, Skill_DataSO skillData = null)
    {
        if (abilityUnlocked.ContainsKey(skillType))
        {
            abilityUnlocked[skillType] = true;
            Debug.Log($"UnlockAbility called for {skillType}, skillData={(skillData!=null ? skillData.name : "null")}");

            if (skillData != null && skillManager != null)
            {
                var sk = skillManager.GetSkillByType(skillType);
                if (sk != null)
                    sk.SetSkillUpgrade(skillData);
            }

            OnAbilityUnlockedEvent?.Invoke(skillType, skillData);
        }
    }

}
