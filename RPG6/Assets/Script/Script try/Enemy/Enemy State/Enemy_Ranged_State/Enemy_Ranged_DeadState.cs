using UnityEngine;

public class Enemy_Ranged_DeadState : Enemy_RangedState
{
    private Collider2D col;
    public Enemy_Ranged_DeadState(Enemy enemy, StateMachine stateMachine, Enemy_Mage mage, string animBoolName) : base(enemy, stateMachine, mage, animBoolName)
    {
        col = enemy.GetComponent<Collider2D>();
    }
}
