using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerStatue : MonoBehaviour
{
    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            return;

        transform.LookAt(player);
        Vector3 rot = transform.eulerAngles;
        rot.x = 0;
        rot.z = 0;
        transform.eulerAngles = rot;
    }
}
