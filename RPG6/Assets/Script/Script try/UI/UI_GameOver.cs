using UnityEngine;
using UnityEngine.UI;

public class UI_GameOver : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        if (root == null)
            root = gameObject;

        if (quitButton != null)
            quitButton.onClick.AddListener(Quit);

        root.SetActive(false);
    }

    public void Show()
    {
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

    private void Quit()
    {
        Time.timeScale = 1f;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
