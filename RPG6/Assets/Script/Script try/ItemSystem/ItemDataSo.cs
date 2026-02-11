using UnityEngine;

[CreateAssetMenu(menuName = "游戏设置/物品数据", fileName = "默认物品数据 -- ")]
public class ItemDataSo : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
}
