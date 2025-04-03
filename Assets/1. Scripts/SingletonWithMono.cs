using UnityEngine;

public class SingletonWithMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static object lockObj = new object();
    private static bool isQuitting = false;

    public static T Instance
    {
        get
        {
            if (isQuitting)
            {
                Debug.LogWarning($"[Singleton] {typeof(T)} �ν��Ͻ��� ��ȯ�� �� �����ϴ�. �̹� ���� ���� ���Դϴ�.");
                return null;
            }

            lock (lockObj)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();

                    if (instance == null)
                    {
                        GameObject singletonObj = new GameObject(typeof(T).Name);
                        instance = singletonObj.AddComponent<T>();
                        DontDestroyOnLoad(singletonObj);
                    }
                }
                return instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
