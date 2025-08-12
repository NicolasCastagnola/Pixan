using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CheckPointObj : MonoBehaviour
{
    [SerializeField] string key;
    [SerializeField] int priority;

    protected Action actionToDo;

    private void Start()
    {
        if (SaveCheckManager.Instance.GetCheckPoints().Count > 0)
        {
            foreach (var item in SaveCheckManager.Instance.GetCheckPoints())
            {
                if (key.Equals(item.Key) && item.Value.Equals(priority))
                {
                    if (GetComponent<BoxCollider>()) GetComponent<BoxCollider>().enabled = false;
                    ActionToDo();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            SaveCheckpoint();
            if(GetComponent<BoxCollider>()) GetComponent<BoxCollider>().enabled = false;
        }
    }

    public virtual void SaveCheckpoint()
    {
        SaveCheckManager.Instance.AddToDic(key, priority); 
    }

    public virtual void ActionToDo()
    {
        actionToDo?.Invoke();
    }
}
