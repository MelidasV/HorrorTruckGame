using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    public Transform holdPoint;

    PickupItem currentItem;

    PlayerStamina stamina;
    CameraEffects camFX;

    void Awake()
    {
        stamina = GetComponent<PlayerStamina>();
        camFX = GetComponentInChildren<CameraEffects>();
    }

    public void Pickup(PickupItem item)
    {
        if (currentItem != null) return;

        currentItem = item;

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        item.transform.SetParent(holdPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        stamina?.AddCarryWeight(item.weight);
        camFX?.SetCarryStress(item.weight);
    }
}