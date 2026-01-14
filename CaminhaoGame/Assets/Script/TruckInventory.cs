using UnityEngine;
using System.Collections.Generic;

public class TruckInventory : MonoBehaviour
{
    public List<ItemData> itensNaCacamba = new List<ItemData>();
    public float valorTotal = 0;

    void OnTriggerEnter(Collider other)
    {
        Item item = other.GetComponent<Item>();
        if (item != null)
        {
            itensNaCacamba.Add(item.data);
            valorTotal += item.data.valor;
            Debug.Log("Carga atualizada: $" + valorTotal);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Item item = other.GetComponent<Item>();
        if (item != null)
        {
            itensNaCacamba.Remove(item.data);
            valorTotal -= item.data.valor;
        }
    }
}