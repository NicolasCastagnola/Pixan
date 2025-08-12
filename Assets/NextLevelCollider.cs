using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelCollider : MonoBehaviour
{
    public void LoadNextLevel() => LevelController.Instance.NextLevel();

    private void OnCollisionEnter(Collision collision)
    {
        var collided = collision.gameObject.GetComponent<Player>();

        if(collided != null )
        {
            LoadNextLevel();
        }
    }
}
