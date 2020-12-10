using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    private Dictionary<EventTypes, List<IEventListener>> Listeners { get; }
    private static EventManager _eventManager;

    public static EventManager GetInstance()
    {
        if (_eventManager == null)
        {
            _eventManager = new EventManager();
        }

        return _eventManager;
    }

    private EventManager()
    {
        Listeners = new Dictionary<EventTypes, List<IEventListener>>();
    }

    public void Subscribe(EventTypes evento, IEventListener eventListener)
    {
        if (Listeners.ContainsKey(evento) == false)
        {
            Listeners.Add(evento, new List<IEventListener> {eventListener});
        }

        if (Listeners[evento].Contains(eventListener) == false)
        {
            Listeners[evento].Add(eventListener);
        }
    }

    public void Notify(EventTypes evento)
    {
        foreach (var eventListener in Listeners[evento])
        {
            eventListener.OnEvent(evento);
        }
    }

    public enum EventTypes
    {
        LoseLife,
        IncreaseScore,
        IncreaseCoins,
        LoseCoin
    }
}