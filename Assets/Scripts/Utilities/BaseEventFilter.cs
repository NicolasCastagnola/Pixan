using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

    public abstract class BaseEventFilter : MonoBehaviour
    {
        public UnityEvent onTrigger;
        public abstract void OnTrigger();

    }
