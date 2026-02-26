using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Referências")]
    public Camera mainCamera;
    public InteractionUI ui;

    [Header("Posição da Mão")]
    [Tooltip("Arraste o objeto HandPoint aqui!")]
    public Transform pontoSegurar;

    [Header("Interação")]
    public float distanciaInteracao = 4f;
    public float suavizacao = 15f;

    [Header("Forças")]
    public float forcaArremessoBase = 60f;

    [Header("Rotação")]
    public float velocidadeRotacao = 120f;
    public bool travarEixoY = true;
    public float snapAngulo = 90f;

    [Header("Segurança")]
    public float distanciaMaximaPerderItem = 4.5f;

    private Rigidbody rb;
    private GameObject item;
    private CharacterController controller;
    private bool modoRotacao = false;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 1. AVISO VISUAL (Raycast sai da câmera para mirar pelo centro da tela)
        if (rb == null)
        {
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, distanciaInteracao))
            {
                if (hit.collider.TryGetComponent(out Rigidbody hitRb) && ui != null)
                {
                    ui.texto.text = "<b>[E]</b> Pegar Objeto";
                    ui.texto.enabled = true;
                }
                else if (ui != null) ui.Esconder();
            }
            else if (ui != null) ui.Esconder();
        }

        // 2. PEGAR / SOLTAR
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (rb == null) TryGrab();
            else Drop();
        }

        if (rb == null) return;

        // 3. SEGURANDO O ITEM
        VerificarDistanciaItem();

        if (ui != null)
        {
            if (modoRotacao) ui.MostrarRotacao();
            else ui.MostrarSegurando();
        }

        // Toggle modo rotação (H)
        if (Input.GetKeyDown(KeyCode.H))
        {
            modoRotacao = !modoRotacao;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            if (!modoRotacao) SnapRotacao();
        }

        if (modoRotacao) RotacionarItem();

        // Arremessar com Botão Direito
        if (!modoRotacao && Input.GetMouseButtonDown(1)) Throw();
    }

    void FixedUpdate()
    {
        if (rb == null || modoRotacao || pontoSegurar == null) return;

        // O destino do item agora é EXATAMENTE a posição do HandPoint
        Vector3 destino = pontoSegurar.position;

        // Move o item suavemente para a mão
        rb.MovePosition(Vector3.Lerp(rb.position, destino, suavizacao * Time.fixedDeltaTime));
    }

    void TryGrab()
    {
        // A mira continua sendo pela câmera para facilitar pegar itens no chão
        if (!Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, distanciaInteracao))
            return;

        if (!hit.collider.TryGetComponent(out Rigidbody hitRb)) return;

        rb = hitRb;
        item = hit.collider.gameObject;

        rb.useGravity = false;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (item.TryGetComponent(out Item itemScript))
            itemScript.sendoSegurado = true;

        // CORREÇÃO: Ignora TODOS os colisores do item para o player não sair voando
        Collider[] colisoresDoItem = item.GetComponentsInChildren<Collider>();
        foreach (Collider col in colisoresDoItem)
        {
            Physics.IgnoreCollision(col, controller, true);
        }
    }

    void Drop()
    {
        rb.freezeRotation = false;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.None;

        if (item.TryGetComponent(out Item itemScript))
            itemScript.sendoSegurado = false;

        // CORREÇÃO: Devolve a colisão para TODOS os colisores do item
        Collider[] colisoresDoItem = item.GetComponentsInChildren<Collider>();
        foreach (Collider col in colisoresDoItem)
        {
            Physics.IgnoreCollision(col, controller, false);
        }

        rb = null;
        item = null;
        modoRotacao = false;

        if (ui != null) ui.Esconder();
    }

    void Throw()
    {
        Rigidbody temp = rb;
        Drop();

        float forcaFinal = forcaArremessoBase / Mathf.Max(temp.mass, 0.1f);
        // Arremessa na direção que a câmera está olhando
        temp.AddForce(mainCamera.transform.forward * forcaFinal, ForceMode.Impulse);
    }

    void RotacionarItem()
    {
        float mx = Input.GetAxis("Mouse X") * velocidadeRotacao * Time.deltaTime;
        float my = Input.GetAxis("Mouse Y") * velocidadeRotacao * Time.deltaTime;

        Vector3 eixo = travarEixoY ? Vector3.up : mainCamera.transform.right;
        Quaternion rot = Quaternion.AngleAxis(mx, eixo) * Quaternion.AngleAxis(-my, mainCamera.transform.right);

        rb.MoveRotation(rb.rotation * rot);
    }

    void SnapRotacao()
    {
        Vector3 euler = rb.rotation.eulerAngles;
        euler.x = Mathf.Round(euler.x / snapAngulo) * snapAngulo;
        euler.y = Mathf.Round(euler.y / snapAngulo) * snapAngulo;
        euler.z = Mathf.Round(euler.z / snapAngulo) * snapAngulo;
        rb.MoveRotation(Quaternion.Euler(euler));
    }

    void VerificarDistanciaItem()
    {
        if (pontoSegurar != null && Vector3.Distance(pontoSegurar.position, rb.position) > distanciaMaximaPerderItem)
            Drop();
    }
}