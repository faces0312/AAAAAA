using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : SingletonWithMono<CanvasManager>, IBaseManager
{
    public bool IsInitialized { get; set; }

    [Header("Map ����")]
    [SerializeField] private Transform mapParent;

    [Header("�ֻ��� ����")]
    [SerializeField] private Button throwDiceButton;

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

    public Button GetThrowDiceButton()
    {
        return throwDiceButton;
    }
}
