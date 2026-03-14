using UnityEngine;

public class Enemy_Ranged_DeadState : Enemy_RangedState
{
    private Collider2D col;
    public Enemy_Ranged_DeadState(Enemy enemy, StateMachine stateMachine, Enemy_Mage mage, string animBoolName) : base(enemy, stateMachine, mage, animBoolName)
    {
        col = enemy.GetComponent<Collider2D>();
    }


    public override void Enter()
    {
        anim.enabled = false;
        col.enabled = false;

        rb.gravityScale = 12;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 15);

        stateMachine.SwitchOffStateMachine();
    }
}
