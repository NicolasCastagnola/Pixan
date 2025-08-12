using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class ItemObject : Interactable
{
    [ShowInInspector, ReadOnly] public string ID => _itemData.itemName;
    
    public InventoryItemData _itemData;

    [SerializeField] int index;
    [SerializeField] string titleText;

    public void Start()
    {
        if (PlayerPrefs.GetInt(ID, 0) == 1)
            StartCoroutine(LoadItem());
    }

    public override void OnInteract()
    {
        Canvas_Inventory.Instance.ShowSlotItem(index);
        Canvas_Playing.Instance.OnGetGreenOrb($"{titleText} added");

        PlayerPrefs.SetInt(ID, 1);

        Canvas_Playing.Instance.HideInteractDisplay();
        
        _itemData.AddToInventory();
        
        gameObject.SetActive(false);
    }
    public override void OnExitOnReachableRadius()
    {
        Canvas_Playing.Instance.HideInteractDisplay();
    }
    IEnumerator LoadItem()
    {
        yield return new WaitUntil(()=> Canvas_Inventory.Instance!=null);
        Canvas_Inventory.Instance.ShowSlotItem(index);
        _itemData.AddToInventory();
        gameObject.SetActive(false);
    }
}
