using UnityEngine;

public class BossSpawnPoint : MonoBehaviour
{
    [SerializeField] private string pointId = "BossEntry";

    public string PointId => pointId;
    public Vector3 Position => transform.position;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
