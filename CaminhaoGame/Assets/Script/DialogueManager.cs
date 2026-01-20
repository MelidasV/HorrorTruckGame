using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Configuração da UI")]
    public GameObject painelDialogo;      // O fundo preto do diálogo
    public TextMeshProUGUI textoNome;     // Onde aparece o nome do NPC
    public TextMeshProUGUI textoFala;     // Onde aparece a frase
    public TextMeshProUGUI avisoInteracao; // Texto solto na tela: "Pressione E"

    [Header("Configurações")]
    public float distanciaInteracao = 3f; // Distância para falar
    public bool travarMovimentoAoFalar = true; // Se quiser falar andando, desmarque isso

    // Referências internas (Preenchidas automaticamente)
    private PlayerController playerMovement;
    private PlayerInteraction playerInteraction;
    private FPSCamera playerCamera;
    private Camera mainCam;

    // Estado do Diálogo
    private string[] falasAtuais;
    private int indexFala;
    private bool emDialogo = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        painelDialogo.SetActive(false);
        if (avisoInteracao) avisoInteracao.enabled = false;
    }

    void Start()
    {
        // Encontra os scripts do jogador automaticamente para não precisar arrastar
        playerMovement = FindFirstObjectByType<PlayerController>();
        playerInteraction = FindFirstObjectByType<PlayerInteraction>();
        playerCamera = FindFirstObjectByType<FPSCamera>();
        mainCam = Camera.main;
    }

    void Update()
    {
        if (emDialogo)
        {
            // Lógica para passar as falas
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            {
                ProximaFala();
            }
        }
        else
        {
            // Lógica de DETECÇÃO (O Radar)
            DetectarNPC();
        }
    }

    // --- NOVA FUNÇÃO QUE SUBSTITUI A LÓGICA NO PLAYER ---
    void DetectarNPC()
    {
        // Cria um raio do centro da câmera
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        bool achouNPC = false;

        // Verifica se bateu em algo na distância configurada
        if (Physics.Raycast(ray, out hit, distanciaInteracao))
        {
            // Tenta pegar o script NPC do objeto que estamos olhando
            NPC npc = hit.collider.GetComponent<NPC>();

            if (npc != null)
            {
                achouNPC = true;

                // Mostra o texto "Pressione E" se ele existir
                if (avisoInteracao)
                {
                    avisoInteracao.text = "Pressione <b>E</b> para falar";
                    avisoInteracao.enabled = true;
                }

                // Se apertar E, começa o diálogo
                if (Input.GetKeyDown(KeyCode.E))
                {
                    IniciarDialogo(npc);
                }
            }
        }

        // Se não estiver olhando para nenhum NPC, esconde o aviso
        if (!achouNPC && avisoInteracao != null)
        {
            avisoInteracao.enabled = false;
        }
    }

    void IniciarDialogo(NPC npc)
    {
        // --- SEGURANÇA NOVA ---
        // Se o NPC não tiver falas ou a lista for nula, cancela tudo para não dar erro.
        if (npc.falas == null || npc.falas.Length == 0)
        {
            Debug.LogWarning("Esse NPC não tem falas cadastradas!");
            return;
        }
        // ----------------------

        emDialogo = true;
        painelDialogo.SetActive(true);
        if (avisoInteracao) avisoInteracao.enabled = false;

        textoNome.text = npc.nomeNPC;
        falasAtuais = npc.falas;
        indexFala = 0;

        MostrarFalaAtual();

        if (travarMovimentoAoFalar) AlternarControlesPlayer(false);
    }

    void MostrarFalaAtual()
    {
        textoFala.text = falasAtuais[indexFala];
    }

    void ProximaFala()
    {
        indexFala++;
        if (indexFala < falasAtuais.Length)
        {
            MostrarFalaAtual();
        }
        else
        {
            FecharDialogo();
        }
    }

    void FecharDialogo()
    {
        emDialogo = false;
        painelDialogo.SetActive(false);

        // Devolve o controle ao jogador
        if (travarMovimentoAoFalar) AlternarControlesPlayer(true);
    }

    // Função auxiliar que desliga/liga os componentes do Player
    void AlternarControlesPlayer(bool estado)
    {
        if (playerMovement) playerMovement.enabled = estado;

        if (playerInteraction)
        {
            playerInteraction.enabled = estado;
            // Garante que não fique bugado segurando algo invisível
            if (!estado) playerInteraction.StopAllCoroutines();
        }

        if (playerCamera) playerCamera.enabled = estado;

        // Solta o mouse para poder clicar se quiser, ou trava de novo para jogar
        Cursor.lockState = estado ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !estado;
    }
}