using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonWithMono<UIManager>
{
    public Button throwDiceButton;
    public int diceType = 6; // 4, 6, 8 ������ ���� ����
    public int diceCount = 2; // ������ �ֻ��� ��

    private Transform diceParent; // �ֻ����� ���� �θ� ������Ʈ
    public Vector3 startSpawnPos = new Vector3(0, 2, 0);

    private void Start()
    {
        throwDiceButton.onClick.AddListener(OnThrowDiceClicked);
    }

    private void OnThrowDiceClicked()
    {
        if (diceParent == null)
        {
            diceParent = new GameObject("DiceParent").transform;
        }

        throwDiceButton.interactable = false;

        DiceManager.Instance.ResetDiceState(); //j

        List<Dice> diceList = DiceManager.Instance.SpawnMultipleDice(
            diceType, diceCount, diceParent, startSpawnPos, Random.rotation
        );
    }
}
