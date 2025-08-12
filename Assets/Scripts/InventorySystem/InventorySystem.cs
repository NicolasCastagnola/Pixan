using System;
using UnityEngine;

public class InventorySystem : BaseInventory
{
    private int currentIndex = 0;
    
    public void CycleCurrentItems()
    {
        if (CurrentInventoryItems.Count == 0) return;
        
        CurrentInventoryItems[currentIndex].ItemData.objectPrefab.SetActive(false);
        currentIndex = (currentIndex + 1) % CurrentInventoryItems.Count;
        CurrentInventoryItems[currentIndex].ItemData.objectPrefab.SetActive(true);
    }
}
