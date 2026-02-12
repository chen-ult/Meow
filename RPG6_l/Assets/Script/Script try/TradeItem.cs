using UnityEngine;

[System.Serializable]
public class TradeItem
{
    public string itemName;
    public int price;
    public GameObject itemPrefab;
    public Sprite itemIcon;

    [TextArea(2, 4)]
    public string description;
}
