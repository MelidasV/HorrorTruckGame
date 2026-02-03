using UnityEngine;
using Unity.Cinemachine;

public class TruckEntry : MonoBehaviour, IInteractable
{
    [Header("Partes do Player")]
    public GameObject playerGraficos;
    public CharacterController playerCollider;
    public PlayerControl playerMovimento;

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

    // --- NOVIDADE: Variável para evitar o clique duplo instantâneo ---
    private float tempoParaPoderSair = 0f;

    void Start()
    {
        scriptMotor.enabled = false;
        if (cameraBrain) cameraBrain.enabled = false;
        if (textoAviso) textoAviso.SetActive(false);
    }

    void Update()
    {
        // Agora verificamos 3 coisas:
        // 1. Se está dirigindo
        // 2. Se já passou o tempo de bloqueio (Time.time > tempoParaPoderSair)
        // 3. Se apertou E
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

    public void Interact(PlayerInteract playerScript)
    {
        EntrarNoCaminhao();
    }

    void EntrarNoCaminhao()
    {
        dirigindo = true;

        // --- AQUI ESTÁ A CORREÇÃO ---
        // Define que só pode sair daqui a 1 segundo.
        // Isso impede que o mesmo aperto de botão que te fez entrar, te faça sair.
        tempoParaPoderSair = Time.time + 1.0f;

        if (playerGraficos) playerGraficos.SetActive(false);
        if (playerCollider) playerCollider.enabled = false;
        if (playerMovimento) playerMovimento.enabled = false;

        if (playerCam) playerCam.Priority = 10;
        if (caminhaoCam) caminhaoCam.Priority = 20;
        if (cameraBrain) cameraBrain.enabled = true;

        scriptMotor.enabled = true;
        if (textoAviso) textoAviso.SetActive(false);
    }

    void SairDoCaminhao()
    {
        dirigindo = false;
        scriptMotor.enabled = false;

        if (caminhaoCam) caminhaoCam.Priority = 0;
        if (cameraBrain) cameraBrain.enabled = false;

        playerMovimento.transform.position = localSair.position;

        if (playerGraficos) playerGraficos.SetActive(true);
        if (playerCollider) playerCollider.enabled = true;
        if (playerMovimento) playerMovimento.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!dirigindo && other.CompareTag("Player") && textoAviso)
            textoAviso.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && textoAviso)
            textoAviso.SetActive(false);
    }
}