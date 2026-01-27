using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI Components")]
    public GameObject painelDialogo;
    public TextMeshProUGUI textoNome;
    public TextMeshProUGUI textoFala;

    // Referências para o NOVO sistema de personagem
    private PlayerControl playerControl;
    private PlayerCamera playerCamera;

    private string[] falasAtuais;
    private int indexFala;
    private bool emDialogo = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        painelDialogo.SetActive(false);
    }

    void Start()
    {
        // Encontra os scripts novos automaticamente
        playerControl = FindFirstObjectByType<PlayerControl>();
        playerCamera = FindFirstObjectByType<PlayerCamera>();
    }

    void Update()
    {
        // Passar as falas com Espaço, Enter ou Clique
        if (emDialogo && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)))
        {
            ProximaFala();
        }
    }

    // --- AQUI ESTAVA O ERRO ---
    // Agora aceitamos o objeto 'NPC' inteiro como parâmetro
    public void IniciarDialogo(NPC npc)
    {
        // Segurança: se não tiver falas, cancela
        if (npc.falas == null || npc.falas.Length == 0) return;

        emDialogo = true;
        painelDialogo.SetActive(true);

        // Pega os dados direto do NPC que enviamos
        textoNome.text = npc.nomeNPC;
        falasAtuais = npc.falas;
        indexFala = 0;

        TravarPlayer(true); // Congela o novo player
        MostrarFalaAtual();
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
        TravarPlayer(false); // Libera o player
    }

    void TravarPlayer(bool travar)
    {
        // 1. Trava o Movimento (Script novo: PlayerControl)
        if (playerControl != null)
            playerControl.enabled = !travar;

        // 2. Trava a Câmera (Script novo: PlayerCamera)
        if (playerCamera != null)
            playerCamera.enabled = !travar;

        // 3. Libera o mouse para clicar no diálogo (se quiser)
        Cursor.lockState = travar ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = travar;
    }
}