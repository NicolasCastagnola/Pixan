using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayAreaName : MonoBehaviour
{
    [SerializeField] private string areaName;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Player>(out var player)) return;
        
        Canvas_Playing.Instance.ShowAreaTitle(areaName, FinishedShowing);
    }
    private void FinishedShowing() => GetComponent<Collider>().enabled = false;
}
