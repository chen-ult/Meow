using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UI_PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;

    private bool isOpen;
    private InputAction pauseAction;

    private void Awake()
    {
        if (root == null)
            root = gameObject;

        if (resumeButton != null)
            resumeButton.onClick.AddListener(Resume);
        if (quitButton != null)
            quitButton.onClick.AddListener(Quit);

        pauseAction = new InputAction("Pause", binding: "<Keyboard>/escape");

        if (root != gameObject)
            root.SetActive(false);
    }

    private void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.performed += OnPausePerformed;
            pauseAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.performed -= OnPausePerformed;
            pauseAction.Disable();
        }
    }

    private void Update()
    {
        bool escapePressed = (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            || Input.GetKeyDown(KeyCode.Escape);

        if (escapePressed)
            Toggle();
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
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
            root.transform.SetAsLastSibling();
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
