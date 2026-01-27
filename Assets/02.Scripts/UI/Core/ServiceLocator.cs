using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator
{
    private static ServiceLocator _instance;
    public static ServiceLocator Instance => _instance ??= new ServiceLocator();

    private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

    public void Register<T>(T service) where T : class
    {
        var type = typeof(T);

        if (_services.ContainsKey(type))
        {
            Debug.LogWarning($"Service {type.Name} is already registered. Overwriting.");
        }

        _services[type] = service;
        Debug.Log($"Service Registered: {type.Name}");
    }

    public T Get<T>() where T : class
    {
        var type = typeof(T);

        if (_services.TryGetValue(type, out var service))
        {
            return service as T;
        }

        Debug.LogError($"Service {type.Name} not found. Make sure it's registered.");
        return null;
    }

    public bool Has<T>() where T : class
    {
        return _services.ContainsKey(typeof(T));
    }

    public void Clear()
    {
        _services.Clear();
        Debug.Log("ServiceLocator cleared.");
    }
}
