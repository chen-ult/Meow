using UnityEngine;
using Unity.Cinemachine;

public class CinemachineFollowBinder : MonoBehaviour
{
    [SerializeField] private bool bindLookAt = false;

    private void Start()
    {
        BindAll();
    }

    public void BindAll()
    {
        var player = Player.Instance ?? FindFirstObjectByType<Player>();
        if (player == null)
            return;

        var vCams = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);
        foreach (var cam in vCams)
        {
            if (cam == null)
                continue;

            cam.Follow = player.transform;
            if (bindLookAt)
                cam.LookAt = player.transform;
        }
    }
}
