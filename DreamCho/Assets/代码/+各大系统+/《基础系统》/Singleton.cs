using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError($"单例 {typeof(T).Name} 尚未初始化！");
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning($"销毁重复的 {typeof(T).Name} 实例：{gameObject.name}");
            Destroy(gameObject);
            return;
        }

        instance = (T)this;
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;  // 清空静态引用，避免指向已销毁对象
        }
    }
}
