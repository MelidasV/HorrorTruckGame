using UnityEngine;
using Unity.Cinemachine;

public class TruckEntry : MonoBehaviour, IInteractable
{
    [Header("Partes do Player")]
    public GameObject playerGraficos;
    public CharacterController playerCollider;
    public PlayerController playerMovimento;

    [Header("Caminhão")]
    public TruckController scriptMotor;

    [Header("Interface")]
    public GameObject textoAviso;

    [Header("Câmeras")]
    public CinemachineCamera caminhaoCam;
    public CinemachineCamera playerCam;

    [Header("Saída")]
    public Transform localSair;

    private bool dirigindo = false;
    private float tempoParaPoderSair = 0f;

    void Start()
    {
        scriptMotor.enabled = false;
        if (textoAviso) textoAviso.SetActive(false);

        // Garante que o jogo comece com a câmera do Player ligada e a do caminhão desligada
        if (playerCam) playerCam.gameObject.SetActive(true);
        if (caminhaoCam) caminhaoCam.gameObject.SetActive(false);
    }

    void Update()
    {
        if (dirigindo && Time.time > tempoParaPoderSair && Input.GetKeyDown(KeyCode.E))
        {
            Rigidbody rbCaminhao = scriptMotor.GetComponent<Rigidbody>();
            if (rbCaminhao.linearVelocity.magnitude * 3.6f < 5f)
                SairDoCaminhao();
            else
                Debug.Log("Pare o caminhão para sair!");
        }
    }

    public void Interact(PlayerInteract playerScript)
    {
        EntrarNoCaminhao();
    }

    void EntrarNoCaminhao()
    {
        dirigindo = true;
        tempoParaPoderSair = Time.time + 1.0f;

        if (playerGraficos) playerGraficos.SetActive(false);
        if (playerCollider) playerCollider.enabled = false;
        if (playerMovimento) playerMovimento.enabled = false;

        // LIGA A CÂMERA DO CAMINHÃO E DESLIGA A DO PLAYER (Método à prova de falhas)
        if (playerCam) playerCam.gameObject.SetActive(false);
        if (caminhaoCam) caminhaoCam.gameObject.SetActive(true);

        scriptMotor.enabled = true;
        if (textoAviso) textoAviso.SetActive(false);
    }

    void SairDoCaminhao()
    {
        dirigindo = false;
        scriptMotor.enabled = false;

        // DEVOLVE A CÂMERA PARA O PLAYER
        if (playerCam) playerCam.gameObject.SetActive(true);
        if (caminhaoCam) caminhaoCam.gameObject.SetActive(false);

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