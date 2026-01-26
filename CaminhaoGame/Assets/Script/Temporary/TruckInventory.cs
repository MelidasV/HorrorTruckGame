using UnityEngine;
using System.Collections.Generic;

public class TruckInventory : MonoBehaviour
{
    // MUDANÇA IMPORTANTE: A lista agora guarda o "Item" (script do objeto)
    // para podermos ler o 'valorAtual' dele e não o valor fixo do Data.
    public List<Item> itensNaCacamba = new List<Item>();

    public float valorTotal = 0;

    void Update()
    {
        // Recalcula o valor a cada frame. 
        // Isso garante que se um item bater e quebrar LÁ DENTRO, o valor total cai na hora.
        CalcularValorTotal();
    }

    void CalcularValorTotal()
    {
        valorTotal = 0;
        // Varre a lista somando as "etiquetas" atuais de cada item
        foreach (Item item in itensNaCacamba)
        {
            if (item != null)
            {
                valorTotal += item.valorAtual;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Procura pelo script "Item" no objeto que entrou
        Item item = other.GetComponent<Item>();

        if (item != null)
        {
            // Adiciona na lista se já não estiver lá
            if (!itensNaCacamba.Contains(item))
            {
                itensNaCacamba.Add(item);
                Debug.Log($"Item adicionado! Valor: ${item.valorAtual}");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        Item item = other.GetComponent<Item>();

        if (item != null)
        {
            // Remove da lista se sair da caçamba
            if (itensNaCacamba.Contains(item))
            {
                itensNaCacamba.Remove(item);
                Debug.Log("Item saiu da caçamba.");
            }
        }
    }
}