using UnityEngine;

public enum InventoryItemType {None, Key, }

[CreateAssetMenu(menuName = "Inventory Item")]
public class InventoryItemData : ScriptableObject
{
    public InventoryItemType itemType;
    public string id;
    public string itemName;
    public string Description;
    public int InventoryIndex=-1;
    public Sprite iconInsideInventory;
    [Range(1, 20)] public int maxStacks;
    [Range(1, 20)] public int minStacks;
    public GameObject objectPrefab;
    public void AddToInventory() => InventorySystem.Instance.Add(this);
}
