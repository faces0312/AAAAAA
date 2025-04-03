using UnityEngine;

public class DiceCameraController : MonoBehaviour
{
    public Camera mainCamera;  // 기본 카메라
    public Camera diceCamera;  // 주사위 전용 카메라
    public Transform diceTransform; // 주사위 위치
    private bool isFollowing = false;

    void Start()
    {
        // 처음에는 기본 카메라 활성화, 주사위 카메라는 비활성화
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

        // 일정 시간 후 다시 기본 카메라로 변경
        Invoke("SwitchToMainCamera", 3f);
    }

    void Update()
    {
        if (isFollowing && diceTransform != null)
        {
            // 주사위를 따라가도록 설정 (부드럽게 이동)
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
