using UnityEngine;
using System;

public abstract class Dice : MonoBehaviour
{
    protected Rigidbody diceRigidbody;
    protected bool hasStopped = false;
    protected bool isDestroyed = false;
    public bool IsDestroyed => isDestroyed;

    public event Action OnDiceStopped;

    private const float stopThreshold = 0.05f; // 멈춘 속도 기준

    protected virtual void Awake()
    {
        diceRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        ThrowDice();
    }

    public void ThrowDice()
    {
        if (diceRigidbody == null)
        {
            Debug.LogError("Rigidbody가 설정되지 않았습니다.");
            return;
        }

        hasStopped = false;

        diceRigidbody.linearVelocity = Vector3.zero;
        diceRigidbody.angularVelocity = Vector3.zero;

        diceRigidbody.AddForce(new Vector3(0, 10, 0), ForceMode.Impulse);
        diceRigidbody.AddTorque(UnityEngine.Random.insideUnitSphere * 10, ForceMode.Impulse);
    }

    protected virtual void Update()
    {
        if (isDestroyed) return; // 죽은 주사위는 처리 안 함

        if (!hasStopped && diceRigidbody != null)
        {
            // ⭐ 타이머 없이 바로 멈춤 판정
            if (diceRigidbody.linearVelocity.sqrMagnitude < stopThreshold * stopThreshold &&
                diceRigidbody.angularVelocity.sqrMagnitude < stopThreshold * stopThreshold)
            {
                hasStopped = true;
                DetermineDiceFace();
                OnDiceStopped?.Invoke();
            }
        }
    }

    public void MarkAsDestroyed()
    {
        isDestroyed = true;
        OnDiceStopped = null;
    }

    protected abstract void DetermineDiceFace();
}
