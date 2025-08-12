using System;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UI_MessageOnTriggerEvents : MonoBehaviour
{
    [ShowInInspector, ReadOnly] private Collider _collider;

    [SerializeField] private bool deactivateColliderOnTriggerExit = true;
    
    [SerializeField] private LayerMask layersMaskToTrigger;
    [SerializeField, Title("Message")] private string messageOnTriggerEnter;
    [SerializeField, BoxGroup("Duration Times")] private float fadeInDuration = 1f;
    [SerializeField, BoxGroup("Duration Times")] private float displayDuration = 1f;
    [SerializeField, BoxGroup("Duration Times")] private float fadeOutDuration = 1f;

    private void Start() => _collider ??= GetComponent<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if (layersMaskToTrigger != (layersMaskToTrigger | 1 << other.gameObject.layer)) return;
            
        Canvas_Playing.Instance.UIFader.ShowText(messageOnTriggerEnter, fadeInDuration, displayDuration, fadeOutDuration);

        GameManager.ShowLog(other.ToString());
    }

    private void OnTriggerExit(Collider other)
    {
        if (deactivateColliderOnTriggerExit)
            _collider.enabled = false;
    }
}
