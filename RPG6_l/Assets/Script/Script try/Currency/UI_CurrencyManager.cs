using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UI_CurrencyManager : MonoBehaviour
{
    private TextMeshProUGUI currencyText;

    private void Awake()
    {
        currencyText = GetComponentInChildren<TextMeshProUGUI>();

        if (currencyText == null)
            Debug.Log("Î´ÕÒµ½TMP");
    }

    private void Start()
    {
        RegisterToCurrencyManager();
    }

    private void RegisterToCurrencyManager()
    {
        if (CurrencyManager.instance != null)
        {
            Debug.Log("ÒÑ¼àÌý");
            CurrencyManager.instance.OnCurrencyChanged += UpdateCurrencyText;
            UpdateCurrencyText(CurrencyManager.instance.GetCurrency());
        }
    }


    private void OnDisable()
    {
        if(CurrencyManager.instance != null )
        {
            CurrencyManager.instance.OnCurrencyChanged -= UpdateCurrencyText;
        }
    }

    private void UpdateCurrencyText(int currentNum)
    {
        if(currencyText != null) 
        currencyText.text = currentNum.ToString();
    }
}
