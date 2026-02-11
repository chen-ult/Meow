using UnityEngine;

public class Enemy_RangedState : EnemyState
{
    protected Enemy_Mage mage;
    public Enemy_RangedState(Enemy enemy, StateMachine stateMachine,Enemy_Mage mage, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        this.mage = mage;
    }

  

    //public override void Enter()
    //{
    //    rb.gravityScale = 12;
    //    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 15);

    //    stateMachine.SwitchOffStateMachine();
    //}
}
