using UnityEngine;

public class Player_BasicAttackState : PlayerState
{
    private float attackVelocityTimer = 0;
    private const int FirstComboIndex = 1;


    private bool comboAttackQueued;
    private int attackDir;
    private int comboIndex = 1;
    private int comboLimit = 3;

    private float lastTimeAttacked;

    public Player_BasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if(comboLimit != player.attackVelocity.Length)
        {
            Debug.LogWarning("连击种类数量超过限制，已调整为合规数量");
            comboLimit = player.attackVelocity.Length;
        }
    }

    public override void Enter()
    {
        base.Enter();
        comboAttackQueued = false;
        ResetComboIndex();
        SyncAttackSpeed();

        attackDir = player.moveInput.x != 0 ? ((int)player.moveInput.x) : player.facingDir;

        player.anim.SetInteger("basicAttackindex", comboIndex);
        ApplyAttackVelocity();
    }




    public override void Exit()
    {
        base.Exit();
        comboIndex++;
        lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();
        HandleAttackVelocity();

        if(input.Player.Attack.WasPressedThisFrame())
            QueueNextAttack();

        if (triggerCalled)
            HandleStateExit();
    }

    private void HandleStateExit()
    {
        if (comboAttackQueued)
        {
            anim.SetBool(animBoolName, false);
            player.EnterAttackStateWithDelay();
        }
        else
            stateMachine.ChangeState(player.idleState);
    }


    private void QueueNextAttack()
    {
        if (comboIndex < comboLimit)
            comboAttackQueued = true;
    }

    private void HandleAttackVelocity()
    {
        attackVelocityTimer -= Time.deltaTime;

        if(attackVelocityTimer < 0)
            player.SetVelocity(0, rb.linearVelocity.y);
    }

    private void ApplyAttackVelocity()
    {
        Vector2 attackVelocity = player.attackVelocity[comboIndex - 1];

        attackVelocityTimer = player.attackVelocityDuration;
        player.SetVelocity(attackVelocity.x * attackDir, attackVelocity.y);
    }
    private void ResetComboIndex()
    {
        

        if (comboIndex > comboLimit || Time.time > lastTimeAttacked + player.comboResetTime)
            comboIndex = FirstComboIndex;
    }
}
