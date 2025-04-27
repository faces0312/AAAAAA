using UnityEngine;

public class CanvasManager : SingletonWithMono<CanvasManager>, IBaseManager
{
    public bool IsInitialized { get; set; }

    [Header("Map 관련")]
    public Transform mapParent;

    public void Init()
    {
        if (mapParent == null)
        {
            Debug.LogError("[CanvasManager] mapParent가 설정되지 않았습니다!");
        }
        IsInitialized = true;
    }

    public Transform GetMapParent()
    {
        return mapParent;
    }
}
