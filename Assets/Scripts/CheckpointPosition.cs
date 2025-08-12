using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public class CheckpointPosition : CheckPointObj
{
    private void Awake()
    {
        actionToDo = ActionToDoL;
    }

    void ActionToDoL()
    {
        StartCoroutine(ActionCO());
    }
    
    IEnumerator ActionCO()
    {
        yield return new WaitForSeconds(0.25f);

        Player.Instance.transform.parent.GetComponent<KinematicCharacterMotor>().SetPosition(new Vector3(transform.position.x, Player.Instance.transform.parent.position.y, transform.position.z));
    }
}
