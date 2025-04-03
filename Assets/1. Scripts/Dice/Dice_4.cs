using UnityEngine;

public class Dice_4 : Dice
{
    protected override void DetermineDiceFace()
    {
        Transform diceTransform = diceRigidbody.transform;
        Vector3[] faceNormals = {
            diceTransform.up,
            diceTransform.forward,
            -diceTransform.forward,
            -diceTransform.up
        };

        int[] faceValues = { 1, 2, 3, 4 };

        int topFaceIndex = GetTopFaceIndex(faceNormals);
        int topFaceValue = faceValues[topFaceIndex];

        Debug.Log("정4면체 윗면 숫자: " + topFaceValue);
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
