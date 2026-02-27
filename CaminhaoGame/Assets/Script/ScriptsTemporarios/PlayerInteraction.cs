using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Referências")]
    public InteractionUI ui;

    [Header("Posição da Mão")]
    public Transform pontoSegurar;

    [Header("Interação")]
    public float raioDeBusca = 3f;
    public float suavizacao = 15f;

    [Header("Forças e Segurança")]
    public float forcaArremessoBase = 60f;
    public float velocidadeRotacao = 120f;
    public float snapAngulo = 90f;
    public float distanciaMaximaPerderItem = 4.5f;

    private Rigidbody rb;
    private GameObject item;
    private CharacterController controller;
    private bool modoRotacao = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (rb == null)
        {
            Rigidbody itemPerto = EncontrarItemProximo();

            if (itemPerto != null && ui != null)
            {
                ui.texto.text = "<b>[E]</b> Pegar Objeto";
                ui.texto.enabled = true;
            }
            else if (ui != null) ui.Esconder();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (rb == null) TryGrab();
            else Drop();
        }

        if (rb == null) return;

        if (pontoSegurar != null && Vector3.Distance(pontoSegurar.position, rb.position) > distanciaMaximaPerderItem)
            Drop();

        if (ui != null)
        {
            if (modoRotacao) ui.MostrarRotacao();
            else ui.MostrarSegurando();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            modoRotacao = !modoRotacao;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            if (!modoRotacao) SnapRotacao();
        }

        if (modoRotacao) RotacionarItem();
        if (!modoRotacao && Input.GetMouseButtonDown(1)) Throw();
    }

    void FixedUpdate()
    {
        if (rb == null || modoRotacao || pontoSegurar == null) return;
        rb.MovePosition(Vector3.Lerp(rb.position, pontoSegurar.position, suavizacao * Time.fixedDeltaTime));
    }

    Rigidbody EncontrarItemProximo()
    {
        Collider[] colisores = Physics.OverlapSphere(transform.position, raioDeBusca);
        Rigidbody itemMaisProximo = null;
        float menorDistancia = Mathf.Infinity;

        foreach (Collider col in colisores)
        {
            if (col.transform.root == transform.root) continue;
            if (col.isTrigger) continue;

            Rigidbody colRb = col.GetComponent<Rigidbody>();
            if (colRb != null && col.GetComponent<Item>() != null)
            {
                float dist = Vector3.Distance(transform.position, col.transform.position);
                if (dist < menorDistancia)
                {
                    menorDistancia = dist;
                    itemMaisProximo = colRb;
                }
            }
        }
        return itemMaisProximo;
    }

    void TryGrab()
    {
        Rigidbody itemAlvo = EncontrarItemProximo();
        if (itemAlvo == null) return;

        PegarItemDireto(itemAlvo); // Reutiliza a nova função abaixo
    }

    void Drop()
    {
        if (rb == null) return;

        rb.freezeRotation = false;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.None;

        if (item.TryGetComponent(out Item itemScript)) itemScript.sendoSegurado = false;

        foreach (Collider col in item.GetComponentsInChildren<Collider>())
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

        Vector3 direcao = mainCamera != null ? mainCamera.transform.forward : transform.forward;
        temp.AddForce(direcao * (forcaArremessoBase / Mathf.Max(temp.mass, 0.1f)), ForceMode.Impulse);
    }

    void RotacionarItem()
    {
        if (mainCamera == null) return;
        float mx = Input.GetAxis("Mouse X") * velocidadeRotacao * Time.deltaTime;
        float my = Input.GetAxis("Mouse Y") * velocidadeRotacao * Time.deltaTime;
        Quaternion rot = Quaternion.AngleAxis(mx, Vector3.up) * Quaternion.AngleAxis(-my, mainCamera.transform.right);
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

    // =========================================================
    // FUNÇÕES NOVAS PARA O CAMINHÃO USAR
    // =========================================================
    public bool TaSegurandoAlgo()
    {
        return rb != null;
    }

    public void PegarItemDireto(Rigidbody novoItemRb)
    {
        if (rb != null) Drop(); // Se já tem algo na mão, solta para pegar o novo

        rb = novoItemRb;
        item = rb.gameObject;

        rb.useGravity = false;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (item.TryGetComponent(out Item itemScript)) itemScript.sendoSegurado = true;

        foreach (Collider col in item.GetComponentsInChildren<Collider>())
        {
            Physics.IgnoreCollision(col, controller, true);
        }
    }
}