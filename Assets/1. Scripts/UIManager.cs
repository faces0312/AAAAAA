using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button throwDiceButton;
    public int diceType = 6; // 4, 6, 8 등으로 변경 가능
    public int diceCount = 2; // 생성할 주사위 수

    public Transform diceParent; // 주사위를 넣을 부모 오브젝트
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

        // 주사위 생성
        List<Dice> diceList = DiceManager.Instance.SpawnMultipleDice(
            diceType, diceCount, diceParent, startSpawnPos, Random.rotation
        );
    }
}
