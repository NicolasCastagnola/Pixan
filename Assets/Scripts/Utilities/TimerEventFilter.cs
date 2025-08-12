using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class TimerEventFilter : BaseEventFilter
    {
        [SerializeField] float timeToTrigger = 5f;
        private float timer = 0;
        private bool hasTriggered = false;

        public void Update()
        {
            if (hasTriggered)
            {
                timer += Time.deltaTime;
                if (timer >= timeToTrigger)
                {
                    onTrigger.Invoke();
                    hasTriggered = false;

                }
            }
        }

        public override void OnTrigger()
        {
            hasTriggered = true;
        }
    }
