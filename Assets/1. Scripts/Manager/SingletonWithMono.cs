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
                Debug.LogWarning($"[Singleton] {typeof(T)} 인스턴스를 반환할 수 없습니다. 이미 게임 종료 중입니다.");
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

            if (instance is DiceManager diceManager)
            {
                CreateDiceCamera(diceManager);
            }
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

    private void CreateDiceCamera(DiceManager diceManager)
    {
        GameObject cameraObj = new GameObject("DiceCamera");
        cameraObj.transform.SetParent(diceManager.transform);
        cameraObj.transform.localPosition = new Vector3(0,30,0);
        cameraObj.transform.localRotation = Quaternion.Euler(90,0,0);

        Camera camera = cameraObj.AddComponent<Camera>();

        camera.enabled = false; // 초기에는 비활성화
        diceManager.diceCamera = camera; // DiceManager에 연결
    }
}
