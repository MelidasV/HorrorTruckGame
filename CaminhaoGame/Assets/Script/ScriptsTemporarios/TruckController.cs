using UnityEngine;

public class TruckController : MonoBehaviour
{
    [Header("Rodas (Wheel Colliders)")]
    public WheelCollider rodaFrenteEsq;
    public WheelCollider rodaFrenteDir;
    public WheelCollider rodaTrasEsq;
    public WheelCollider rodaTrasDir;

    [Header("Visual (Meshes)")]
    public Transform visualFrenteEsq;
    public Transform visualFrenteDir;
    public Transform visualTrasEsq;
    public Transform visualTrasDir;

    [Header("Motor e Estabilidade")]
    public float forcaMotor = 2000f;
    public float forcaFreio = 4000f;

    // Configurações de Curva (Mais restritas para evitar 360)
    public float anguloMaximoBaixaVelocidade = 35f;
    public float anguloMaximoAltaVelocidade = 6f;   // Reduzi para 6 (era 10)
    public float velocidadeParaTravar = 40f;        // Endurece a direção mais cedo

    // ANTI-CAPOTAMENTO AGRESSIVO:
    // Baixamos o peso para -2.0 (muito abaixo do chão) para ancorar o caminhão
    public Vector3 centroDeMassa = new Vector3(0, -2.0f, 0);

    private float aceleracao;
    private float direcao;
    private bool freando;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centroDeMassa;
    }

    void Update()
    {
        aceleracao = Input.GetAxis("Vertical");
        direcao = Input.GetAxis("Horizontal");
        freando = Input.GetKey(KeyCode.Space);
    }

    void FixedUpdate()
    {
        // 1. CÁLCULO DA DIREÇÃO
        float velocidadeKmH = rb.linearVelocity.magnitude * 3.6f;

        // Interpolação da curva baseada na velocidade
        float anguloAtual = Mathf.Lerp(
            anguloMaximoBaixaVelocidade,
            anguloMaximoAltaVelocidade,
            velocidadeKmH / velocidadeParaTravar
        );

        float curva = direcao * anguloAtual;
        rodaFrenteEsq.steerAngle = curva;
        rodaFrenteDir.steerAngle = curva;

        // 2. MOTOR (4x4 com Controle de Tração Manual)
        float torque = aceleracao * forcaMotor;

        // Se estiver muito rápido, reduz um pouco a força para não destracionar
        if (velocidadeKmH > 60 && aceleracao > 0) torque *= 0.5f;

        // Força extra na Ré
        if (aceleracao < 0) torque *= 1.5f;

        rodaTrasEsq.motorTorque = torque;
        rodaTrasDir.motorTorque = torque;
        rodaFrenteEsq.motorTorque = torque;
        rodaFrenteDir.motorTorque = torque;

        // 3. FREIO
        float freio = freando ? forcaFreio : 0f;

        // Freio de mão automático se não estiver acelerando (para não descer ladeira)
        if (aceleracao == 0 && !freando && velocidadeKmH < 5) freio = 100f;

        rodaFrenteEsq.brakeTorque = freio;
        rodaFrenteDir.brakeTorque = freio;
        rodaTrasEsq.brakeTorque = freio;
        rodaTrasDir.brakeTorque = freio;

        AtualizarRodas();
    }

    void AtualizarRodas()
    {
        AtualizarUnicaRoda(rodaFrenteEsq, visualFrenteEsq);
        AtualizarUnicaRoda(rodaFrenteDir, visualFrenteDir);
        AtualizarUnicaRoda(rodaTrasEsq, visualTrasEsq);
        AtualizarUnicaRoda(rodaTrasDir, visualTrasDir);
    }

    void AtualizarUnicaRoda(WheelCollider col, Transform vis)
    {
        Vector3 p; Quaternion r;
        col.GetWorldPose(out p, out r);
        vis.position = p;
        vis.rotation = r;
    }
}