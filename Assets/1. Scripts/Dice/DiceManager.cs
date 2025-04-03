using UnityEngine;
using UnityEngine.SocialPlatforms;

public class DiceManager : BaseObjectManager<DiceManager, Dice>
{
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
            Debug.LogError($"�ֻ��� �������� ã�� �� ����: {dicePath}");
            return null;
        }

        GameObject diceObj = Instantiate(dicePrefab);
        Dice dice = diceObj.GetComponent<Dice>();

        if (dice == null)
        {
            Debug.LogError($"�ֻ��� �����տ� Dice ������Ʈ�� ����: {dicePath}");
            Destroy(diceObj);
            return null;
        }

        return dice;
    }

    public Dice SpawnDice(int id, Vector3 position, Quaternion rotation)
    {
        Dice dice = Spawn(id, position, rotation);
        if (dice != null)
        {
            SetDiceCamera(true, dice.transform);
            dice.OnDiceStopped += () => SetDiceCamera(false, null);
        }
        return dice;
    }

    private void SetDiceCamera(bool isDiceView, Transform diceTransform)
    {
        if (diceCamera == null || mainCamera == null) return;

        if (isDiceView)
        {
            diceCamera.enabled = true;
            mainCamera.enabled = false;

            diceCamera.transform.position = diceTransform.position + new Vector3(0, 2, -3);
            diceCamera.transform.LookAt(diceTransform);
        }
        else
        {
            diceCamera.enabled = false;
            mainCamera.enabled = true;
        }
    }
}
