using UnityEngine;

[System.Serializable]
public class Inventory_Item
{
    public ItemDataSo itemData;

    public Inventory_Item(ItemDataSo itemData)
    {
        this.itemData = itemData;
    }
}
