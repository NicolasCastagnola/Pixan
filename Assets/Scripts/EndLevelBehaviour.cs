using System.Text;
using UnityEngine;
public class EndLevelBehaviour : Interactable
{
    [SerializeField] private string hasAllRequirementsMessage;
    [SerializeField] private InventoryItemData[] requiredItems;

    [SerializeField] private bool force = true; 

    public override void OnEnterReachableRadius(InteractComponent component)
    {
        base.OnEnterReachableRadius(component);

        // OnHoverMessage = MessageBasedOnItems();
    }

    private string MessageBasedOnItems()
    {
        if (ValidateAllRequiredItems())
        {
            return hasAllRequirementsMessage;
        }
                
        var sb = new StringBuilder();

        foreach (var item in requiredItems)
        {
            sb.Append(item.itemName);

            if (requiredItems.Length > 1)
            {
                sb.Append(" & ");  
            }
        }

        var result = sb.ToString();
                    
        return $"Requires {result} to open";
    }
    
    private bool ValidateAllRequiredItems()
    {
        var matchAllItems = false;

        if (requiredItems.Length == 0) return true;
        
        foreach (var item in requiredItems)
        {
            if (!componentWithinReach.PlayerComponent.InventoryComponent.InventoryContains(item))
            {
                return false;
            }
            
            matchAllItems = true;
        }

        return matchAllItems;
    }

    public override void OnInteract()
    {
        if (force)
        {
            LevelController.Instance.Win();
        }
        
        if (ValidateAllRequiredItems())
        {
            LevelController.Instance.Win();
        }
    }
    public override void OnExitOnReachableRadius()
    {
    }
}
