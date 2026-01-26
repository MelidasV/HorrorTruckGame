using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    public float sensibilidade = 200f;
    public Transform playerCorpo; // Arraste o PLAYER (Pai) para cá
    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibilidade * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidade * Time.deltaTime;

        // CIMA E BAIXO (Gira só a câmera)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // ESQUERDA E DIREITA (Gira o corpo todo)
        // Isso impede que você veja suas próprias costas
        playerCorpo.Rotate(Vector3.up * mouseX);
    }
}