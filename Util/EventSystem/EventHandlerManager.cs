using System;
using System.Collections.Generic;

public static class EventHandlerManager
{
    private static Dictionary<EventEnum, EventHandler<EventArgs>> events = new Dictionary<EventEnum, EventHandler<EventArgs>>(new FastEnumIntEqualityComparer<EventEnum>());
    private static Dictionary<EventEnum, Stack<EventHandler<EventArgs>>> lockEvents = new Dictionary<EventEnum, Stack<EventHandler<EventArgs>>>(new FastEnumIntEqualityComparer<EventEnum>());

    public static void RegisterEventHandler(EventEnum eventType, EventHandler<EventArgs> handle)
    {
        if (!events.ContainsKey(eventType))
        {
            events.Add(eventType, handle);
        }
        else
        {
            events[eventType] += handle;
        }
    }

    public static void UnRegisterEventHandler(EventEnum eventType, EventHandler<EventArgs> handle)
    {
        if (events.ContainsKey(eventType))
        {
            events[eventType] -= handle;
            if (events[eventType] == null)
                events.Remove(eventType);
        }
    }

    public static void Invoke(EventEnum eventType, object sender, EventArgs args)
    {
        if (events.ContainsKey(eventType))
        {
            events[eventType].Invoke(sender, args);
        }
    }

    /// <summary>
    /// 다른 이벤트트리거를 중단하고 설정한것만 수행하게 하는 함수
    /// </summary>
    public static void LockEvent(EventEnum eventType, EventHandler<EventArgs> handle)
    {
        if (events.ContainsKey(eventType))
        {
            if (!lockEvents.ContainsKey(eventType))
            {
                var lockStack = new Stack<EventHandler<EventArgs>>();
                lockStack.Push(events[eventType]);

                lockEvents.Add(eventType, lockStack);
                events.Remove(eventType);
            }
            else
            {
                lockEvents[eventType].Push(events[eventType]);
                events.Remove(eventType);
            }

            RegisterEventHandler(eventType, handle);
        }
    }

    /// <summary>
    /// 캐시해둔 이벤트 트리거를 재 설정
    /// </summary>
    public static void UnLockEvent(EventEnum eventType, EventHandler<EventArgs> handle)
    {
        if(lockEvents.ContainsKey(eventType))
        {
            UnRegisterEventHandler(eventType, handle);

            events.Add(eventType, lockEvents[eventType].Pop());
            if(lockEvents[eventType].Count == 0)
                lockEvents.Remove(eventType);
        }
    }
}

public enum EventRegisterCall
{
    Awake,
    OnEnable,
    OnDisable,
    OnDestroy
}

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class AutoRegisterEvent : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
public class EventMethodAttribute : Attribute
{
    public EventEnum eventType { get; set; }
    public EventRegisterCall eventRegisterCall { get; set; }
    public EventRegisterCall eventUnregisterCall { get; set; }

    public EventMethodAttribute
        (EventEnum _event, EventRegisterCall registCall = EventRegisterCall.Awake, EventRegisterCall unregistCall = EventRegisterCall.OnDestroy)
    {
        eventType = _event;
        eventRegisterCall = registCall;
        eventUnregisterCall = unregistCall;
    }
}