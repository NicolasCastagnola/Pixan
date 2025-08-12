using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class InteractComponent : MonoBehaviour
{
    [ShowInInspector, ReadOnly] private Interactable _interactableWithinRadius;
    [field:SerializeField] public Player PlayerComponent { get; private set; }
    
    [SerializeField] private CapsuleCollider capsuleCollider;

    private void Start()
    {
        PlayerComponent ??= GetComponentInParent<Player>();
        capsuleCollider ??= GetComponent<CapsuleCollider>();
    }
    private void Update()
    {
        if (_interactableWithinRadius == null) return;
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            _interactableWithinRadius.OnInteract();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Interactable>(out var interactable)) return;

        _interactableWithinRadius = interactable;
        _interactableWithinRadius.OnEnterReachableRadius(this);
            
        Canvas_Playing.Instance.ShowInteractDisplay(_interactableWithinRadius.OnHoverMessage);
    }
    private void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent<Interactable>(out var interactable) || Canvas_Playing.InBonfireMenu || Canvas_Playing.InDialogue) return;

        Canvas_Playing.Instance.ShowInteractDisplay(interactable.OnHoverMessage);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Interactable>(out var interactable)) return;
        
        interactable.OnExitOnReachableRadius();
        
        Canvas_Playing.Instance.HideInteractDisplay();

        _interactableWithinRadius = null;
    }
    private void OnDrawGizmos()
    {
        var oldMatrix = capsuleCollider.transform.localToWorldMatrix;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + capsuleCollider.center, capsuleCollider.bounds.size);;
        Gizmos.matrix = oldMatrix;
    }
    
}
