using UnityEngine;

public class DiceCameraController : MonoBehaviour
{
    public Camera mainCamera;  // �⺻ ī�޶�
    public Camera diceCamera;  // �ֻ��� ���� ī�޶�
    public Transform diceTransform; // �ֻ��� ��ġ
    private bool isFollowing = false;

    void Start()
    {
        // ó������ �⺻ ī�޶� Ȱ��ȭ, �ֻ��� ī�޶�� ��Ȱ��ȭ
        mainCamera.enabled = true;
        diceCamera.enabled = false;
    }

    public void ActivateDiceCamera()
    {
        if (diceCamera == null || mainCamera == null || diceTransform == null)
            return;

        mainCamera.enabled = false;
        diceCamera.enabled = true;
        isFollowing = true;

        // ���� �ð� �� �ٽ� �⺻ ī�޶�� ����
        Invoke("SwitchToMainCamera", 3f);
    }

    void Update()
    {
        if (isFollowing && diceTransform != null)
        {
            // �ֻ����� ���󰡵��� ���� (�ε巴�� �̵�)
            Vector3 targetPosition = diceTransform.position + new Vector3(0, 2, -3);
            diceCamera.transform.position = Vector3.Lerp(diceCamera.transform.position, targetPosition, Time.deltaTime * 5);
            diceCamera.transform.LookAt(diceTransform.position);
        }
    }

    void SwitchToMainCamera()
    {
        mainCamera.enabled = true;
        diceCamera.enabled = false;
        isFollowing = false;
    }
}
