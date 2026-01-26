using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    public float baseShake = 0.02f;
    float carryShake;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float shake = baseShake + carryShake;

        transform.localPosition = startPos +
            Random.insideUnitSphere * shake;
    }

    public void SetCarryStress(float weight)
    {
        carryShake = weight * 0.01f;
    }
}