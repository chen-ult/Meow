using Unity.Cinemachine;
using UnityEngine;

public class Player_Combat : Entity_Combat
{

    [Header("反击参数")]
    [SerializeField] private float counterRecovery;//反击恢复时间


    public override void PerformAttack()
    {
        base.PerformAttack();
    }

    public bool CounterAttackPerformed()//执行反击攻击
    {
        bool hasPerformedCounter = false;

        foreach(var target in GetDetectedColliders())
        {

            ICounterable counterable = target.GetComponent<ICounterable>();

            if (counterable == null)
                continue;

            if(counterable.CanBeCountered)
            {
                counterable.HandleCounter();
                hasPerformedCounter = true;
            }
        }

        return hasPerformedCounter;
    }

    public float GetCounterRecoveryDuration() => counterRecovery;//获取反击恢复时间

    
}
