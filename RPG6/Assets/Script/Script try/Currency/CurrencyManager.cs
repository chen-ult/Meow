using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance;

    public int currentCurrency;

    public event System.Action <int> OnCurrencyChanged;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCurrency(int amount)
    {
        if(amount > 0)
        {
            currentCurrency += amount;
            OnCurrencyChanged?.Invoke(currentCurrency);
            Debug.Log("获得" + amount + "光点，当前" + currentCurrency + "光点");
        }
    }

    public bool RemoveCurrency(int amount)
    {
        if (amount > 0 && currentCurrency >= amount)
        {
            currentCurrency -= amount;
            OnCurrencyChanged?.Invoke(currentCurrency);
            Debug.Log("扣除" + amount + "光点，当前" + currentCurrency + "光点");
            return true;
        }
        Debug.Log("货币不足");
        return false;
    }

    public int GetCurrency() => currentCurrency;

    public void SetCurrency(int amount)
    {
        currentCurrency = Mathf.Max(0, amount);
        OnCurrencyChanged?.Invoke(currentCurrency);
    }

}
