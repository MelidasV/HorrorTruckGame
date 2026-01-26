using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;

    [Header("Follow")]
    public float followSpeed = 6f;
    public Vector3 offset = new Vector3(0, 1.6f, -3.5f);

    [Header("Look")]
    public float lookSpeed = 3f;
    public float maxAngle = 30f;

    float currentY;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);

        float mouseX = Input.GetAxis("Mouse X");
        currentY += mouseX * lookSpeed;
        currentY = Mathf.Clamp(currentY, -maxAngle, maxAngle);

        Quaternion rot = Quaternion.Euler(0, currentY, 0);
        transform.rotation = Quaternion.Slerp( transform.rotation,rot, lookSpeed * Time.deltaTime );
    }
}
