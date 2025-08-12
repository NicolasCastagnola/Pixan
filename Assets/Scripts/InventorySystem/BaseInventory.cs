using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class BaseInventory : BaseMonoSingleton<BaseInventory>
{
    public event Action<InventoryItemData> OnItemAdded, OnItemRemoved;
    
    [ShowInInspector, ReadOnly] private readonly Dictionary<InventoryItemData, InventoryItem> _itemDictionary = new Dictionary<InventoryItemData, InventoryItem>();
    [ShowInInspector, ReadOnly] public List<InventoryItem> CurrentInventoryItems { get; private set; } = new List<InventoryItem>();
    public bool InventoryContains(InventoryItemData item) => CurrentInventoryItems.FirstOrDefault(i => i.ItemData == item) != default;
    public InventoryItem Get(InventoryItemData itemToSearchReferenceData) => _itemDictionary.TryGetValue(itemToSearchReferenceData, out var value) ? value : null;
    public InventoryItem Get(string itemToSearchName) => _itemDictionary.Values.FirstOrDefault(i => i.ItemData.itemName == itemToSearchName);
    public void Add(InventoryItemData referenceData)
    {
        if (_itemDictionary.TryGetValue(referenceData, out var value))
        {
            if (value.CurrentStackSize < value.MaxStack)
            {
                value.AddToStack();
            }
        }
        else
        {
            var i = Instantiate(new GameObject($"ITEM ID:{referenceData}"), transform)
                   .AddComponent<InventoryItem>()
                   .SetData(referenceData);
            
            AddItemToInventory(referenceData, i);
        }
        
        OnItemAdded?.Invoke(referenceData);
    }
    
    public void Remove(InventoryItemData referenceData)
    {
        if (_itemDictionary.TryGetValue(referenceData, out var value))
        {
            value.RemoveFromStack();

            if (value.CurrentStackSize == value.MinStack)
            {
                RemoveItemFromInventory(referenceData, value);
            }
        }
        
        OnItemRemoved?.Invoke(referenceData);
    }
    private void RemoveItemFromInventory(InventoryItemData referenceData, InventoryItem itemToRemove)
    {
        CurrentInventoryItems.Remove(itemToRemove);
        _itemDictionary.Remove(referenceData);
    }
    private void AddItemToInventory(InventoryItemData referenceData, InventoryItem itemToAdd)
    {
        CurrentInventoryItems.Add(itemToAdd);
        _itemDictionary.Add(referenceData, itemToAdd);
    }
}
