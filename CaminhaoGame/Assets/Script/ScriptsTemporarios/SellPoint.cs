using UnityEngine;
using TMPro;
using System.Collections;

public class SellPoint : MonoBehaviour
{
    [Header("Interface")]
    [Tooltip("Arraste o texto que vai ficar fixo mostrando o Saldo")]
    public TextMeshProUGUI textoBanco;

    [Tooltip("Arraste o texto que vai aparecer e sumir com os avisos")]
    public TextMeshProUGUI textoVenda;

    public float tempoMensagemNaTela = 4f;

    [Header("Banco do Jogador")]
    public float dinheiroTotal = 0f;

    private int pecasDoCaminhaoNaArea = 0;
    private Coroutine rotinaTexto;

    void Start()
    {
        // Esconde a mensagem de venda no início
        if (textoVenda != null) textoVenda.enabled = false;

        // Atualiza o placar do banco logo que o jogo começa
        AtualizarTextoBanco();
    }

    void AtualizarTextoBanco()
    {
        if (textoBanco != null)
        {
            textoBanco.text = $"<b>Saldo no Banco: <color=#FFD700>${dinheiroTotal}</color></b>";
            textoBanco.enabled = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 1. VENDA PELO CAMINHÃO
        if (other.CompareTag("Truck"))
        {
            pecasDoCaminhaoNaArea++;
            if (pecasDoCaminhaoNaArea == 1)
            {
                TruckInventory inv = other.GetComponentInChildren<TruckInventory>();
                if (inv != null) VenderItensDoCaminhao(inv);
            }
        }
        // 2. VENDA DIRETA (Player)
        else if (other.CompareTag("Player"))
        {
            PlayerInteraction playerInteract = other.GetComponent<PlayerInteraction>();
            if (playerInteract != null && playerInteract.TaSegurandoAlgo())
            {
                Item itemNaMao = playerInteract.ObterItemSegurado();
                if (itemNaMao != null)
                {
                    playerInteract.ForcarSoltar();
                    VenderItemUnico(itemNaMao);
                }
            }
        }
        // 3. VENDA POR ARREMESSO
        else
        {
            Item itemSolto = other.GetComponent<Item>();
            if (itemSolto != null && !itemSolto.sendoSegurado)
            {
                VenderItemUnico(itemSolto);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Truck"))
        {
            pecasDoCaminhaoNaArea--;
            if (pecasDoCaminhaoNaArea < 0) pecasDoCaminhaoNaArea = 0;
        }
    }

    void VenderItensDoCaminhao(TruckInventory inv)
    {
        float lucroDaViagem = inv.valorTotal;
        int quantidade = inv.itensNaCacamba.Count;

        if (quantidade > 0)
        {
            dinheiroTotal += lucroDaViagem;
            AtualizarTextoBanco(); // Sobe o dinheiro no placar fixo

            foreach (Item item in inv.itensNaCacamba)
            {
                if (item != null) Destroy(item.gameObject);
            }

            inv.itensNaCacamba.Clear();
            inv.valorTotal = 0;

            MostrarMensagem($"Viagem Finalizada!\n{quantidade} itens vendidos.\n<color=green>+ ${lucroDaViagem}</color>");
        }
        else
        {
            MostrarMensagem("A caçamba está vazia!");
        }
    }

    void VenderItemUnico(Item item)
    {
        float lucro = item.valorAtual;
        dinheiroTotal += lucro;
        AtualizarTextoBanco(); // Sobe o dinheiro no placar fixo

        Destroy(item.gameObject);

        MostrarMensagem($"Venda Avulsa!\n<color=green>+ ${lucro}</color>");
    }

    void MostrarMensagem(string msg)
    {
        if (textoVenda != null)
        {
            if (rotinaTexto != null) StopCoroutine(rotinaTexto);
            rotinaTexto = StartCoroutine(RotinaMensagem(msg));
        }
    }

    IEnumerator RotinaMensagem(string msg)
    {
        textoVenda.text = msg;
        textoVenda.enabled = true;

        yield return new WaitForSeconds(tempoMensagemNaTela);

        textoVenda.enabled = false;
    }
}