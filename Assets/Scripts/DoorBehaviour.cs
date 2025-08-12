using System;
using System.Text;
using Audio;
using Sirenix.OdinInspector;
using UnityEngine;

public enum DoorState { None, Unlocked, Locked, ItemLocked,Disable}

public class DoorBehaviour : Interactable
{
    [ShowInInspector, ReadOnly] private bool IsOpen;
    [ShowInInspector, ReadOnly] private Collider _collider;
    [SerializeField] private bool shouldRemainPermaOpen;
    
    [SerializeField, TabGroup("Interact")] private string lockedMessage = "This door is locked";
    [SerializeField, TabGroup("Interact")] private string openDoorMessage = "Open door";
    [SerializeField, TabGroup("Interact")] private string closeDoorMessage = "Close door";
    [SerializeField, TabGroup("Interact")] private DoorState doorState = DoorState.Unlocked;
    [SerializeField, TabGroup("Interact")] private bool shouldDisappearItems = false;
    [SerializeField, ShowIf(nameof(doorState), DoorState.ItemLocked)] private InventoryItemData[] requiredItemsToOpenDoor;
    
    [SerializeField, TabGroup("Animation")] private Animation doorAnimations;
    [SerializeField, TabGroup("Animation")] private string openAnimationName;
    [SerializeField, TabGroup("Animation")] private string closeAnimationName;

    [SerializeField, TabGroup("Sound")] private AudioConfigurationData closeSoundEffect;
    [SerializeField, TabGroup("Sound")] private AudioConfigurationData openSoundEffect;
    [SerializeField, TabGroup("Sound")] private AudioConfigurationData lockedSoundEffect;

    private void Start() => _collider = GetComponent<Collider>();
    private void TryOpenDoor()
    {
        switch (doorState)
        {
            case DoorState.None: goto case DoorState.Locked;
                
            case DoorState.Unlocked:
                if (IsOpen) CloseDoor();
                else OpenDoor();
                break;
            case DoorState.Locked:
                LockedDoor();
                break;
            case DoorState.ItemLocked:
                if (ValidateAllRequiredItems())
                {
                    OpenDoor();

                    if (shouldDisappearItems)
                    {
                        foreach (var item in requiredItemsToOpenDoor)
                        {
                            if (Canvas_Inventory.Instance != null)
                                Canvas_Inventory.Instance.HideSlotItem(item.InventoryIndex);
                        }
                    }

                    
                }
                else
                    LockedDoor();
                break;
            case DoorState.Disable:
                    break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool ValidateAllRequiredItems()
    {
        var matchAllItems = false;

        if (requiredItemsToOpenDoor.Length == 0) return true;
        
        foreach (var item in requiredItemsToOpenDoor)
        {
            if (!componentWithinReach.PlayerComponent.InventoryComponent.InventoryContains(item))
            {
                return false;
            }
            
            matchAllItems = true;
        }

        return matchAllItems;
    }
    
    private void LockedDoor()
    {
        lockedSoundEffect.Play3D();
        //TODO: locked anim
        // doorAnimations.Play("Door_Open");
    }
    public void OpenDoor()
    {
        openSoundEffect.Play2D();
        doorAnimations.Play(openAnimationName);
        IsOpen = true;

        if (shouldRemainPermaOpen)
        {
            _collider.enabled = false;

            if(Canvas_Playing.Instance!=null)
                Canvas_Playing.Instance.HideInteractDisplay();
        }
    }
    private void CloseDoor()
    {
        closeSoundEffect.Play3D();
        doorAnimations.Play(closeAnimationName);
        IsOpen = false;
    }

    public override void OnInteract()
    {
        if (shouldRemainPermaOpen && IsOpen) return;

        TryOpenDoor();
    }
    private string DoorMessageBasedOnState()
    {
        switch (doorState)
        {

            case DoorState.None: goto case DoorState.Locked;
                
            case DoorState.Unlocked: return IsOpen ? closeDoorMessage : openDoorMessage;
                
            case DoorState.Locked: return lockedMessage;
            
            case DoorState.ItemLocked:

                if (ValidateAllRequiredItems())
                {
                    return openDoorMessage;
                }
                
                var sb = new StringBuilder();

                foreach (var item in requiredItemsToOpenDoor)
                {
                    sb.Append(item.itemName);

                    if (requiredItemsToOpenDoor.Length > 1)
                    {
                        sb.Append(" & ");  
                    }
                }

                var result = sb.ToString();
                    
                return $"Requires {result} to open";
            case DoorState.Disable: goto case DoorState.Locked;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    public override void OnEnterReachableRadius(InteractComponent component)
    {
        base.OnEnterReachableRadius(component);
        
        if (shouldRemainPermaOpen && IsOpen) return;
        
        OnHoverMessage = DoorMessageBasedOnState();
        
    }
    public override void OnExitOnReachableRadius()
    {
        if (shouldRemainPermaOpen && IsOpen) return;
        
        OnHoverMessage = DoorMessageBasedOnState();
    }
}
