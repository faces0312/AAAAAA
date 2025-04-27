using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonWithMono<UIManager>, IBaseManager
{
    public Button throwDiceButton;
    public int diceType = 6; // 4, 6, 8 등으로 변경 가능
    public int diceCount = 2; // 생성할 주사위 수

    private Transform diceParent;
    public Vector3 startSpawnPos = new Vector3(0, 2, 0);

    public bool IsInitialized { get; set; }

    public void Init()
    {
        if (throwDiceButton != null)
        {
            throwDiceButton.onClick.AddListener(OnThrowDiceClicked);
        }
        IsInitialized = true;
    }

    private void OnThrowDiceClicked()
    {
        if (diceParent == null)
        {
            diceParent = new GameObject("DiceParent").transform;
        }

        throwDiceButton.interactable = false;

        DiceManager.Instance.ResetDiceState();

        List<Dice> diceList = DiceManager.Instance.SpawnMultipleDice(
            diceType, diceCount, diceParent, startSpawnPos, Random.rotation
        );
    }
}
