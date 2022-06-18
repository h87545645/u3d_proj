using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
 
public class EventManager
{
    private Dictionary<string, UnityEvent> eventDictionary = new Dictionary<string, UnityEvent>();
    private static EventManager eventManager = new EventManager();
    private EventManager()
    {
 
    }
    public static EventManager I
    {
        get
        {
            return eventManager;
        }
    }
    public void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (eventManager.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            eventManager.eventDictionary.Add(eventName, thisEvent);
        }
    }
 
    public void StopListening(string eventName, UnityAction listener)
    {
        if (eventManager == null) return;
        UnityEvent thisEvent = null;
        if (eventManager.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }
 
    public void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (eventManager.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
}