using UnityEngine;
using Unity.Cinemachine;

public class TruckEntry : MonoBehaviour, IInteractable
{
    [Header("Partes do Player (Para desligar sem matar a câmera)")]
    public GameObject playerGraficos;     // O visual do boneco (PlayerBobs ou Mesh)
    public CharacterController playerCollider; // A física (está no objeto Pai)
    public PlayerControl playerMovimento; // O script de andar (está no objeto Pai)

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

    void Start()
    {
        // Garante estado inicial
        scriptMotor.enabled = false;
        if (cameraBrain) cameraBrain.enabled = false;
        if (textoAviso) textoAviso.SetActive(false);
    }

    void Update()
    {
        if (dirigindo && Input.GetKeyDown(KeyCode.E))
        {
            SairDoCaminhao();
        }
    }

    public void Interact(PlayerInteract playerScript)
    {
        EntrarNoCaminhao();
    }

    void EntrarNoCaminhao()
    {
        dirigindo = true;

        // --- A MUDANÇA ESTÁ AQUI ---
        // Em vez de desligar o objeto todo, desligamos só o que precisa
        if (playerGraficos) playerGraficos.SetActive(false); // Some o corpo
        if (playerCollider) playerCollider.enabled = false;  // Desliga colisão
        if (playerMovimento) playerMovimento.enabled = false; // Trava o andar
        // A Câmera continua ligada porque o Pai continua ativo!

        // Configura Cinemachine
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

        // Teleporta o Pai (que nunca foi desligado)
        // Precisamos mover o Transform do player, pois o CharacterController estava desligado
        playerMovimento.transform.position = localSair.position;

        // Reativa as partes
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