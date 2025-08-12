using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointItems : CheckPointObj
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

        if (GetComponent<ItemObject>()) GetComponent<ItemObject>().OnInteract();
    }
}
