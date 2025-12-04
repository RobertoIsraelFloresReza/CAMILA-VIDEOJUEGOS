using UnityEngine;
using System;
using System.Collections.Generic;

[DefaultExecutionOrder(-1000)]
public class EventManager : MonoBehaviour
{
    private static readonly Dictionary<GlobalEvents, List<Delegate>> Events = new();

    private void Awake()
    {
        foreach (var eventName in Enum.GetValues(typeof(GlobalEvents)))
        {
            Events.Add((GlobalEvents)eventName, new List<Delegate>());
        }
    }

    public static void Subscribe<T>(GlobalEvents globalEvent, Action<T> method)
    {
        Events[globalEvent].Add(method);
    }

    public static void Subscribe(GlobalEvents globalEvent, Action method)
    {
        Events[globalEvent].Add(method);
    }

    public static void Unsubscribe<T>(GlobalEvents globalEvent, Action<T> method)
    {
        if (!Events.ContainsKey(globalEvent))
        {
            return;
        }

        var eventList = Events[globalEvent];
        Delegate delegateToRemove = method;

        int index = eventList.FindIndex(d => d.Equals(delegateToRemove));

        if (index >= 0)
        {
            eventList.RemoveAt(index);
        }
    }

    public static void Unsubscribe(GlobalEvents globalEvent, Action method)
    {
        if (!Events.ContainsKey(globalEvent)) 
        {
            return;
        }

        var eventList = Events[globalEvent];
        Delegate delegateToRemove = method;

        int index = eventList.FindIndex(d => d.Equals(delegateToRemove));

        if (index >= 0)
        {
            eventList.RemoveAt(index);
        }
    }

    public static void Invoke<T>(GlobalEvents globalEvent, T value = default)
    {
        if (!Events.ContainsKey(globalEvent))
        {
            return;
        }

        foreach (var @delegate in Events[globalEvent])
        {
            switch (@delegate)
            {
                case Action<T> action:
                    action.Invoke(value);
                    break;
            }
        }
    }

    public static void Invoke(GlobalEvents globalEvent)
    {
        if (!Events.ContainsKey(globalEvent))
        {
            return; 
        }
    
        foreach (var @delegate in Events[globalEvent]) 
            if (@delegate is Action action) 
                action.Invoke();
    }
}
