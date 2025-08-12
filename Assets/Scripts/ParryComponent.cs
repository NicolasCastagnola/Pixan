using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryComponent : MonoBehaviour
{
    [SerializeField] private Collider _collider;
    
    public bool CanParry { get; private set; }

    public void SetParryState(bool value) => CanParry = value;

}
