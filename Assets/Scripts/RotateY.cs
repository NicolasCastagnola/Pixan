using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateY : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles += Vector3.up;
    }
}
