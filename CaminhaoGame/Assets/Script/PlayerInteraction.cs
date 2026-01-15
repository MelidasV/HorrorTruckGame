using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Referências")]
    public Camera mainCamera;

    [Header("Interação")]
    public float distanciaInteracao = 4f;

    [Header("Segurar Item")]
    public float distanciaSegurar = 2.5f;
    public float minDistancia = 1.5f;
    public float maxDistancia = 2.7f;
    public float scrollSpeed = 2f;
    public float suavizacao = 15f;

    [Header("Anti-Clipping")]
    public float raioColisao = 0.5f;
    public float alturaRayChao = 1.5f;

    [Header("Forças")]
    public float forcaArremessoBase = 60f;

    [Header("Rotação")]
    public float velocidadeRotacao = 120f;
    public bool travarEixoY = true;
    public float snapAngulo = 90f;

    private Rigidbody rb;
    private GameObject item;
    private CharacterController controller;
    public InteractionUI ui;


    private bool modoRotacao = false;

    void Start()
    {
        mainCamera ??= Camera.main;
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 🖱 PEGAR / SOLTAR
        if (Input.GetMouseButtonDown(0))
        {
            if (rb == null) TryGrab();
            else Drop();
        }

        if (rb == null) return;

        // Scroll distância
        if (!modoRotacao)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            distanciaSegurar = Mathf.Clamp(
                distanciaSegurar - scroll * scrollSpeed,
                minDistancia,
                maxDistancia
            );
        }

        // Toggle modo rotação (H)
        if (Input.GetKeyDown(KeyCode.H))
        {
            modoRotacao = !modoRotacao;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Snap ao sair do modo rotação
            if (!modoRotacao)
                SnapRotacao();
        }

        // Rotação manual
        if (modoRotacao)
            RotacionarItem();

        // Arremessar
        if (!modoRotacao && Input.GetMouseButtonDown(1))
            Throw();
    }

    void FixedUpdate()
    {
        if (rb == null || modoRotacao) return;

        Vector3 origem = mainCamera.transform.position;
        Vector3 frente = mainCamera.transform.forward.normalized;

        Vector3 destino = origem + frente * distanciaSegurar;

        // Colisão frontal
        if (Physics.SphereCast(origem, raioColisao, frente, out RaycastHit hit, distanciaSegurar))
            destino = hit.point - frente * (raioColisao + 0.05f);

        // Proteção de chão
        if (Physics.Raycast(destino + Vector3.up, Vector3.down, out RaycastHit chao, alturaRayChao))
        {
            float yMin = chao.point.y + raioColisao;
            if (destino.y < yMin) destino.y = yMin;
        }

        rb.MovePosition(Vector3.Lerp(
            rb.position,
            destino,
            suavizacao * Time.fixedDeltaTime
        ));
    }

    void TryGrab()
    {
        if (!Physics.Raycast(
            mainCamera.transform.position,
            mainCamera.transform.forward,
            out RaycastHit hit,
            distanciaInteracao))
            return;

        if (!hit.collider.TryGetComponent(out Rigidbody hitRb)) return;

        rb = hitRb;
        item = hit.collider.gameObject;

        rb.useGravity = false;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        Physics.IgnoreCollision(item.GetComponent<Collider>(), controller, true);
    }

    void Drop()
    {
        rb.freezeRotation = false;
        rb.useGravity = true;

        Physics.IgnoreCollision(item.GetComponent<Collider>(), controller, false);

        rb = null;
        item = null;
        modoRotacao = false;
    }

    void Throw()
    {
        Rigidbody temp = rb;
        Drop();

        float forcaFinal = forcaArremessoBase / Mathf.Max(temp.mass, 0.1f);
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
}
