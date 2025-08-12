using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHeadBehaviour : MonoBehaviour
{
    [SerializeField] GameObject eyes;
    [SerializeField] Material offMaterial;
    
    public void TurnEyesOff()
    {
        eyes.GetComponent<Renderer>().material = offMaterial;
    }
}
