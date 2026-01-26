using UnityEngine;

public class CameraBreathing : MonoBehaviour
{
    public PlayerStamina stamina;

    [Header("Breathing")]
    public float normalIntensity = 0.02f;
    public float tiredIntensity = 0.05f;
    public float exhaustedIntensity = 0.1f;

    public float breathSpeed = 1.5f;

    Vector3 startLocalPos;
    float timer;

    void Start()
    {
        startLocalPos = transform.localPosition;
    }

    void Update()
    {
        if (!stamina) return;

        float intensity = normalIntensity;

        if (stamina.IsExhausted)
            intensity = exhaustedIntensity;
        else if (stamina.CurrentStamina < stamina.maxStamina)
            intensity = tiredIntensity;

        timer += Time.deltaTime * breathSpeed;

        float offsetY = Mathf.Sin(timer) * intensity;
        float offsetX = Mathf.PerlinNoise(Time.time * 1.5f, 0f) * intensity * 0.5f;

        transform.localPosition = startLocalPos + new Vector3(offsetX, offsetY, 0);
    }
}