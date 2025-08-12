using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDRotator : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    private void Update()
    {
        gameObject.transform.Rotate(new Vector3(0, 0, rotationSpeed) * Time.deltaTime);
    }
}
