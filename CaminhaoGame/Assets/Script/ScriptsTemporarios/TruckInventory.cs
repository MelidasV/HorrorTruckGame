using UnityEngine;
using System.Collections.Generic;

public class TruckInventory : MonoBehaviour
{
    [Header("Inventário")]
    public List<Item> itensNaCacamba = new List<Item>();
    public float valorTotal = 0;

    [Header("Interface (Opcional)")]
    public GameObject textoAvisoRetirada;

    private bool playerPerto = false;
    private PlayerInteraction playerScript; // Sabe quem é o player

    void Update()
    {
        CalcularValorTotal();

        // Se o player apertar R na caçamba
        if (playerPerto && Input.GetKeyDown(KeyCode.R))
        {
            RetirarItemParaAMao();
        }
    }

    void CalcularValorTotal()
    {
        valorTotal = 0;
        foreach (Item item in itensNaCacamba)
        {
            if (item != null) valorTotal += item.valorAtual;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        ChecarPlayer(other, true);
        TentarGuardarItem(other.GetComponent<Item>());
    }

    void OnTriggerStay(Collider other)
    {
        TentarGuardarItem(other.GetComponent<Item>());
    }

    void OnTriggerExit(Collider other)
    {
        ChecarPlayer(other, false);
    }

    void ChecarPlayer(Collider other, bool entrou)
    {
        if (other.CompareTag("Player"))
        {
            playerPerto = entrou;
            if (textoAvisoRetirada) textoAvisoRetirada.SetActive(entrou);

            // Pega ou solta a referência do script do player
            if (entrou) playerScript = other.GetComponent<PlayerInteraction>();
            else playerScript = null;
        }
    }

    void TentarGuardarItem(Item item)
    {
        if (item != null && !item.sendoSegurado && item.gameObject.activeInHierarchy)
        {
            if (!itensNaCacamba.Contains(item))
            {
                itensNaCacamba.Add(item);
                Debug.Log($"Item armazenado! Valor: ${item.valorAtual}");
                item.gameObject.SetActive(false); // Esconde o item
            }
        }
    }

    public void RetirarItemParaAMao()
    {
        if (itensNaCacamba.Count > 0)
        {
            // Verifica se o player já está com as mãos ocupadas
            if (playerScript != null && playerScript.TaSegurandoAlgo())
            {
                Debug.Log("Suas mãos estão cheias! Jogue o item atual fora primeiro.");
                return;
            }

            // Tira o último item da lista do caminhão
            int ultimoIndice = itensNaCacamba.Count - 1;
            Item item = itensNaCacamba[ultimoIndice];
            itensNaCacamba.RemoveAt(ultimoIndice);

            // Reativa o item no mundo
            item.gameObject.SetActive(true);

            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // O PULO DO GATO: Envia o item direto para a mão do jogador
            if (playerScript != null && rb != null)
            {
                // Teleporta pra mão instantaneamente antes de ligar a física da mão
                item.transform.position = playerScript.pontoSegurar.position;
                playerScript.PegarItemDireto(rb);
            }

            Debug.Log("Item sacado direto para a mão!");
        }
        else
        {
            Debug.Log("A caçamba está vazia!");
        }
    }
}