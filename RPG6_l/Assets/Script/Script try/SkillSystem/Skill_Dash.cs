using UnityEngine;

public class Skill_Dash : Skill_Base
{

    public void OnStartEffect()
    {
        if(unlocked(SkillUpgradeType.Dash_CloneOnStart) || unlocked(SkillUpgradeType.Dash_CloneOnStartAndArrival))
            CreateClone();



        if(unlocked(SkillUpgradeType.Dash_ShardOnStart) || unlocked(SkillUpgradeType.Dash_ShardOnStartAndArrival))
            CreateShard(); 
    }

    public void OnEndEffect()
    {
        if (unlocked(SkillUpgradeType.Dash_CloneOnStartAndArrival))
            CreateClone();

        if(unlocked(SkillUpgradeType.Dash_ShardOnStartAndArrival))
            CreateShard();
    }


    private void CreateShard()
    {
        Debug.Log("时间碎片");


    }

    private void CreateClone()
    {
        Debug.Log("时间回收");
    }
}
