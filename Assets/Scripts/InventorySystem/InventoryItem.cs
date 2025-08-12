using Sirenix.OdinInspector;
using UnityEngine;
public class InventoryItem : MonoBehaviour
{
    [ShowInInspector, ReadOnly] public InventoryItemData ItemData { get; private set;}
    [ShowInInspector, ReadOnly] public int CurrentStackSize { get; private set; }
    [ShowInInspector, ReadOnly] public int MaxStack { get; private set; }
    [ShowInInspector, ReadOnly] public int MinStack { get; private set; }
    public InventoryItem SetData(InventoryItemData dataSource)
    {
        ItemData = dataSource;
        MaxStack = dataSource.maxStacks;
        MinStack = dataSource.minStacks;
        
        AddToStack();

        return this;
    }
    
    public void AddToStack() => CurrentStackSize++;
    public void RemoveFromStack() => CurrentStackSize--;
}