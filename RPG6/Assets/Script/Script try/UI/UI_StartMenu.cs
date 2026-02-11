using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_StartMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private Button continueButton;

    private void Start()
    {
        if (continueButton != null)
        {
            bool hasSave = SaveManager.Instance != null && SaveManager.Instance.HasSave();
            continueButton.interactable = hasSave;
        }
    }

    public void StartNewGame()
    {
        SaveManager.Instance?.BeginNewGame();
        SceneManager.LoadScene(gameSceneName);
    }

    public void ContinueGame()
    {
        if (SaveManager.Instance == null || !SaveManager.Instance.HasSave())
            return;

        SaveManager.Instance.BeginContinue();
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
