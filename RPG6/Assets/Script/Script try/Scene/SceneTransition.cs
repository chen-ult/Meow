using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string targetSceneName;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool showBossHealthBarOnLoad;
    [SerializeField] private string bossSpawnPointId = "BossEntry";
    [Header("Float")]
    [SerializeField] private float floatAmplitude = 0.25f;
    [SerializeField] private float floatSpeed = 2f;

    private Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (floatAmplitude <= 0f || floatSpeed <= 0f)
            return;

        float offset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = startPosition + new Vector3(0f, offset, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(playerTag))
            return;

        if (showBossHealthBarOnLoad && SaveManager.Instance != null)
            SaveManager.Instance.RequestShowBossHealthBar();

        if (!string.IsNullOrEmpty(bossSpawnPointId) && SaveManager.Instance != null)
            SaveManager.Instance.RequestSpawnPoint(bossSpawnPointId);

        if (!string.IsNullOrEmpty(targetSceneName))
            SceneManager.LoadScene(targetSceneName);
    }
}
