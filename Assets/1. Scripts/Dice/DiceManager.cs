using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DiceManager : BaseObjectManager<DiceManager, Dice>
{
    private int _activeDiceCount = 0;
    private Coroutine cameraResetCoroutine = null;

    public int _diceResult;

    public Camera mainCamera;
    public Camera diceCamera;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (diceCamera != null)
            diceCamera.enabled = false;
    }

    public override Dice CreateObject(int id)
    {
        string dicePath = $"Prefabs/Dice_{id}";
        GameObject dicePrefab = Resources.Load<GameObject>(dicePath);

        if (dicePrefab == null)
        {
            Debug.LogError($"주사위 프리팹을 찾을 수 없음: {dicePath}");
            return null;
        }

        GameObject diceObj = Instantiate(dicePrefab);
        Dice dice = diceObj.GetComponent<Dice>();

        if (dice == null)
        {
            Debug.LogError($"주사위 프리팹에 Dice 컴포넌트가 없음: {dicePath}");
            Destroy(diceObj);
            return null;
        }

        return dice;
    }

    public List<Dice> SpawnMultipleDice(int id, int count, Transform parent, Vector3 startPos, Quaternion rotation)
    {
        List<Dice> diceList = new List<Dice>();

        for (int i = 0; i < count; i++)
        {
            Vector3 offsetPos = startPos + new Vector3(i * 2f, 0, 0); // 위치 간격 조절
            Dice dice = SpawnDice(id, offsetPos, rotation);
            if (dice != null)
            {
                dice.transform.SetParent(parent); // 부모 설정
                diceList.Add(dice);
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
            SetDiceCamera(true, dice.transform);

            dice.OnDiceStopped += () =>
            {
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

    private IEnumerator DelayedCameraReset()
    {
        yield return new WaitForSeconds(1f);
        SetDiceCamera(false, null);
    }

    private void SetDiceCamera(bool isDiceView, Transform diceTransform)
    {
        if (diceCamera == null || mainCamera == null) return;

        if (isDiceView)
        {
            diceCamera.enabled = true;
            mainCamera.enabled = false;

            /*diceCamera.transform.position = diceTransform.position + new Vector3(0, 2, -3);
            diceCamera.transform.LookAt(diceTransform);*/
        }
        else
        {
            diceCamera.enabled = false;
            mainCamera.enabled = true;
        }
    }
}
