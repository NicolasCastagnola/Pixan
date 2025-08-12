using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatTimerEventFilter : BaseEventFilter
{

    [SerializeField] float timeToTrigger = 5f;
    private float timer = 0;

    public void Update()
    {
            timer += Time.deltaTime;
            if (timer >= timeToTrigger)
            {
                onTrigger.Invoke();
                timer = 0;
            }
    }

    public override void OnTrigger()
    {
        //hasTriggered = true;
    }
}
