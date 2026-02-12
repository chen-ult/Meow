using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int count = 1;
    [SerializeField] private float spawnRadius = 0f;
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private bool spawnOnPlayerEnter = true;
    [SerializeField] private float triggerRadius = 3f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private bool spawnOnce = true;

    private readonly List<GameObject> spawned = new List<GameObject>();
    private bool hasSpawned;

    private void Start()
    {
        if (spawnOnStart)
            Spawn();
    }

    private void Update()
    {
        if (!spawnOnPlayerEnter)
            return;

        if (spawnOnce && hasSpawned)
            return;

        if (Physics2D.OverlapCircle(transform.position, triggerRadius, playerLayer) == null)
            return;

        Spawn();
    }

    public void Spawn()
    {
        if (enemyPrefab == null || count <= 0)
            return;

        if (spawnOnce && hasSpawned)
            return;

        for (int i = 0; i < count; i++)
        {
            Vector2 offset = spawnRadius > 0f ? Random.insideUnitCircle * spawnRadius : Vector2.zero;
            Vector3 pos = transform.position + new Vector3(offset.x, offset.y, 0f);
            var instance = Instantiate(enemyPrefab, pos, Quaternion.identity);
            spawned.Add(instance);
        }

        hasSpawned = true;
    }

    public void ClearSpawned()
    {
        for (int i = spawned.Count - 1; i >= 0; i--)
        {
            if (spawned[i] != null)
                Destroy(spawned[i]);
        }

        spawned.Clear();
        hasSpawned = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
