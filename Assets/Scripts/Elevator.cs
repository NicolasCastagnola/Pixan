using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Elevator : Interactable
{
    [SerializeField] float seconds,downPos,upPos;
    bool isInteract;
    public override void OnInteract()
    {
        if (!isInteract)
        {
            isInteract = true;
            if (transform.position.y > downPos)
                transform.DOMoveY(downPos, seconds)
                    .OnComplete(() => { 
                        transform.position = new Vector3(transform.position.x, downPos, transform.position.z); 
                        isInteract = false; 
                    });
            else
                transform.DOMoveY(upPos, seconds).OnComplete(() => {
                    isInteract = false;
                    transform.DOMoveX(transform.position.x, 20).OnComplete(//wait 20 seconds
                        () => {
                            if (!isInteract && transform.position.y > downPos)
                            {
                                isInteract = true;
                                transform.DOMoveY(downPos, seconds)
                                    .OnComplete(() => { 
                                        transform.position = new Vector3(transform.position.x, downPos, transform.position.z); 
                                        isInteract = false; 
                                    });
                            }
                        });
                });
        }
    }
    public override void OnExitOnReachableRadius()
    {
        
    }
}
