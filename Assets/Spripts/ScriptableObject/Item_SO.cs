using UnityEngine;


public enum ItemType { HEART, BATTERY, GEM, KEY, EMPTY };

[CreateAssetMenu(fileName = "NewItem", menuName = "Item", order = 1)]
public class Item_SO : ScriptableObject
{
    public string itemName = "New Item";
    public ItemType type = ItemType.EMPTY;
    public Material itemMaterial = null;
    public Sprite itemIcon = null;

}
