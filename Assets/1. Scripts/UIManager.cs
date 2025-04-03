using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button throwDiceButton;
    public int diceType = 6; // 4, 6, 8 등으로 변경 가능

    private void Start()
    {
        throwDiceButton.onClick.AddListener(OnThrowDiceClicked);
    }

    private void OnThrowDiceClicked()
    {
        Vector3 spawnPos = new Vector3(0, 2, 0);
        Quaternion spawnRot = Random.rotation;

        Dice dice = DiceManager.Instance.SpawnDice(diceType, spawnPos, spawnRot);
        if (dice != null)
        {
            dice.ThrowDice();
        }
    }
}
