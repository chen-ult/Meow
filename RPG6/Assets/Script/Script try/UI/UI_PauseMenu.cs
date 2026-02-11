using UnityEngine;
using UnityEngine.UI;

public class UI_PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;

    private bool isOpen;

    private void Awake()
    {
        if (root == null)
            root = gameObject;

        if (resumeButton != null)
            resumeButton.onClick.AddListener(Resume);
        if (quitButton != null)
            quitButton.onClick.AddListener(Quit);

        root.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Toggle();
    }

    public void Toggle()
    {
        if (isOpen)
            Hide();
        else
            Show();
    }

    public void Show()
    {
        isOpen = true;
        if (root != null)
            root.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Hide()
    {
        isOpen = false;
        if (root != null)
            root.SetActive(false);
        Time.timeScale = 1f;
    }

    private void Resume()
    {
        Hide();
    }

    private void Quit()
    {
        Time.timeScale = 1f;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
