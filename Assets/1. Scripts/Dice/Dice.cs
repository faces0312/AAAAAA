using UnityEngine;
using System;

public abstract class Dice : MonoBehaviour
{
    protected Rigidbody diceRigidbody;
    protected bool hasStopped = false;
    public event Action OnDiceStopped; // 주사위 멈춤 이벤트

    protected virtual void Awake()
    {
        diceRigidbody = GetComponent<Rigidbody>();
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
        if (!hasStopped && diceRigidbody.linearVelocity.magnitude < 0.1f && diceRigidbody.angularVelocity.magnitude < 0.1f)
        {
            hasStopped = true;
            DetermineDiceFace();
            OnDiceStopped?.Invoke();
        }
    }

    protected abstract void DetermineDiceFace();
}
