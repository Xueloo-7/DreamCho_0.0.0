using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistenceSingleton<T> : MonoBehaviour where T: PersistenceSingleton<T>
{
    private static T instance;

    public static T Instance
    {
        get { return instance; }
    }

    protected virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = (T)this;
        }

        DontDestroyOnLoad(gameObject);
    }
}
