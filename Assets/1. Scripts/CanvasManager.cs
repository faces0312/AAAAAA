using UnityEngine;

public class CanvasManager : SingletonWithMono<CanvasManager>, IBaseManager
{
    public bool IsInitialized { get; set; }

    [Header("Map ����")]
    public Transform mapParent;

    public void Init()
    {
        if (mapParent == null)
        {
            Debug.LogError("[CanvasManager] mapParent�� �������� �ʾҽ��ϴ�!");
        }
        IsInitialized = true;
    }

    public Transform GetMapParent()
    {
        return mapParent;
    }
}
