using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public bool PendingLoad { get; private set; }
    public bool PendingNewGame { get; private set; }
    public bool PendingShowBossHealthBar { get; private set; }

    private bool hasStartPoint;
    private Vector3 startPointPosition;

    private readonly System.Collections.Generic.List<SavePointData> unlockedSavePoints =
        new System.Collections.Generic.List<SavePointData>();

    private SaveData pendingRespawnData;
    private SavePointData pendingTeleportPoint;
    private string pendingSpawnPointId;

    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void BeginNewGame()
    {
        PendingNewGame = true;
        PendingLoad = false;
        DeleteSave();
    }

    public void BeginContinue()
    {
        PendingLoad = true;
        PendingNewGame = false;
    }

    public void ClearPending()
    {
        PendingLoad = false;
        PendingNewGame = false;
    }

    public void RequestShowBossHealthBar()
    {
        PendingShowBossHealthBar = true;
    }

    public void ClearShowBossHealthBar()
    {
        PendingShowBossHealthBar = false;
    }

    public void RequestSpawnPoint(string pointId)
    {
        pendingSpawnPointId = pointId;
    }

    public bool HasPendingSpawnPoint() => !string.IsNullOrEmpty(pendingSpawnPointId);

    public void ApplyPendingSpawnPoint(Player player)
    {
        if (player == null || string.IsNullOrEmpty(pendingSpawnPointId))
            return;

        var points = Object.FindObjectsByType<BossSpawnPoint>(FindObjectsSortMode.None);
        foreach (var point in points)
        {
            if (point != null && point.PointId == pendingSpawnPointId)
            {
                player.transform.position = point.Position;
                pendingSpawnPointId = null;
                return;
            }
        }

        pendingSpawnPointId = null;
    }

    public void SetStartPoint(Vector3 position)
    {
        startPointPosition = position;
        hasStartPoint = true;
    }

    public void RegisterSavePoint(string id, Vector3 position, string displayName = null, string sceneName = null)
    {
        if (string.IsNullOrEmpty(id))
            return;

        var existing = unlockedSavePoints.Find(p => p.id == id);
        if (existing != null)
        {
            existing.position = position;
            if (!string.IsNullOrEmpty(displayName))
                existing.displayName = displayName;
            if (!string.IsNullOrEmpty(sceneName))
                existing.sceneName = sceneName;
            return;
        }

        unlockedSavePoints.Add(new SavePointData
        {
            id = id,
            displayName = string.IsNullOrEmpty(displayName) ? id : displayName,
            position = position,
            sceneName = sceneName
        });
    }

    public System.Collections.Generic.IReadOnlyList<SavePointData> GetUnlockedSavePoints()
        => unlockedSavePoints;

    public void Save(Player player)
    {
        if (player == null || player.stats == null)
            return;

        var data = new SaveData
        {
            maxHealthBase = player.stats.resource.maxHealth.GetBaseValue(),
            damageBase = player.stats.offense.damage.GetBaseValue(),
            checkpointPosition = player.transform.position,
            checkpointScene = SceneManager.GetActiveScene().name,
            currency = CurrencyManager.instance != null ? CurrencyManager.instance.GetCurrency() : 0,
            unlockedSavePoints = new System.Collections.Generic.List<SavePointData>(unlockedSavePoints)
        };

        foreach (var kvp in player.abilityUnlocked)
        {
            if (kvp.Value)
                data.unlockedSkills.Add(kvp.Key);
        }

        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public bool Load(Player player)
    {
        if (player == null || player.stats == null)
            return false;

        if (!File.Exists(SavePath))
            return false;

        var json = File.ReadAllText(SavePath);
        var data = JsonUtility.FromJson<SaveData>(json);
        if (data == null)
            return false;

        if (!string.IsNullOrEmpty(data.checkpointScene) &&
            SceneManager.GetActiveScene().name != data.checkpointScene)
        {
            pendingRespawnData = data;
            SceneManager.LoadScene(data.checkpointScene);
            return true;
        }

        ApplySaveData(player, data);
        return true;
    }

    public bool TryRespawn(Player player)
    {
        if (player == null)
            return false;

        if (HasSave())
            return Load(player);

        if (!hasStartPoint)
            return false;

        player.transform.position = startPointPosition;
        player.health?.SetHealthToPercent(1f);
        Save(player);
        return true;
    }

    public bool HasPendingRespawn() => pendingRespawnData != null;

    public void ApplyPendingRespawn(Player player)
    {
        if (pendingRespawnData == null || player == null)
            return;

        ApplySaveData(player, pendingRespawnData);
        pendingRespawnData = null;
    }

    public void RequestTeleport(SavePointData point)
    {
        pendingTeleportPoint = point;
    }

    public bool HasPendingTeleport() => pendingTeleportPoint != null;

    public void ApplyPendingTeleport(Player player)
    {
        if (pendingTeleportPoint == null || player == null)
            return;

        player.transform.position = pendingTeleportPoint.position;
        Save(player);
        pendingTeleportPoint = null;
    }

    public bool HasSave() => File.Exists(SavePath);

    public bool HasStartPoint() => hasStartPoint;

    public void DeleteSave()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
    }

    private void ApplySaveData(Player player, SaveData data)
    {
        if (player.stats.resource != null && player.stats.resource.maxHealth != null)
            player.stats.resource.maxHealth.SetBaseValueAndNotify(data.maxHealthBase);
        if (player.stats.offense != null && player.stats.offense.damage != null)
            player.stats.offense.damage.SetBaseValueAndNotify(data.damageBase);

        foreach (var key in new System.Collections.Generic.List<SkillType>(player.abilityUnlocked.Keys))
            player.abilityUnlocked[key] = false;

        foreach (var skill in data.unlockedSkills)
            player.UnlockAbility(skill, null);

        if (CurrencyManager.instance != null)
            CurrencyManager.instance.SetCurrency(data.currency);

        unlockedSavePoints.Clear();
        if (data.unlockedSavePoints != null)
            unlockedSavePoints.AddRange(data.unlockedSavePoints);

        player.transform.position = data.checkpointPosition;
        player.health?.SetHealthToPercent(1f);
    }
}

