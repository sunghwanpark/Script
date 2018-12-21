using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class PhotonBehaviourBase : Photon.PunBehaviour
{
    protected Transform cachedTransform;
    private Dictionary<EventRegisterCall, List<MethodInfo>> registEventMethods = null;
    private Dictionary<EventRegisterCall, List<MethodInfo>> unregistEventMethods = null;

    private void SetCallingEventHandlers()
    {
        registEventMethods = new Dictionary<EventRegisterCall, List<MethodInfo>>();
        unregistEventMethods = new Dictionary<EventRegisterCall, List<MethodInfo>>();

        foreach (EventRegisterCall call in Enum.GetValues(typeof(EventRegisterCall)))
        {
            var regMethods = GetType()
                .GetMethods()
                .Where(m =>
                {
                    var eAttr = m.GetCustomAttribute<EventMethodAttribute>();
                    return eAttr != null && eAttr.eventRegisterCall == call;
                })
                .ToList();

            var unregMethods = GetType()
                .GetMethods()
                .Where(m =>
                {
                    var eAttr = m.GetCustomAttribute<EventMethodAttribute>();
                    return eAttr != null && eAttr.eventUnregisterCall == call;
                })
                .ToList();

            if (regMethods.Count > 0)
                registEventMethods.Add(call, regMethods);
            if (unregMethods.Count > 0)
                unregistEventMethods.Add(call, unregMethods);
        }
    }

    private void RegistEventHandler(EventRegisterCall callType)
    {
        if (registEventMethods.ContainsKey(callType))
            RegEventHandler(registEventMethods[callType]);
    }

    private void UnRegistEventHandler(EventRegisterCall callType)
    {
        if (unregistEventMethods.ContainsKey(callType))
            UnregEventHandler(unregistEventMethods[callType]);
    }

    protected virtual void Awake()
    {
        this.SetFields();
        cachedTransform = this.transform;

        var attr = GetType().GetCustomAttributes<AutoRegisterEvent>();
        if (attr != null)
            SetCallingEventHandlers();

        RegistEventHandler(EventRegisterCall.Awake);
        UnRegistEventHandler(EventRegisterCall.Awake);
    }

    protected virtual void OnEnable()
    {
        RegistEventHandler(EventRegisterCall.OnEnable);
        UnRegistEventHandler(EventRegisterCall.OnEnable);
    }

    protected virtual void OnDisable()
    {
        RegistEventHandler(EventRegisterCall.OnDisable);
        UnRegistEventHandler(EventRegisterCall.OnDisable);
    }

    protected virtual void OnDestroy()
    {
        RegistEventHandler(EventRegisterCall.OnDestroy);
        UnRegistEventHandler(EventRegisterCall.OnDestroy);
    }

    private void RegEventHandler(List<MethodInfo> eventMethods)
    {
        foreach (var m in eventMethods)
        {
            var eventType = m.GetCustomAttribute<EventMethodAttribute>();
            var eventHandler = Delegate.CreateDelegate(typeof(EventHandler<EventArgs>), this, m) as EventHandler<EventArgs>;
            EventHandlerManager.RegisterEventHandler(eventType.eventType, eventHandler);
        }
    }

    private void UnregEventHandler(List<MethodInfo> eventMethods)
    {
        foreach (var m in eventMethods)
        {
            var eventType = m.GetCustomAttribute<EventMethodAttribute>();
            var eventHandler = Delegate.CreateDelegate(typeof(EventHandler<EventArgs>), this, m) as EventHandler<EventArgs>;
            EventHandlerManager.UnRegisterEventHandler(eventType.eventType, eventHandler);
        }
    }
}
