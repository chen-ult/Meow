using UnityEngine;

public class Player_DeadState : PlayerState
{
    private bool deathMenuShown;
    public Player_DeadState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        input.Disable();
        rb.simulated = false;
        deathMenuShown = false;
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled && !deathMenuShown)
        {
            deathMenuShown = true;
            player.ui?.ShowDeathScreen(player);
        }
    }
}
