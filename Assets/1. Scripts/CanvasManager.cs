using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : SingletonWithMono<CanvasManager>, IBaseManager
{
    public bool IsInitialized { get; set; }

    [Header("Map 관련")]
    [SerializeField] private Transform mapParent;

    [Header("주사위 관련")]
    [SerializeField] private Button throwDiceButton;

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

    public Button GetThrowDiceButton()
    {
        return throwDiceButton;
    }
}
