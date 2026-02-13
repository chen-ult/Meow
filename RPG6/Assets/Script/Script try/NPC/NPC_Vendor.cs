using UnityEngine;

public class NPC_Vendor : MonoBehaviour
{
    [Header("对话")]
    public DialogueData dialogue;

    [Header("商店")]
    public TradeItem[] shopItems;

    [Header("互动")]
    public GameObject interactIcon;
    public float interactRange = 2f;
    public KeyCode interactKey = KeyCode.E;

    [Header("选择按钮（最后一句显示）")]
    public GameObject btnChoiceGroup;   // 按钮父物体
    public UnityEngine.UI.Button btnOpenShop;
    public UnityEngine.UI.Button btnExitTalk;

    private Player playerController;
    private Transform player;
    private bool inRange;
    private bool isTalking;
    private int currentSentence;

    // 标记：是否到了最后一句（必须点按钮）
    private bool waitForChoice;

    private void Start()
    {
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
        playerController = playerObject != null ? playerObject.GetComponent<Player>() : null;
        // 绑定按钮
        if (btnOpenShop != null)
            btnOpenShop.onClick.AddListener(OpenShopAndClose);
        if (btnExitTalk != null)
            btnExitTalk.onClick.AddListener(OnlyCloseTalk);
    }

    private void Update()
    {
        if (player == null)
            return;

        inRange = Vector2.Distance(transform.position, player.position) < interactRange;

        if (inRange && Input.GetKeyDown(interactKey))
        {
            // 重点：正在等待按钮选择时，E 无效！
            if (waitForChoice)
                return;

            if (!isTalking)
                StartTalk();
            else
                NextSentence();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (interactIcon != null)
                interactIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (interactIcon != null)
                interactIcon.SetActive(false);
        }
    }

    void StartTalk()
    {
        isTalking = true;
        waitForChoice = false;  // 重置选择状态
        currentSentence = 0;
        ShowCurrentSentence();

        if (btnChoiceGroup != null)
            btnChoiceGroup.SetActive(false);

        if(playerController != null)
            playerController.enabled = false;
    }

    void NextSentence()
    {
        if (dialogue == null || dialogue.sentences == null || dialogue.sentences.Length == 0)
            return;

        currentSentence++;

        // 判断：是不是最后一句
        if (currentSentence >= dialogue.sentences.Length)
        {
            ShowLastSentenceAndWaitChoice();
            return;
        }

        ShowCurrentSentence();
    }

    void ShowCurrentSentence()
    {
        if (dialogue == null || dialogue.sentences == null || dialogue.sentences.Length == 0)
            return;

        if (currentSentence < 0 || currentSentence >= dialogue.sentences.Length)
            return;

        if (UI_Dialogue.Instance == null)
            return;

        string text = dialogue.sentences[currentSentence];
        UI_Dialogue.Instance.Show(text);
    }

    // 最后一句：显示文字 + 显示按钮 + 禁用 E
    void ShowLastSentenceAndWaitChoice()
    {
        waitForChoice = true;       // E 失效
        isTalking = false;

        // 显示最后一句
        string lastText = dialogue.sentences[dialogue.sentences.Length - 1];
        UI_Dialogue.Instance.Show(lastText);

        // 显示按钮
        if (btnChoiceGroup != null)
            btnChoiceGroup.SetActive(true);
    }

    // 选择：打开商店
    void OpenShopAndClose()
    {
        UI_Dialogue.Instance?.Hide();
        if (btnChoiceGroup != null) btnChoiceGroup.SetActive(false);
        UI_Shop.Instance?.OpenShop(shopItems, this);
        waitForChoice = false;
        SetPlayerController(false);
    }

    // 选择：只关闭对话
    void OnlyCloseTalk()
    {
        UI_Dialogue.Instance?.Hide();
        if (btnChoiceGroup != null) btnChoiceGroup.SetActive(false);
        waitForChoice = false;
        SetPlayerController(true);
    }

    
    void SetPlayerController(bool enable)
    {
        if(playerController != null)
            playerController.enabled = enable;
    }
    public void OnShopClosed()
    {
        SetPlayerController(true);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
