using System.Collections.Generic;
using UnityEngine;

public class EventManager {
    public delegate void EventReceiver(params object[] parameterContainer);

    static Dictionary<string, EventReceiver> _dictionaryEvents;

    public static void Subscribe(string eventType, EventReceiver listener){
        if (_dictionaryEvents == null){
            _dictionaryEvents = new Dictionary<string, EventReceiver>();
        }

        if (!_dictionaryEvents.ContainsKey(eventType)){
            _dictionaryEvents.Add(eventType, null);
        }

        _dictionaryEvents[eventType] += listener;
    }

    public static void Unsubscribe(string eventType, EventReceiver listener){
        if (_dictionaryEvents != null){
            if (_dictionaryEvents.ContainsKey(eventType)){
                _dictionaryEvents[eventType] -= listener;
            }
        }
    }

    public static void TriggerEvent(string eventType, params object[] parameters){
        if (_dictionaryEvents == null){
            GameManager.ShowLog("No events subscribed");
            return;
        }

        if (_dictionaryEvents.ContainsKey(eventType)){
            if (_dictionaryEvents[eventType] != null)
            {
                _dictionaryEvents[eventType](parameters);
            }
        }

    }

    public static void TriggerEvent(string eventType){
        TriggerEvent(eventType, null);
    }
}