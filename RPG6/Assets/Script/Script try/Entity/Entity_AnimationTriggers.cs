using Unity.Cinemachine;
using UnityEngine;

public class Entity_AnimationTriggers : MonoBehaviour
{
    private Entity entity;
    private Entity_Combat entityCombat;
    private Player_SkillManager skillManager;
    public CinemachineImpulseSource impulseSource;

    protected virtual void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entityCombat = GetComponentInParent<Entity_Combat>();
        skillManager = GetComponentInParent<Player_SkillManager>();
    }

    private void CurrentStateTrigger()//¶¯»­×´Ì¬´¥·¢Æ÷
    {
        entity.CurrentStateAnimationTrigger();
    }

    private void AttackTrigger()//¹¥»÷´¥·¢Æ÷
    {
        entityCombat.PerformAttack();
    }

    private void AOEAttackTrigger()//AOE¹¥»÷´¥·¢Æ÷
    {
        skillManager.aoe.PerformAOE();
    }

    private void CameraShakeTrigger()
    {
        impulseSource.GenerateImpulse();
    }
}
