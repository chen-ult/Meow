using TMPro;
using UnityEngine;

public class UI_Dialogue : MonoBehaviour
{
    public static UI_Dialogue Instance;

    public GameObject panel;
    public TextMeshProUGUI text;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(string content)
    {
        panel.SetActive(true);
        text.text = content;
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}
