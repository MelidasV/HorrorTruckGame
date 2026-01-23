using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData data;
    public bool sendoSegurado = false;

    // Valor atual do item (Começa cheio e cai se bater)
    [HideInInspector]
    public float valorAtual;

    [Header("Fragilidade")]
    public float forcaParaDanificar = 6f; // Força mínima da batida para perder valor
    public float perdaPorBatida = 15f;    // Quantos $$ perde a cada batida forte

    void Start()
    {
        // 1. Ao nascer, o item vale o preço cheio do arquivo
        if (data != null)
        {
            valorAtual = data.valor;
        }
    }

    // 2. Detecta colisão (Batidas no chão, paredes ou caminhão)
    void OnCollisionEnter(Collision collision)
    {
        // Só toma dano se tiver dados e não estiver na mão do player (opcional)
        if (data == null) return;

        // Verifica a força da pancada (magnitude)
        if (collision.relativeVelocity.magnitude > forcaParaDanificar)
        {
            // Desconta o valor
            valorAtual -= perdaPorBatida;

            // Não deixa o valor ficar negativo (mínimo é $0)
            if (valorAtual < 0) valorAtual = 0;

            Debug.Log($"BATEU FORTE! Perdeu valor. Valor restante: ${valorAtual}");

            // DICA: Aqui você pode colocar um som de vidro quebrando ou amassado
        }
    }

    void FixedUpdate()
    {
        // Lógica do item maldito (Manteve igual)
        if (data != null && data.ehMaldito && !sendoSegurado)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * Mathf.Sin(Time.time * 2f) * 5f);
        }
    }
}