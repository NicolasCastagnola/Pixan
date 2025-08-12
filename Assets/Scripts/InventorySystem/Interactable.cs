using Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Interactable : MonoBehaviour, IInteractable
{
    [ShowInInspector, ReadOnly] protected InteractComponent componentWithinReach;
    [field:SerializeField] public string OnHoverMessage  { get; protected set;  } = "press 'E' to Interact";
    public abstract void OnInteract();
    public virtual void OnEnterReachableRadius(InteractComponent component) => componentWithinReach = component;
    public abstract void OnExitOnReachableRadius();
}
