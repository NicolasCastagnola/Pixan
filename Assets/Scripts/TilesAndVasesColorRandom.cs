using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesAndVasesColorRandom : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Renderer>().material.color = Random.ColorHSV(0.05f,0.07f, 0.5f,0.7f,0.5f,1f); 
        transform.localScale = transform.localScale * Random.Range(0.9f, 1.1f);
        Destroy(this);
    }
}
