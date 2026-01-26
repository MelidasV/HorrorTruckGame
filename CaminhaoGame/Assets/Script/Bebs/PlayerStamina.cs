using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina")]
    public float maxStamina = 50f;
    public float CurrentStamina = 50f;

    public float drainRate = 25f;
    public float regenRate = 15f;

    [Header("Exhaustion")]
    public float exhaustedThreshold = 5f;
    public float recoverThreshold = 25f;

    [Header("Carry Weight")]
    public float carryWeight = 0f;
    public float weightDrainMultiplier = 0.15f;

    [Header("Exhaustion Delay")]
    public float exhaustedRecoverDelay = 2.5f;

    public float exhaustedTimer;


    public bool IsExhausted { get; private set; }

    void Start()
    {
        CurrentStamina = maxStamina;
        IsExhausted = false;
    }

    public bool CanRun()
    {
        return !IsExhausted && CurrentStamina > exhaustedThreshold;
    }

    public void Drain()
    {
        float weightPenalty = 1f + (carryWeight * weightDrainMultiplier);
        CurrentStamina -= drainRate * weightPenalty * Time.deltaTime;

        ClampStamina();

        if (CurrentStamina <= exhaustedThreshold)
            IsExhausted = true;
    }

    public void Recover(bool isMoving)
    {
        if (IsExhausted)
        {
            if (isMoving)
            {
                exhaustedTimer = 0f;
                return;
            }

            exhaustedTimer += Time.deltaTime;

            if (exhaustedTimer < exhaustedRecoverDelay)
                return;
        }

        CurrentStamina += regenRate * Time.deltaTime;
        ClampStamina();

        if (IsExhausted && CurrentStamina >= recoverThreshold)
        {
            IsExhausted = false;
            exhaustedTimer = 0f;
        }
    }

    public void AddCarryWeight(float weight)
    {
        carryWeight = Mathf.Max(0, carryWeight + weight);
    }

    void ClampStamina()
    {
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0, maxStamina);
    }
}