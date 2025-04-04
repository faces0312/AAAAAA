using UnityEngine;

public class Dice_6 : Dice
{
    protected override void DetermineDiceFace()
    {
        Transform diceTransform = diceRigidbody.transform;
        Vector3[] faceNormals = {
            diceTransform.up,
            -diceTransform.up,
            diceTransform.right,
            -diceTransform.right,
            diceTransform.forward,
            -diceTransform.forward
        };

        int[] faceValues = { 2, 5, 4, 3, 1, 6 };

        int topFaceIndex = GetTopFaceIndex(faceNormals);
        int topFaceValue = faceValues[topFaceIndex];

        Debug.Log("정6면체 윗면 숫자: " + topFaceValue);

        DiceManager.Instance._diceResult += topFaceValue;

        Debug.Log("주사위 숫자합: " + DiceManager.Instance._diceResult);
    }

    private int GetTopFaceIndex(Vector3[] normals)
    {
        int highestIndex = 0;
        float highestValue = -1f;

        for (int i = 0; i < normals.Length; i++)
        {
            if (normals[i].y > highestValue)
            {
                highestValue = normals[i].y;
                highestIndex = i;
            }
        }

        return highestIndex;
    }
}
