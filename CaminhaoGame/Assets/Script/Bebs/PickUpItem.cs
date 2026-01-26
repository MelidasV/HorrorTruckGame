using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    public float weight = 1f;

    public void Interact(PlayerInteract player)
    {
        ItemHolder holder = player.GetComponent<ItemHolder>();
        if (holder == null) return;

        holder.Pickup(this);
        gameObject.layer = 0;
    }
}
