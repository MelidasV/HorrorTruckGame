using UnityEngine;

[RequireComponent(typeof(Camera))]
public class StaminaFOV : MonoBehaviour
{
    public PlayerStamina stamina;

    [Header("FOV")]
    public float normalFOV = 60f;
    public float runFOV = 66f;
    public float exhaustedFOV = 54f;
    public float fovSmooth = 4f;

    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = normalFOV;
    }

    void Update()
    {
        if (!stamina) return;

        float targetFOV = normalFOV;

        if (stamina.IsExhausted)
            targetFOV = exhaustedFOV;
        else if (stamina.CurrentStamina < stamina.maxStamina)
            targetFOV = runFOV;

        cam.fieldOfView = Mathf.Lerp( cam.fieldOfView,targetFOV,fovSmooth * Time.deltaTime);
    }
}