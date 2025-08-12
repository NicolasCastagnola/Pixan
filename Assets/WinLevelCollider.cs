using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLevelCollider : MonoBehaviour
{
    public void WinLevel() => LevelController.Instance.Win();

    private void OnCollisionEnter(Collision collision)
    {
        var collided = collision.gameObject.GetComponent<Player>();

        if(collided != null)
        {
            WinLevel();
        }
    }
}
