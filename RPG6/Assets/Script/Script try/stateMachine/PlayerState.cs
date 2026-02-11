using Unity.VisualScripting;
using UnityEngine;

public abstract class PlayerState : EntityState
{
    protected Player player;
    protected PlayerInputSet input;
    protected Player_SkillManager skillManager;



    public PlayerState(Player player,StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.player = player;

        anim = player.anim;
        rb = player.rb;
        stats = player.stats;
        input = player.input;
        skillManager = player.skillManager;
    }

    public override void Update()
    {
        base.Update();


        if (input.Player.Dash.WasPressedThisFrame() && CanDash())
        {
            skillManager.dash.SetSkillOnCoolDown();
            stateMachine.ChangeState(player.dashState);
        }

        if (input.Player.AOE.WasPressedThisFrame() && CanAOE())
        {
            skillManager.aoe.SetSkillOnCoolDown();
            stateMachine.ChangeState(player.aoeAttackState);

        }

    }

    public override void UpdateAnimationParameters()//更新动画参数
    {
        base.UpdateAnimationParameters();

        anim.SetFloat("yVelocity", rb.linearVelocity.y);

    }

    private bool CanAOE()
    {
        if (!skillManager.aoe.CanUseSkill())
            return false;

        if (stateMachine.currentState == player.aoeAttackState)
            return false;

        if (!player.groundDetected)
            return false;

        if(!player.abilityUnlocked[SkillType.AOE])
            return false;

        return true;
    }


    private bool CanDash()//能否冲刺
    {
        if (!player.abilityUnlocked[SkillType.Dash])
            return false;

        if(!skillManager.dash.CanUseSkill())
            return false;

        if (player.wallDetected)
            return false;

        if (stateMachine.currentState == player.dashState)
            return false;

        return true;
        
    }

}
