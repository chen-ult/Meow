using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SavePointMenu : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private Button closeButton;

    private Player player;
    private readonly List<Button> spawnedButtons = new List<Button>();

    private void Awake()
    {
        if (root == null)
            root = gameObject;

        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);

        root.SetActive(false);
    }

    public void Show(Player player, System.Collections.Generic.IReadOnlyList<SavePointData> savePoints)
    {
        this.player = player;
        BuildButtons(savePoints);

        if (root != null)
            root.SetActive(true);

        Time.timeScale = 0f;
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);

        Time.timeScale = 1f;
    }

    private void BuildButtons(System.Collections.Generic.IReadOnlyList<SavePointData> savePoints)
    {
        foreach (var button in spawnedButtons)
        {
            if (button != null)
                Destroy(button.gameObject);
        }
        spawnedButtons.Clear();

        if (buttonPrefab == null || buttonContainer == null || savePoints == null)
            return;

        foreach (var point in savePoints)
        {
            var button = Instantiate(buttonPrefab, buttonContainer);
            var label = button.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.text = string.IsNullOrEmpty(point.displayName) ? point.id : point.displayName;

            button.onClick.AddListener(() => TeleportTo(point));
            spawnedButtons.Add(button);
        }
    }

    private void TeleportTo(SavePointData point)
    {
        if (player == null)
            return;

        if (SaveManager.Instance == null)
            return;

        var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (!string.IsNullOrEmpty(point.sceneName) && point.sceneName != currentScene)
        {
            SaveManager.Instance.RequestTeleport(point);
            UnityEngine.SceneManagement.SceneManager.LoadScene(point.sceneName);
            Hide();
            return;
        }

        player.transform.position = point.position;
        SaveManager.Instance.Save(player);
        Hide();
    }
}
