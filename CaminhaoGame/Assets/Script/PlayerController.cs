using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 5f;
    public float gravity = -20f;

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
        // ===== INPUT =====
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(x, 0, z).normalized;

        // ===== GRAVIDADE =====
        if (controller.isGrounded && verticalVelocity.y < 0)
            verticalVelocity.y = -2f;

        verticalVelocity.y += gravity * Time.deltaTime;

        // ===== MOVIMENTO FINAL =====
        Vector3 finalMove =
            (move * speed) +
            (Vector3.up * verticalVelocity.y);

        controller.Move(finalMove * Time.deltaTime);

        // ===== ROTACAO =====
        if (move.sqrMagnitude > 0.01f)
        {
            transform.forward = move;
        }

        // ===== ANIMACAO =====
        animator.SetFloat("Speed", move.magnitude);
    }
}
