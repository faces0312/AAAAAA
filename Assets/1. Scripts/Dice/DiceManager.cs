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

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (diceCamera == null)
            diceCamera = GetComponentInChildren<Camera>(true);
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
        Debug.Log($"[DiceManager] SpawnMultipleDice 龋免: id={id}, count={count}");

        List<Dice> diceList = new List<Dice>();

        for (int i = 0; i < count; i++)
        {
            Vector3 offset = startPos + new Vector3(i * 2f, 0, 0);
            Dice dice = SpawnDice(id, offset, rotation);
            if (dice != null)
            {
                Debug.Log($"[DiceManager] Dice 积己 己傍 at {offset}");
                dice.transform.SetParent(parent);
                diceList.Add(dice);
            }
            else
            {
                Debug.LogError($"[DiceManager] Dice 积己 角菩 at {offset}");
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
    }
}
