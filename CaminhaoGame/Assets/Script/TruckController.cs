using UnityEngine;

public class TruckController : MonoBehaviour
{
    public WheelCollider[] rodasTracao;
    public WheelCollider[] rodasDirecao;
    public float torqueMotor = 1500f;
    public bool playerConduzindo = false;

    void Update()
    {
        if (!playerConduzindo) return;

        float v = Input.GetAxis("Vertical") * torqueMotor;
        float h = Input.GetAxis("Horizontal") * 30f;

        foreach (var r in rodasTracao) r.motorTorque = v;
        foreach (var r in rodasDirecao) r.steerAngle = h;
    }
}