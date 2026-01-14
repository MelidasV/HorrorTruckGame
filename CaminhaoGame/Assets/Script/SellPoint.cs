using UnityEngine;

public class SellPoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Truck"))
        {
            TruckInventory inv = other.GetComponentInChildren<TruckInventory>();
            Debug.Log("Viagem Finalizada! Lucro: $" + inv.valorTotal);
            inv.itensNaCacamba.Clear();
            inv.valorTotal = 0;
            // Carregar próxima fase ou mostrar menu de upgrade
        }
    }
}