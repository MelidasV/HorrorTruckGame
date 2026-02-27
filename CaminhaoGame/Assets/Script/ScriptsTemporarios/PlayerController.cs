using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 5f;
    [Tooltip("Aumente esse valor (ex: 150) para ele girar mais rápido com A e D")]
    public float rotationSpeed = 150f;
    public float gravity = -19.62f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Trava de segurança para quando entra no caminhão
        if (!controller.enabled) return;

        float x = Input.GetAxisRaw("Horizontal"); // A e D
        float z = Input.GetAxisRaw("Vertical");   // W e S

        // =========================================================
        // 1. ROTAÇÃO NO PRÓPRIO EIXO (A e D)
        // =========================================================
        if (x != 0)
        {
            transform.Rotate(Vector3.up * x * rotationSpeed * Time.deltaTime);
        }

        // =========================================================
        // 2. MOVIMENTO PARA FRENTE E PARA TRÁS (W e S)
        // =========================================================
        // "transform.forward" garante que ele ande para onde o nariz dele aponta
        Vector3 moveDir = transform.forward * z;

        if (z != 0)
        {
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        // =========================================================
        // 3. ANIMAÇÃO E GRAVIDADE
        // =========================================================
        if (animator != null)
        {
            // Mathf.Abs garante que a animação de "andar" toque mesmo se der ré (Z negativo)
            animator.SetFloat("Speed", Mathf.Abs(z));
        }

        if (controller.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;
        }

        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }
}