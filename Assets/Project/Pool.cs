using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

public class Pool : SerializedMonoBehaviour
{
    [SerializeField] private Dictionary<string, ObjectPool<object>> pools = new Dictionary<string, ObjectPool<object>>();

    public void RegistPool<T>(string key, Func<object> onCreate, Action<object> onGet = null, Action<object> onRelease = null, Action<object> onDestroy = null)
    {
        ObjectPool<object> newPool = new ObjectPool<object>(onCreate, onGet, onRelease, onDestroy, defaultCapacity: 1);

        pools.Add(key, newPool);
    }

    public ObjectPool<object> GetPool(string key)
    {
        if (pools.TryGetValue(key, out var pool))
        {
            return pool;
        }

        return null;
    }

    public bool TryGetPool(string key, out ObjectPool<object> result)
    {
        result = null;

        if (pools.TryGetValue(key, out var pool))
        {
            result = pool;
            return true;
        }

        return false;
    }
}
