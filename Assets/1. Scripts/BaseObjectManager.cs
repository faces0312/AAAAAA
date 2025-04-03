using System.Collections.Generic;
using UnityEngine;

public abstract class BaseObjectManager<T, U> : SingletonWithMono<T>, IBaseManager
    where T : MonoBehaviour
    where U : MonoBehaviour
{
    public bool IsInitialized { get; set; }

    private Dictionary<int, Queue<U>> objectPool = new Dictionary<int, Queue<U>>();

    public virtual void Init()
    {
        IsInitialized = true;
    }

    public abstract U CreateObject(int id);

    public virtual U Spawn(int id, Vector3 position, Quaternion rotation)
    {
        if (!objectPool.ContainsKey(id))
        {
            objectPool[id] = new Queue<U>();
        }

        U obj;
        if (objectPool[id].Count > 0)
        {
            obj = objectPool[id].Dequeue();
            obj.gameObject.SetActive(true);
        }
        else
        {
            obj = CreateObject(id);
            if (obj == null)
            {
                Debug.LogError($"오브젝트 생성 실패: ID {id}");
                return null;
            }
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        return obj;
    }

    public virtual void Despawn(int id, U obj)
    {
        if (obj == null) return;

        obj.gameObject.SetActive(false);
        if (!objectPool.ContainsKey(id))
        {
            objectPool[id] = new Queue<U>();
        }

        objectPool[id].Enqueue(obj);
    }
}