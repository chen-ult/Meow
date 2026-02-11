using UnityEngine;

public class Enemy_Skeleton : Enemy , ICounterable, ICanBeStunned
{
    public bool CanBeCountered { get => canBeStunned; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        battleState = new Enemy_BattleState(this, stateMachine, "battle");
        deadState = new Enemy_DeadState(this, stateMachine, "idle");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }


    public override void TryEnterBattleState(Transform player)
    {
        base.TryEnterBattleState(player);

        stateMachine.ChangeState(battleState);
    }

    public void HandleCounter()//´¦Àí·´»÷
    {
        if (!CanBeCountered)
            return;

            stateMachine.ChangeState(stunnedState);
    }

    public void GetIntoStunned()
    {
        stateMachine.ChangeState(stunnedState);
    }

   
   


}
