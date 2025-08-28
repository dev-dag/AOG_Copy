using System;
using UnityEngine;

public struct ObserverEventArg<T>
{
    public T oldValue;
    public T newValue;

    public ObserverEventArg(T oldValue, T newValue)
    {
        this.oldValue = oldValue;
        this.newValue = newValue;
    }
}

public class ObserverProperty<T>
{
    public event Action<ObserverEventArg<T>> onValueChengeEvent;

    public T Value
    {
        get => value;
        set
        {
            var temp = value;
            this.value = value;

            onValueChengeEvent?.Invoke(new ObserverEventArg<T>(temp, this.value));
        }
    }

    [SerializeField] private T value;

    public ObserverProperty()
    {
        value = default(T);
    }

    public ObserverProperty(T newValue)
    {
        value = newValue;
    }
}