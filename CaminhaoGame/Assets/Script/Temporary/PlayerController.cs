using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 5f;
    public float gravity = -19.62f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (animator == null)
            Debug.LogError("Animator não encontrado no Player!");
    }

    void Update()
    {
        // Entrada top-down
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(x, 0, z);

        // Movimento horizontal
        controller.Move(move.normalized * speed * Time.deltaTime);

        // Rotação na direção do movimento
        if (move.sqrMagnitude > 0.01f)
        {
            transform.forward = move;
        }

        // ===== ANIMAÇÃO =====
        animator.SetFloat("Speed", move.magnitude);

        // ===== GRAVIDADE =====
        if (controller.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;
        }

        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }
}

