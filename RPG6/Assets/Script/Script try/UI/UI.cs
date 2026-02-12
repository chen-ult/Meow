using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public static UI Instance { get; private set; }
    public UI_SkillToolTip skillToolTip;
    public UI_SkillTree skillTree;
    public UI_DeathScreen deathScreen;
    public UI_PauseMenu pauseMenu;
    [SerializeField] private GameObject bossHealthBarRoot;
    public UI_SavePointMenu savePointMenu;

    public UI_InGame inGameUI { get; private set; }

    private bool skillTreeEnabled;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        skillToolTip = GetComponentInChildren<UI_SkillToolTip>();
        skillTree = GetComponentInChildren<UI_SkillTree>(true);
        inGameUI = GetComponentInChildren<UI_InGame>(true);
        deathScreen = GetComponentInChildren<UI_DeathScreen>(true);
        pauseMenu = GetComponentInChildren<UI_PauseMenu>(true);
        savePointMenu = GetComponentInChildren<UI_SavePointMenu>(true);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    public void ToggleSkillTreeUI()
    {
        skillTreeEnabled = !skillTreeEnabled;
        skillTree.gameObject.SetActive(skillTreeEnabled);
        skillToolTip.ShowToolTip(false, null);
    }

    public void ShowDeathScreen(Player player)
    {
        deathScreen?.Show(player);
    }

    public void HideDeathScreen()
    {
        deathScreen?.Hide();
    }

    public void ShowSavePointMenu(Player player)
    {
        if (SaveManager.Instance == null)
            return;

        savePointMenu?.Show(player, SaveManager.Instance.GetUnlockedSavePoints());
    }

    public void ShowBossHealthBar(bool show)
    {
        if (bossHealthBarRoot != null)
            bossHealthBarRoot.SetActive(show);

        // trigger boss music when boss bar shows
        if (show)
            AudioManager.Instance?.PlayBossMusic(true);
        else
            AudioManager.Instance?.PlayBossMusic(false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SaveManager.Instance != null && SaveManager.Instance.PendingShowBossHealthBar)
        {
            ShowBossHealthBar(true);
            SaveManager.Instance.ClearShowBossHealthBar();
        }
        else
        {
            ShowBossHealthBar(false);
        }
    }
}
