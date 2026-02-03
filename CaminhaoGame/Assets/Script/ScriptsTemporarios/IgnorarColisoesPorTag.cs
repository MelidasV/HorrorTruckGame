using UnityEngine;

public class IgnorarColisoesPorTag : MonoBehaviour
{
    [Header("Configuração")]
    public string tagDoCaminhao = "Truck"; // Verifique se sua Tag é "Truck" ou "truck"

    void Start()
    {
        // 1. Tenta rodar a correção imediatamente
        IgnorarTudo();

        // 2. Tenta rodar de novo depois de 0.5 segundos (Garante que tudo carregou)
        Invoke("IgnorarTudo", 0.5f);
    }

    void IgnorarTudo()
    {
        // Pega as rodas deste objeto (ou filhos)
        WheelCollider[] minhasRodas = GetComponentsInChildren<WheelCollider>();

        // Busca o caminhão pela TAG específica
        GameObject caminhao = GameObject.FindGameObjectWithTag(tagDoCaminhao);

        if (caminhao == null)
        {
            Debug.LogError($"ERRO: Não encontrei nenhum objeto com a tag '{tagDoCaminhao}' na cena!");
            return;
        }

        // Pega todos os colisores do caminhão encontrado
        Collider[] colisoresDoCaminhao = caminhao.GetComponentsInChildren<Collider>();

        int contador = 0;

        foreach (WheelCollider roda in minhasRodas)
        {
            foreach (Collider colisor in colisoresDoCaminhao)
            {
                // Ignora colisão entre a roda e qualquer parte do caminhão
                Physics.IgnoreCollision(roda, colisor, true);
                contador++;
            }
        }

        Debug.Log($"Sucesso! {contador} regras de colisão aplicadas no caminhão com tag '{tagDoCaminhao}'.");
    }
}