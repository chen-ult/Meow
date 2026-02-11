using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Inventory_Base : MonoBehaviour
{
    public int maxInventorySize = 10;
    [SerializeField] public List<Inventory_Item> itemList = new List<Inventory_Item>();

    public bool CanAddItem() => itemList.Count < maxInventorySize;

    public void AddItem(Inventory_Item itemToAdd)
    {
        itemList.Add(itemToAdd);
    }

}
