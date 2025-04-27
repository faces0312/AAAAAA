using System.Resources;
using UnityEngine;

public class GameManager : BaseGameManager
{
    public static GameManager Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    protected override void AddManagers()
    {
        _managers.Add(CanvasManager.Instance);
        _managers.Add(MapGenerator.Instance);
        _managers.Add(UIManager.Instance);
    }

    protected override void InitializeManagerForce() { }

    protected override void OnInit()
    {
    }

    public T GetManager<T>() where T : class, IBaseManager
    {
        foreach (var manager in _managers)
        {
            if (manager is T typedManager)
                return typedManager;
        }
        return null;
    }
}
