using UnityEngine;

public class Bullet_AnimationTrigger : MonoBehaviour
{
    private Bullet bullet;

    private void Awake()
    {
        bullet = GetComponentInParent<Bullet>();
    }

    public void AttackTrigger()
    {
        // Animation event: only play VFX, do not apply damage here.
        bullet.PlayHitVfxFallback();
    }
}
