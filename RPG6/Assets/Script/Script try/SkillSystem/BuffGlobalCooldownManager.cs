using System.Collections.Generic;
using UnityEngine;

// Simple global cooldown manager for buff skills.
public class BuffGlobalCooldownManager : MonoBehaviour
{
    public float globalCooldown = 3f;
    private float lastUsedTime = -999f;
    private readonly List<Apply_Buff> registeredBuffs = new List<Apply_Buff>();

    public void RegisterBuff(Apply_Buff buff)
    {
        if (!registeredBuffs.Contains(buff))
            registeredBuffs.Add(buff);
    }

    public bool CanUseAnyBuff()
    {
        return Time.time >= lastUsedTime + globalCooldown;
    }

    public void TriggerGlobalCooldown()
    {
        lastUsedTime = Time.time;
    }

    // Optional: expose remaining cooldown
    public float GetRemainingCooldown()
    {
        float remaining = (lastUsedTime + globalCooldown) - Time.time;
        return remaining > 0 ? remaining : 0f;
    }
}
