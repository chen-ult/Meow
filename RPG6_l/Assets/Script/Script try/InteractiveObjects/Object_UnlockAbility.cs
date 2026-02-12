using System.Collections;
using UnityEngine;

public class Object_UnlockAbility : MonoBehaviour
{
    public SkillType unlockSkill;
    public Skill_DataSO skillData;
    public bool canBeUsed;
    private SpriteRenderer sr;//¾«ÁéäÖÈ¾Æ÷

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!canBeUsed)
            return;

        if (collision.collider.CompareTag("Player"))
        {
            // get Player on the collided object or its parents
            Player player = collision.collider.GetComponentInParent<Player>();
            if (player == null)
                return;

            // prefer the explicit skillData field on this pickup; fallback to Skill_DataComponent if present
            Skill_DataSO dataToPass = this.skillData;
            if (dataToPass == null)
            {
                var dataComp = GetComponent<Skill_DataComponent>() ?? GetComponentInChildren<Skill_DataComponent>();
                dataToPass = dataComp != null ? dataComp.skillData : null;
            }

            Debug.Log($"Object_UnlockAbility picked up. passing skillData={(dataToPass!=null?dataToPass.name:"null")}");
            player.UnlockAbility(unlockSkill, dataToPass);

            sr.color = Color.clear;
            canBeUsed = false;
            Destroy(gameObject);
        }
    }


    
}
