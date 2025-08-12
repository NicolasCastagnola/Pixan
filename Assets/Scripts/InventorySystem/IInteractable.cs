using UnityEngine;

namespace Interfaces
{
    public interface IInteractable
    {
        void OnInteract();
        void OnEnterReachableRadius(InteractComponent component);
        void OnExitOnReachableRadius();
    }
}
