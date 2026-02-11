using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] private EnemySpawnPoint[] spawnPoints;

    private void Awake()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            spawnPoints = GetComponentsInChildren<EnemySpawnPoint>(true);
    }

    private void Start()
    {
        SpawnAll();
    }

    public void SpawnAll()
    {
        if (spawnPoints == null)
            return;

        foreach (var point in spawnPoints)
        {
            if (point != null)
                point.Spawn();
        }
    }

    public void ClearAll()
    {
        if (spawnPoints == null)
            return;

        foreach (var point in spawnPoints)
        {
            if (point != null)
                point.ClearSpawned();
        }
    }
}
