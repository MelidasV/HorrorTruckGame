using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction")]
    public Transform originPoint;
    public float interactRadius = 1.2f;
    public LayerMask interactLayer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        if (!originPoint)
        {
            Debug.LogWarning("OriginPoint não atribuído!");
            return;
        }

        Collider[] hits = Physics.OverlapSphere(
            originPoint.position,
            interactRadius,
            interactLayer
        );

        if (hits.Length == 0)
            return;

        Collider closest = null;
        float minDist = Mathf.Infinity;

        foreach (Collider col in hits)
        {
            float dist = Vector3.Distance(originPoint.position, col.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = col;
            }
        }

        if (closest != null)
        {
            IInteractable interactable =
                closest.GetComponentInParent<IInteractable>();

            interactable?.Interact(this);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!originPoint) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(originPoint.position, interactRadius);
    }
}
