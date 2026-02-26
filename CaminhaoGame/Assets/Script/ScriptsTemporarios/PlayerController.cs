using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 5f;
    public float rotationSpeed = 15f; // Controla a velocidade que ele vira o corpo
    public float gravity = -19.62f;

    private Transform cameraTransform;
    private CharacterController controller;
    private Animator animator;
    private Vector3 verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        if (cameraTransform == null) return;

        // Pega a direção exata para onde a câmera está olhando (Frente e Lados)
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        // Ignora a inclinação da câmera (para o personagem não tentar entrar na terra)
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // Calcula a direção final em que o boneco vai andar
        Vector3 moveDir = (camForward * z + camRight * x).normalized;

        if (moveDir.magnitude >= 0.1f)
        {
            // Faz o boneco olhar para a direção em que está andando (sem bugs de ângulo)
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move o boneco
            controller.Move(moveDir * speed * Time.deltaTime);
        }

        // ===== ANIMAÇÃO =====
        if (animator != null)
            animator.SetFloat("Speed", moveDir.magnitude);

        // ===== GRAVIDADE =====
        if (controller.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;
        }

        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }
}