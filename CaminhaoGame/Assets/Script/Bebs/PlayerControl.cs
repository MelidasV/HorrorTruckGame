using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerControl : MonoBehaviour
{
    [Header("Movimento")]
    public float walkSpeed = 2.8f;
    public float runSpeed = 6f;
    public float rotationSpeed = 5f;
    public float backwardSpeedMultiplier = 0.7f;

    [Header("Fisica")]
    public float gravity = -20f;
    public float groundStick = -2f;

    CharacterController controller;
    PlayerStamina stamina;
    Vector3 velocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        stamina = GetComponent<PlayerStamina>();
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0, v);

        bool wantsRun = Input.GetKey(KeyCode.LeftShift);
        bool isRunning = wantsRun && stamina != null && stamina.CanRun();
        bool isMoving = input.sqrMagnitude > 0.1f;

        float speed = isRunning ? runSpeed : walkSpeed;

        if (input.z < 0)
            speed *= backwardSpeedMultiplier;

        if (stamina != null && stamina.IsExhausted)
            speed *= 0.6f;

        if (stamina != null)
        {
            if (isRunning)
                stamina.Drain();
            else
                stamina.Recover(isMoving);
        }

        Vector3 move = transform.forward * input.z + transform.right * input.x;
        move = Vector3.ClampMagnitude(move, 1f);
        move *= speed;

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = groundStick;

        velocity.y += gravity * Time.deltaTime;
        move.y = velocity.y;

        controller.Move(move * Time.deltaTime);

        if (input.z > 0.1f)
        {
            Vector3 lookDir = transform.forward * input.z + transform.right * input.x;
            lookDir.y = 0;

            if (lookDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }
}
