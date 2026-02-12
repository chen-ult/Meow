using UnityEngine;

public class Enemy_VFX : Entity_VFX
{
    [Header("Counter Attack Window")]
    [SerializeField] private GameObject attackAlert;//¹¥»÷¾¯±¨




    public void EnableAttackAlert(bool enable)//ÆôÓÃ¹¥»÷¾¯±¨
    {
        if (attackAlert == null)
            return;

        attackAlert.SetActive(enable);
    }
}
