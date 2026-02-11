using UnityEngine;

public class Player_AiredState : PlayerState
{
    public Player_AiredState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (player.moveInput.x != 0)
            player.SetVelocity(player.moveInput.x * (player.movespeed * player.inAirMoveMultiplier), rb.linearVelocity.y);

        if (input.Player.Attack.WasPressedThisFrame() && player.abilityUnlocked[SkillType.JumpAttack])
            stateMachine.ChangeState(player.jumpAttackState);
    }
}
