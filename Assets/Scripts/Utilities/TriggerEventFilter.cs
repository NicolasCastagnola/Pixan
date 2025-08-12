using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))] 
public class TriggerEventFilter : BaseEventFilter
{
    [SerializeField] private string searchingTag;

    //public UnityEvent onTriggerExit;

    public override void OnTrigger()
    {
        onTrigger?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(searchingTag)) OnTrigger();
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag(tag)) onTriggerExit?.Invoke();
    //}
}
