using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData data;
    public bool sendoSegurado = false;

    void FixedUpdate()
    {
        // Se for um item maldito, ele flutua levemente para assustar
        if (data != null && data.ehMaldito && !sendoSegurado)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * Mathf.Sin(Time.time * 2f) * 5f);
        }
    }
}