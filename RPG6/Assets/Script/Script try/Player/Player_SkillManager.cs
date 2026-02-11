using UnityEngine;

public class Player_SkillManager : MonoBehaviour
{
    public Skill_Dash dash {  get; private set; }
    public Skill_AOE aoe { get; private set; }


    private void Awake()
    {
        dash = GetComponentInChildren<Skill_Dash>();
        aoe = GetComponentInChildren<Skill_AOE>();
    }

    public Skill_Base GetSkillByType(SkillType type)
    {
        switch (type) 
        {
            case SkillType.Dash: return dash;

            default:
                Debug.Log("没有技能");
                return null;
        }
    }


}
