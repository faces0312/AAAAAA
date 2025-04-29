using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BaseGameManager : MonoBehaviour
{
    protected List<IBaseManager> _managers = new List<IBaseManager>();

    public bool IsInitialized { get; private set; } = false;

    public static BaseGameManager Instance { get; private set; }
    protected virtual void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 60;

        AddManagers();
    }

    protected abstract void AddManagers();

    private void Start()
    {
        Debug.Log($"[ {GetType().Name} ] Initialize Start!");

        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        yield return StartCoroutine(InitializeManagers());
        InitializeManagerForce();
        InitializeCompleted();
    }

    private IEnumerator InitializeManagers()
    {
        yield return null;
        foreach (var asyncManager in _managers)
        {
            asyncManager.Init();
            yield return new WaitUntil(() => asyncManager.IsInitialized);
        }
    }

    protected abstract void InitializeManagerForce();

    private void InitializeCompleted()
    {
        Debug.Log($"[ {GetType().Name} ] Initialize Success!");
        IsInitialized = true;
        OnInit();
    }

    protected abstract void OnInit();
}
