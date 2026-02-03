using UnityEngine;

public class IgnorarColisaoRodas : MonoBehaviour
{
    // Se quiser garantir, pode arrastar o corpo do caminhão aqui no Inspector.
    // Se deixar vazio, ele procura pela TAG "Truck".
    public GameObject caminhaoEspecifico;

    void Start()
    {
        // 1. Pega o colisor desta roda (WheelCollider)
        Collider meuCollider = GetComponent<Collider>();

        if (meuCollider == null)
        {
            Debug.LogError("Este script precisa estar no mesmo objeto do WheelCollider!");
            return;
        }

        // 2. Encontra os colisores do caminhão para ignorar
        if (caminhaoEspecifico != null)
        {
            IgnorarObjeto(meuCollider, caminhaoEspecifico);
        }
        else
        {
            // Busca automática pela TAG "Truck"
            GameObject[] caminhoes = GameObject.FindGameObjectsWithTag("Truck");

            foreach (GameObject caminhao in caminhoes)
            {
                IgnorarObjeto(meuCollider, caminhao);
            }
        }
    }

    void IgnorarObjeto(Collider roda, GameObject veiculo)
    {
        // Pega TODOS os colisores do caminhão (BoxCollider, MeshCollider, etc)
        Collider[] colisoresDoCaminhao = veiculo.GetComponentsInChildren<Collider>();

        foreach (Collider colisorCaminhao in colisoresDoCaminhao)
        {
            // A mágica acontece aqui:
            // Se o colisor não for a própria roda, ignora a colisão entre eles.
            if (colisorCaminhao != roda)
            {
                Physics.IgnoreCollision(roda, colisorCaminhao);
            }
        }

        Debug.Log($"Roda {gameObject.name} agora ignora colisões com {veiculo.name}");
    }
}