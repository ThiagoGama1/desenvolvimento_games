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

    private Rigidbody playerRb;
    private bool isOnGround = true;

    /// <summary>
    /// Pega o Rigidbody do jogador quando o jogo começa.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// Guarda a referência ao Rigidbody e trava a rotação física,
    /// para o robô não tombar ao bater em algo.
    /// </remarks>
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerRb.freezeRotation = true;
    }

    /// <summary>
    /// Lê o teclado a cada frame e move, gira e faz o jogador pular.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// 1. W/S movem para frente e para trás com Translate (Shift corre).
    /// 2. A/D giram o robô com Rotate.
    /// 3. Espaço aplica AddForce para cima, se estiver no chão.
    /// 4. Envia a velocidade e o estado do chão para o Animator.
    /// </remarks>
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : speed;

        transform.Translate(Vector3.forward * Time.deltaTime * currentSpeed * verticalInput);
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
