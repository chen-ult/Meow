using UnityEngine;

public class DropOnDeath : MonoBehaviour
{
    [System.Serializable]
    public class DropEntry
    {
        public GameObject prefab;
        public float chance = 1f; // 0..1
        public int min = 1;
        public int max = 1;
    }

    public DropEntry[] drops;

    public void DropItems()
    {
        foreach (var entry in drops)
        {
            if (entry.prefab == null) continue;

            if (Random.value <= Mathf.Clamp01(entry.chance))
            {
                int count = Random.Range(entry.min, entry.max + 1);
                for (int i = 0; i < count; i++)
                {
                    Instantiate(entry.prefab, transform.position, Quaternion.identity);
                }
            }
        }
    }
}
