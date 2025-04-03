using UnityEngine;

public class Dice_8 : Dice
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
            -diceTransform.forward,
            (diceTransform.up + diceTransform.forward).normalized,
            (-diceTransform.up + -diceTransform.forward).normalized
        };

        int[] faceValues = { 1, 8, 2, 7, 3, 6, 4, 5 };

        int topFaceIndex = GetTopFaceIndex(faceNormals);
        int topFaceValue = faceValues[topFaceIndex];

        Debug.Log("정8면체 윗면 숫자: " + topFaceValue);
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
