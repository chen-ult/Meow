using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_DeathScreen : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Button respawnButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.35f;
    [SerializeField] private float shownAlpha = 0.6f;
    [SerializeField] private TextMeshProUGUI noSaveText;

    private Player player;
    private Coroutine fadeCo;

    private void Awake()
    {
        if (root == null)
            root = gameObject;

        if (respawnButton != null)
            respawnButton.onClick.AddListener(OnRespawnClicked);
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);

        if (fadeImage != null)
        {
            var color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;
        }

        if (noSaveText != null)
            noSaveText.gameObject.SetActive(false);

        root.SetActive(false);
    }

    public void Show(Player player)
    {
        this.player = player;
        bool canRespawn = SaveManager.Instance != null &&
            (SaveManager.Instance.HasSave() || SaveManager.Instance.HasStartPoint());
        if (respawnButton != null)
            respawnButton.interactable = canRespawn;
        if (noSaveText != null)
            noSaveText.gameObject.SetActive(!canRespawn);

        if (root != null)
            root.SetActive(true);

        Time.timeScale = 0f;

        StartFade(shownAlpha);
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);

        Time.timeScale = 1f;
    }

    private void OnRespawnClicked()
    {
        if (player == null)
            return;

        if (fadeCo != null)
            StopCoroutine(fadeCo);

        if (respawnButton != null)
            respawnButton.interactable = false;
        if (quitButton != null)
            quitButton.interactable = false;

        fadeCo = StartCoroutine(RespawnRoutine());
    }

    private void OnQuitClicked()
    {
        Debug.Log("Quit clicked");
        Time.timeScale = 1f;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void StartFade(float targetAlpha)
    {
        if (fadeImage == null)
            return;

        if (fadeCo != null)
            StopCoroutine(fadeCo);

        fadeCo = StartCoroutine(FadeTo(targetAlpha));
    }

    private System.Collections.IEnumerator RespawnRoutine()
    {
        if (fadeImage != null)
            yield return FadeTo(1f);

        bool respawned = player != null && player.RespawnFromSave();
        if (respawned)
        {
            if (fadeImage != null)
                yield return FadeTo(0f);
            Hide();
        }
        else
        {
            bool canRespawn = SaveManager.Instance != null &&
                (SaveManager.Instance.HasSave() || SaveManager.Instance.HasStartPoint());
            if (respawnButton != null)
                respawnButton.interactable = canRespawn;
            if (quitButton != null)
                quitButton.interactable = true;
            if (noSaveText != null)
                noSaveText.gameObject.SetActive(!canRespawn);
            if (fadeImage != null)
                yield return FadeTo(shownAlpha);
        }
    }

    private System.Collections.IEnumerator FadeTo(float targetAlpha)
    {
        if (fadeImage == null)
            yield break;

        float startAlpha = fadeImage.color.a;
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, fadeDuration);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            var color = fadeImage.color;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            fadeImage.color = color;
            yield return null;
        }

        var finalColor = fadeImage.color;
        finalColor.a = targetAlpha;
        fadeImage.color = finalColor;
    }
}
