using Sirenix.OdinInspector;
using UnityEngine;

public abstract class SingleTon<T> : SerializedMonoBehaviour where T : class
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}