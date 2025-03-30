using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    private readonly Dictionary<string, Queue<GameObject>> objectPools = new();
    
    // 所有不同类型的对象池共用同一个结构
    private readonly Dictionary<string, Component> prefabs = new();
    // 跟踪正在被回收的对象，确保对象不在回收时被重复回收
    private readonly HashSet<GameObject> pendingReturnObjects = new();

    private new void Awake()
    {
        base.Awake();
        Event.onNewSceneStart += ClearPool;
    }
    private new void OnDestroy()
    {
        base.OnDestroy();
        Event.onNewSceneStart -= ClearPool;
    }


    private void RegisterPrefab<T>(T prefab) where T : Component
    {
        if (!prefabs.ContainsKey(prefab.gameObject.name))
        {
            prefabs.Add(prefab.gameObject.name, prefab);
        }
    }

    #region GET

    // 泛型的获取方法
    public T GetObjectFromPool<T>(T prefab, Transform parent = null) where T : Component
    {
        if (Instance == null)
        {
            Debug.LogError("Object Pool不存在，无法调用!");
            return null;
        }
        var poolName = prefab.gameObject.name;
        RegisterPrefab(prefab); // 如果该 prefab 还没有注册，则注册它

        if (!objectPools.ContainsKey(poolName))
        {
            objectPools.Add(poolName, new Queue<GameObject>());
        }

        var pool = objectPools[poolName];
        T obj;
        if (pool.Count == 0)
        {
            // 对象池中没有可用对象时，创建实例并加入到池中
            obj = Instantiate(prefab.gameObject).GetComponent<T>();
            obj.name = obj.name.Replace("(Clone)", "");
            obj.transform.SetParent(parent == null ? transform : parent);
        }
        else
        {
            // 从池中获取可用对象
            obj = pool.Dequeue().GetComponent<T>();
            if (!obj.gameObject.activeInHierarchy)
                obj.gameObject.SetActive(true);
        }

        return obj;
    }
    #endregion

    #region RETURN

    // 泛型的返回方法
    public Coroutine ReturnObjectToPool<T>(T obj, float delay, Action onReturn = null) where T : Component
    {
        if (Instance == null)
        {
            Debug.LogError("Object Pool不存在，无法调用!");
            return null;
        }
        if (pendingReturnObjects.Contains(obj.gameObject))
        {
            // 对象已经在等待回收列表中，避免重复调用
            return null;
        }

        pendingReturnObjects.Add(obj.gameObject);
        return StartCoroutine(DelayReturnObjectToPool(obj, delay, onReturn));
    }
    #endregion

    #region RETURN Coroutine

    // 泛型的延时返回协程
    private IEnumerator DelayReturnObjectToPool<T>(T obj, float delay, Action onReturn) where T : Component
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        obj.gameObject.SetActive(false);

        // 将对象放回池中
        objectPools[obj.gameObject.name].Enqueue(obj.gameObject);

        // 从等待回收列表中移除
        pendingReturnObjects.Remove(obj.gameObject);

        onReturn?.Invoke();
    }

    #endregion

    private void ClearPool()
    {
        objectPools.Clear();
        prefabs.Clear();
        pendingReturnObjects.Clear();
    }
}