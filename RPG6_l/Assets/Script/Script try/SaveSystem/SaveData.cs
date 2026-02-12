using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public float maxHealthBase;
    public float damageBase;
    public Vector3 checkpointPosition;
    public string checkpointScene;
    public int currency;
    public List<SavePointData> unlockedSavePoints = new List<SavePointData>();
    public List<SkillType> unlockedSkills = new List<SkillType>();
}

[Serializable]
public class SavePointData
{
    public string id;
    public string displayName;
    public Vector3 position;
    public string sceneName;
}
