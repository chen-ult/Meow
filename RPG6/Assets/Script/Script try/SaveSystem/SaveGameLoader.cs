using UnityEngine;

public class SaveGameLoader : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private string startPointId = "Start";
    [SerializeField] private string startPointDisplayName = "Start";

    private void Start()
    {
        var player = FindFirstObjectByType<Player>();
        var manager = SaveManager.Instance;
        if (player == null || manager == null)
            return;

        if (startPoint != null)
        {
            manager.SetStartPoint(startPoint.position);
            manager.RegisterSavePoint(startPointId, startPoint.position, startPointDisplayName, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        if (manager.HasPendingRespawn())
        {
            manager.ApplyPendingRespawn(player);
            return;
        }

        if (manager.HasPendingTeleport())
        {
            manager.ApplyPendingTeleport(player);
            return;
        }

        if (manager.HasPendingSpawnPoint())
        {
            manager.ApplyPendingSpawnPoint(player);
            return;
        }

        if (manager.PendingLoad)
        {
            manager.Load(player);
            manager.ClearPending();
            return;
        }

        if (manager.PendingNewGame)
        {
            manager.ResetSaveAndRespawn(player);
            return;
        }

        // rebind cinemachine virtual cameras to player after scene load
        var binder = FindFirstObjectByType<CinemachineFollowBinder>();
        binder?.BindAll();
    }
}
