using UnityEngine;

public class Enemy_Ranged_TeleportState : Enemy_RangedState
{
    private float teleportDurationTimer;
    private float targetX;

    public Enemy_Ranged_TeleportState(Enemy enemy, StateMachine stateMachine, Enemy_Mage mage, string animBoolName) : base(enemy, stateMachine, mage, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // compute target position based on mage facing direction and configured distance
        targetX = enemy.transform.position.x + (mage.teleportDistance * mage.facingDir);

        // perform instant teleport
        enemy.transform.position = new Vector3(targetX, enemy.transform.position.y, enemy.transform.position.z);

        // mark teleport used on mage for cooldown
        mage.UseTeleport();

        // set a short duration before returning to battle
        teleportDurationTimer = mage.teleportDuration;
    }

    public override void Update()
    {
        base.Update();

        teleportDurationTimer -= Time.deltaTime;
        if (teleportDurationTimer <= 0f)
        {
            stateMachine.ChangeState(mage.ranged_BattleState);
        }
    }

}
