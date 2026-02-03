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

    [Header("Motor e Performance")]
    public float forcaMotor = 2500f;
    public float forcaFreio = 6000f;
    public float velocidadeMaxima = 90f; // Km/h

    [Header("Direção Suave (Anti-Pulo)")]
    [Tooltip("Quanto menor, mais lento o volante gira. Tente entre 80 e 150.")]
    public float velocidadeGiroVolante = 100f;

    [Tooltip("Ângulo máximo quando está devagar")]
    public float anguloMaximoBaixa = 30f;

    [Tooltip("Ângulo máximo quando está correndo")]
    public float anguloMaximoAlta = 5f;

    [Header("Estabilidade")]
    public float forcaAntiRoll = 5000f; // Impede tombar
    [Tooltip("X=Lado, Y=Altura, Z=Frente. Mantenha Y negativo e Z positivo para estabilidade.")]
    public Vector3 centroDeMassa = new Vector3(0, -0.5f, 0.5f);

    // Variáveis Internas
    private float aceleracao;
    private float direcaoInput;
    private bool freando;
    private float anguloAtualRodas = 0f; // Para suavizar o movimento
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centroDeMassa;
    }

    void Update()
    {
        // Lê os comandos do jogador
        aceleracao = Input.GetAxis("Vertical");
        direcaoInput = Input.GetAxis("Horizontal");
        freando = Input.GetKey(KeyCode.Space);
    }

    void FixedUpdate()
    {
        // Pega velocidade em Km/h (Compatível com Unity 6 e anteriores)
        // Se der erro em versao antiga, troque 'linearVelocity' por 'velocity'
        float velKmH = rb.linearVelocity.magnitude * 3.6f;

        // =========================================================
        // 1. SISTEMA DE DIREÇÃO SUAVE (A Correção Principal)
        // =========================================================

        // Define qual o limite de ângulo baseado na velocidade atual
        float limiteAngulo = Mathf.Lerp(anguloMaximoBaixa, anguloMaximoAlta, velKmH / 60f);

        // Calcula onde queremos chegar
        float anguloAlvo = direcaoInput * limiteAngulo;

        // Move o ângulo atual em direção ao alvo PASSO A PASSO (não instantâneo)
        anguloAtualRodas = Mathf.MoveTowards(anguloAtualRodas, anguloAlvo, velocidadeGiroVolante * Time.deltaTime);

        // Aplica nas rodas da frente
        rodaFrenteEsq.steerAngle = anguloAtualRodas;
        rodaFrenteDir.steerAngle = anguloAtualRodas;

        // =========================================================
        // 2. MOTOR E FREIO
        // =========================================================

        float torque = aceleracao * forcaMotor;

        // Limita velocidade máxima cortando o motor
        if (velKmH > velocidadeMaxima && aceleracao > 0) torque = 0;

        // Aplica força (Tração traseira forte, dianteira fraca opcional)
        rodaTrasEsq.motorTorque = torque;
        rodaTrasDir.motorTorque = torque;
        rodaFrenteEsq.motorTorque = torque * 0.2f; // 20% de força na frente ajuda a curvar
        rodaFrenteDir.motorTorque = torque * 0.2f;

        // Freios
        float forcaFrear = freando ? forcaFreio : 0f;

        // Freio automático se soltar o acelerador (Drag)
        if (aceleracao == 0 && !freando && velKmH > 1) forcaFrear = 50f;
        // Freio de mão parado
        if (aceleracao == 0 && velKmH < 1) forcaFrear = 500f;

        rodaFrenteEsq.brakeTorque = forcaFrear;
        rodaFrenteDir.brakeTorque = forcaFrear;
        rodaTrasEsq.brakeTorque = forcaFrear;
        rodaTrasDir.brakeTorque = forcaFrear;

        // =========================================================
        // 3. BARRA ESTABILIZADORA (Anti-Tombo)
        // =========================================================
        AplicarAntiRoll(rodaFrenteEsq, rodaFrenteDir);
        AplicarAntiRoll(rodaTrasEsq, rodaTrasDir);

        AtualizarVisuais();
    }

    void AplicarAntiRoll(WheelCollider rodaL, WheelCollider rodaR)
    {
        WheelHit hit;
        float travelL = 1.0f;
        float travelR = 1.0f;

        bool groundedL = rodaL.GetGroundHit(out hit);
        if (groundedL)
            travelL = (-rodaL.transform.InverseTransformPoint(hit.point).y - rodaL.radius) / rodaL.suspensionDistance;

        bool groundedR = rodaR.GetGroundHit(out hit);
        if (groundedR)
            travelR = (-rodaR.transform.InverseTransformPoint(hit.point).y - rodaR.radius) / rodaR.suspensionDistance;

        float antiRollForce = (travelL - travelR) * forcaAntiRoll;

        if (groundedL) rb.AddForceAtPosition(rodaL.transform.up * -antiRollForce, rodaL.transform.position);
        if (groundedR) rb.AddForceAtPosition(rodaR.transform.up * antiRollForce, rodaR.transform.position);
    }

    void AtualizarVisuais()
    {
        AtualizarRodaUnica(rodaFrenteEsq, visualFrenteEsq);
        AtualizarRodaUnica(rodaFrenteDir, visualFrenteDir);
        AtualizarRodaUnica(rodaTrasEsq, visualTrasEsq);
        AtualizarRodaUnica(rodaTrasDir, visualTrasDir);
    }

    void AtualizarRodaUnica(WheelCollider col, Transform vis)
    {
        Vector3 p; Quaternion r;
        col.GetWorldPose(out p, out r);
        vis.position = p;
        vis.rotation = r;
    }
}