using UnityEngine;
using Unity.Cinemachine;

public class TruckEntry : MonoBehaviour, IInteractable
{
    [Header("Partes do Player")]
    public GameObject playerGraficos;
    public CharacterController playerCollider;
    public PlayerController playerMovimento; // Atualizado para o seu script novo

    [Header("Caminhão")]
    public TruckController scriptMotor;

    [Header("Interface")]
    public GameObject textoAviso;

    [Header("Câmeras")]
    public CinemachineBrain cameraBrain;
    public CinemachineCamera caminhaoCam;
    public CinemachineCamera playerCam;

    [Header("Saída")]
    public Transform localSair;

    private bool dirigindo = false;
    private bool pertoDaPorta = false; // NOVA VARIÁVEL
    private float tempoParaPoderSair = 0f;

    void Start()
    {
        scriptMotor.enabled = false;
        if (textoAviso) textoAviso.SetActive(false);
    }

    void Update()
    {
        // 1. PARA ENTRAR (Nova lógica independente de mira!)
        if (!dirigindo && pertoDaPorta && Input.GetKeyDown(KeyCode.E))
        {
            EntrarNoCaminhao();
        }

        // 2. PARA SAIR
        if (dirigindo && Time.time > tempoParaPoderSair && Input.GetKeyDown(KeyCode.E))
        {
            Rigidbody rbCaminhao = scriptMotor.GetComponent<Rigidbody>();
            float velocidadeAtual = rbCaminhao.linearVelocity.magnitude * 3.6f;

            if (velocidadeAtual < 5f)
            {
                SairDoCaminhao();
            }
            else
            {
                Debug.Log("Pare o caminhão para sair!");
            }
        }
    }

    // Mantemos isso apenas para não dar erro com o sistema antigo de NPCs
    public void Interact(PlayerInteract playerScript)
    {
        EntrarNoCaminhao();
    }

    void EntrarNoCaminhao()
    {
        if (dirigindo) return; // Evita bugar se apertar duas vezes rápido
        dirigindo = true;
        tempoParaPoderSair = Time.time + 1.0f;

        if (playerGraficos) playerGraficos.SetActive(false);
        if (playerCollider) playerCollider.enabled = false;
        if (playerMovimento) playerMovimento.enabled = false;

        if (playerCam) playerCam.gameObject.SetActive(false);
        if (caminhaoCam) caminhaoCam.gameObject.SetActive(true);

        scriptMotor.enabled = true;
        if (textoAviso) textoAviso.SetActive(false);
    }

    void SairDoCaminhao()
    {
        dirigindo = false;
        scriptMotor.enabled = false;

        if (playerCam) playerCam.gameObject.SetActive(true);
        if (caminhaoCam) caminhaoCam.gameObject.SetActive(false);

        playerMovimento.transform.position = localSair.position;

        if (playerGraficos) playerGraficos.SetActive(true);
        if (playerCollider) playerCollider.enabled = true;
        if (playerMovimento) playerMovimento.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pertoDaPorta = true;
            if (!dirigindo && textoAviso) textoAviso.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pertoDaPorta = false;
            if (textoAviso) textoAviso.SetActive(false);
        }
    }
}