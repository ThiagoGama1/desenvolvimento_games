using UnityEngine;

/// <summary>
/// Controla a movimentação do Scrap-E: andar, correr, girar e pular.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float runSpeed = 8f;
    public float turnSpeed = 150f;
    public float jumpForce = 6f;
    public Animator animator;
    public float fallLimitY = -1f;      // abaixo desta altura, o jogador caiu do mapa e volta ao início

    private Rigidbody playerRb;
    private bool isOnGround = true;
    private Vector3 startPosition;

    /// <summary>
    /// Pega o Rigidbody do jogador quando o jogo começa.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// Guarda a referência ao Rigidbody, trava a rotação física (para o robô
    /// não tombar ao bater em algo) e memoriza a posição inicial, usada para
    /// trazer o jogador de volta se ele cair do mapa.
    /// </remarks>
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerRb.freezeRotation = true;
        startPosition = transform.position;
    }

    /// <summary>
    /// Lê o teclado a cada frame e move, gira e faz o jogador pular.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// 0. Se o jogador caiu abaixo de fallLimitY, volta para o ponto inicial.
    /// 1. W/S movem para frente e para trás com Translate (Shift corre),
    ///    desde que não haja uma parede no caminho (ver CanMove).
    /// 2. A/D giram o robô com Rotate.
    /// 3. Espaço aplica AddForce para cima, se estiver no chão.
    /// 4. Envia a velocidade e o estado do chão para o Animator.
    /// </remarks>
    void Update()
    {
        if (transform.position.y < fallLimitY)
        {
            Respawn();
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : speed;

        Vector3 moveStep = transform.forward * verticalInput * currentSpeed * Time.deltaTime;
        if (CanMove(moveStep))
        {
            transform.Translate(moveStep, Space.World);
        }
        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);

        if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnGround = false;

            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }

        if (animator != null)
        {
            float animSpeed = Mathf.Abs(verticalInput) * (isRunning ? 1f : 0.5f);
            animator.SetFloat("Speed", animSpeed);
            animator.SetBool("IsGrounded", isOnGround);
        }
    }

    /// <summary>
    /// Verifica se o jogador pode se mover sem entrar dentro de uma parede.
    /// </summary>
    /// <param name="moveStep">Deslocamento que o jogador quer fazer neste frame.</param>
    /// <returns>bool: true se pode mover; false se há uma parede fixa no caminho.</returns>
    /// <remarks>
    /// O Translate move o robô sem consultar a física. Sem esta verificação, ao
    /// andar contra uma parede ele entra nela um pouco a cada frame: treme e, no
    /// terreno, pode até atravessar a parede e cair por baixo do mapa.
    /// 1. Se não há deslocamento, não precisa mover.
    /// 2. Usa Rigidbody.SweepTest: "arrasta" o formato da cápsula na direção do
    ///    movimento e verifica se ela bateria em algo (com uma folga de 5 cm).
    /// 3. Se bater numa superfície íngreme (parede, normal com y menor que 0.5)
    ///    que não tem física, bloqueia o movimento.
    /// 4. Se bater em algo com Rigidbody (caixote, barril), deixa mover para empurrar.
    ///    Se bater numa rampa (normal apontando para cima), deixa mover para subir.
    /// </remarks>
    bool CanMove(Vector3 moveStep)
    {
        if (moveStep.sqrMagnitude < 0.000001f)
        {
            return false;
        }

        if (playerRb.SweepTest(moveStep.normalized, out RaycastHit hit, moveStep.magnitude + 0.05f, QueryTriggerInteraction.Ignore))
        {
            bool isWall = hit.normal.y < 0.5f;
            bool isPushable = hit.rigidbody != null && !hit.rigidbody.isKinematic;

            if (isWall && !isPushable)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Traz o jogador de volta ao ponto inicial depois de cair do mapa.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// Rede de segurança: recoloca o robô na posição memorizada no Start,
    /// zera a velocidade da queda e avisa no Console.
    /// </remarks>
    void Respawn()
    {
        transform.position = startPosition;
        playerRb.linearVelocity = Vector3.zero;
        isOnGround = false;
        Debug.Log("Jogador caiu do mapa e voltou ao ponto inicial.");
    }

    /// <summary>
    /// Remove qualquer velocidade horizontal que a física tenha dado ao jogador.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// O movimento horizontal é feito só pelo Translate. Quando o jogador encosta
    /// numa parede ou objeto, a física o empurra para fora e isso gera velocidade
    /// no Rigidbody. Como o collider não tem atrito, essa velocidade nunca pararia
    /// e o robô deslizaria sozinho. Aqui zeramos X e Z e mantemos Y, que é a
    /// gravidade e o pulo.
    /// </remarks>
    void FixedUpdate()
    {
        Vector3 velocity = playerRb.linearVelocity;
        playerRb.linearVelocity = new Vector3(0f, velocity.y, 0f);
    }

    /// <summary>
    /// Verifica, a cada passo de física, se o jogador está apoiado em alguma superfície.
    /// </summary>
    /// <param name="collision">Dados da colisão, incluindo todos os pontos de contato.</param>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// Roda enquanto o contato durar (não só quando ele começa), então funciona
    /// mesmo se o jogador nunca chegou a se separar do chão.
    /// 1. Ignora enquanto o jogador ainda está subindo, para não "recarregar" o pulo no ar.
    /// 2. Percorre os pontos de contato: se algum tem a normal apontando para cima
    ///    (a superfície está embaixo do jogador), considera que ele está no chão.
    ///    Contatos com paredes (normal para o lado) são ignorados.
    /// </remarks>
    void OnCollisionStay(Collision collision)
    {
        if (playerRb.linearVelocity.y > 0.1f)
        {
            return;
        }

        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isOnGround = true;
                return;
            }
        }
    }
}
