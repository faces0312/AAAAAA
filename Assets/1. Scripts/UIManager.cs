using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonWithMono<UIManager>, IBaseManager
{
    private Button throwDiceButton;
    private Transform diceParent;

    public bool IsInitialized { get; set; }

    public void Init()
    {
        throwDiceButton = CanvasManager.Instance.GetThrowDiceButton();
        if (throwDiceButton != null)
        {
            throwDiceButton.onClick.AddListener(OnThrowDiceClicked);
        }
        else
        {
            Debug.LogWarning("[UIManager] throwDiceButton이 존재하지 않습니다.");
        }


        diceParent = CanvasManager.Instance.GetMapParent();
        if (diceParent == null)
        {
            Debug.LogWarning("[UIManager] diceParent가 없어서 새로 생성합니다.");
            diceParent = new GameObject("DiceParent").transform;
        }

        IsInitialized = true;
    }

    private void OnThrowDiceClicked()
    {
        throwDiceButton.interactable = false;
        DiceManager.Instance.ResetDiceState();

        List<Dice> diceList = DiceManager.Instance.SpawnMultipleDice(
            6, 2, diceParent, new Vector3(0, 2, 0), Random.rotation
        );
    }

    public void EnableThrowDiceButton()
    {
        if (throwDiceButton != null)
            throwDiceButton.interactable = true;
    }
}
