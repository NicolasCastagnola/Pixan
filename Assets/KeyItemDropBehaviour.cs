using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class KeyItemDropBehaviour : Interactable
{
    [SerializeField] KeyBehaviour requiredKey;
    [SerializeField] AltarHandler altar;
    [SerializeField] string keyName;

    private void Start()
    {
        if(PlayerPrefs.HasKey($"Souls/{keyName}"))
        {
            if(PlayerPrefs.GetInt($"Souls/{keyName}", 0) == 1)
            {
                foreach (var child in transform.GetAllChildren())
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
    }

    public override void OnExitOnReachableRadius()
    {
        Canvas_Playing.Instance.HideInteractDisplay();
    }

    public override void OnInteract()
    {
        if(componentWithinReach.PlayerComponent.InventoryComponent.InventoryContains(requiredKey._itemData))
        {
            foreach (var child in transform.GetAllChildren())
            {
                child.gameObject.SetActive(true);
            }
            altar.altarDrop(keyName);
        }
    }
}
