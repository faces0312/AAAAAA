using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using Unity.VisualScripting;


public class DiceManager : BaseObjectManager<DiceManager, Dice>
{
    public int _diceResult;

    private List<Dice> activeDiceList = new List<Dice>();
    private Coroutine cameraResetCoroutine = null;
    private int _activeDiceCount = 0;

    public Camera mainCamera;
    public Camera diceCamera;
    
    private GameObject basketPrefab;     //
    private GameObject spawnedBasket;
    
    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (diceCamera == null)
            diceCamera = GetComponentInChildren<Camera>(true);

        basketPrefab = Resources.Load<GameObject>("Prefabs/Dice/Dice_Basket");
        if (basketPrefab == null)
            Debug.LogError("Basket 프리팹을 찾을 수 없습니다!");
    }

    public override Dice CreateObject(int id)
    {
        string path = $"Prefabs/Dice/Dice_{id}";
        GameObject prefab = Resources.Load<GameObject>(path);

        if (prefab == null)
            return null;

        GameObject obj = Instantiate(prefab);
        Dice dice = obj.GetComponent<Dice>();

        if (dice == null)
        {
            Destroy(obj);
            return null;
        }

        return dice;
    }

    public List<Dice> SpawnMultipleDice(int id, int count, Transform parent, Vector3 startPos, Quaternion rotation)
    {
        Debug.Log($"[DiceManager] SpawnMultipleDice ȣ��: id={id}, count={count}");

        if (spawnedBasket == null && basketPrefab != null)
        {
            spawnedBasket = Instantiate(basketPrefab, parent);
            spawnedBasket.transform.localPosition = Vector3.zero;
            Debug.Log("[DiceManager] DiceBasket 생성 완료");
        }

        List<Dice> diceList = new List<Dice>();

        for (int i = 0; i < count; i++)
        {
            Vector3 offset = startPos + new Vector3(i * 2f, 0, 0);
            Dice dice = SpawnDice(id, offset, rotation);
            if (dice != null)
            {
                Debug.Log($"[DiceManager] Dice ���� ���� at {offset}");
                dice.transform.SetParent(parent);
                diceList.Add(dice);
            }
            else
            {
                Debug.LogError($"[DiceManager] Dice ���� ���� at {offset}");
            }
        }

        return diceList;
    }

    public Dice SpawnDice(int id, Vector3 position, Quaternion rotation)
    {
        Dice dice = Spawn(id, position, rotation);
        if (dice != null)
        {
            _activeDiceCount++;
            activeDiceList.Add(dice);

            StartCoroutine(DelayedSetDiceCamera(dice.transform));

            dice.OnDiceStopped += () =>
            {
                if (dice.IsDestroyed) return;

                _activeDiceCount--;

                if (_activeDiceCount <= 0)
                {
                    ResetDiceState();

                    if (cameraResetCoroutine != null)
                      StopCoroutine(cameraResetCoroutine);

                    cameraResetCoroutine = StartCoroutine(DelayedCameraReset());
                }
            };
        }
        return dice;
    }

    private IEnumerator DelayedSetDiceCamera(Transform diceTransform)
    {
        yield return new WaitForSeconds(0.01f);
        SetDiceCamera(true);
    }

    private IEnumerator DelayedCameraReset()
    {
        yield return new WaitForSeconds(1f);
        SetDiceCamera(false);
        if (UIManager.Instance != null)
            UIManager.Instance.EnableThrowDiceButton();
        BattleManager.Instance.StartBattle();
    }

    private void SetDiceCamera(bool useDiceCamera)
    {
        if (mainCamera == null || diceCamera == null) return;

        diceCamera.enabled = useDiceCamera;
        mainCamera.enabled = !useDiceCamera;
    }

    public void ResetDiceState()
    {
        if (cameraResetCoroutine != null)
        {
            StopCoroutine(cameraResetCoroutine);
            cameraResetCoroutine = null;
        }

        SetDiceCamera(false);

        foreach (var dice in activeDiceList)
        {
            if (dice != null)
            {
                dice.MarkAsDestroyed();
                Destroy(dice.gameObject);
            }
        }

        activeDiceList.Clear();
        _activeDiceCount = 0;

        if (spawnedBasket != null)
        {
            Destroy(spawnedBasket);
            spawnedBasket = null;
        }
    }
}
