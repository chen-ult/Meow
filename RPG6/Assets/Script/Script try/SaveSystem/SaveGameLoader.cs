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

        if (manager.PendingLoad)
        {
            manager.Load(player);
            manager.ClearPending();
            return;
        }

        if (manager.PendingNewGame)
        {
            if (startPoint != null)
                player.transform.position = startPoint.position;

            player.health?.SetHealthToPercent(1f);
            manager.Save(player);
            manager.ClearPending();
        }
    }
}
