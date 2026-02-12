using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Shop : MonoBehaviour
{
    public static UI_Shop Instance;

    public NPC_Vendor currentVendor;

    public GameObject panel;
    public Image itemIcon;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI ItemDescriptionText;

    private TradeItem[] currentItems;
    private int currentIndex;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    private void Update()
    {
        if (panel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }

    public void OpenShop(TradeItem[] items, NPC_Vendor vendor)
    {
        currentVendor = vendor;
        currentItems = items;
        currentIndex = 0;
        panel.SetActive(true);
        RefreshUI();
    }

    public void CloseShop()
    {
        panel.SetActive(false);
        currentVendor.OnShopClosed();
    }

    void RefreshUI()
    {
        if (currentItems == null || currentItems.Length == 0)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
            priceText.text = "";
            ItemDescriptionText.text = "";
            currencyText.text = "Your dots: " + CurrencyManager.instance.currentCurrency;
            return;

        }

        var item = currentItems[currentIndex];
        itemIcon.sprite = item.itemIcon;
        itemIcon.enabled = item.itemIcon != null;
        ItemDescriptionText.text = item.description;
        priceText.text = "Price: " + item.price;
        currencyText.text = "Your dots: " + CurrencyManager.instance.currentCurrency;
    }

    public void Buy()
    {
        if (currentItems == null || currentItems.Length == 0) return;

        var item = currentItems[currentIndex];
        bool success = CurrencyManager.instance.RemoveCurrency(item.price);

        if (success && item.itemPrefab != null)
        {
            Vector3 pos = GameObject.FindWithTag("Player").transform.position + Vector3.right;
            Instantiate(item.itemPrefab, pos, Quaternion.identity);
            RefreshUI();
        }
    }

    public void NextItem()
    {
        currentIndex++;
        if (currentIndex >= currentItems.Length) currentIndex = 0;
        RefreshUI();
    }

    public void PrevItem()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = currentItems.Length - 1;
        RefreshUI();
    }
}
