using UnityEngine;

/// <summary>
/// Controla a Integridade Estrutural (HP) do Scrap-E: dano do lodo tóxico,
/// dano de detritos em queda e o Game Over quando o HP chega a zero.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float slimeDamagePerSecond = 10f;  // dano contínuo enquanto estiver no lodo
    public float debrisDamage = 25f;          // dano de um detrito caindo em cima
    public float debrisMinSpeed = 5f;         // velocidade mínima do impacto para causar dano
    public GameObject deathEffectPrefab;      // faíscas/fumaça de curto-circuito (opcional)

    private bool isDead = false;
    private Collider bodyCollider;          // corpo do jogador (cápsula)
    private string lastDamageSource = "";  // última causa de dano registrada no Console
    private float lastDamageLogTime = -10f;

    /// <summary>
    /// Começa a fase com o HP cheio e guarda o collider do corpo do jogador.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// Copia maxHealth para currentHealth e pega o collider (a cápsula),
    /// usado para conferir se o jogador está mesmo encostando no lodo.
    /// </remarks>
    void Start()
    {
        currentHealth = maxHealth;
        bodyCollider = GetComponent<Collider>();
    }

    /// <summary>
    /// Aplica o dano contínuo enquanto o jogador está dentro de uma poça de lodo.
    /// </summary>
    /// <param name="other">Collider da área em que o jogador está.</param>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// OnTriggerStay roda a cada passo de física enquanto o jogador estiver
    /// dentro do trigger. Se a área tem a tag ToxicSlime, tira um pedaço do
    /// dano por segundo, proporcional ao tempo do passo (Time.fixedDeltaTime).
    /// Por segurança, antes confere se as caixas (bounds) do corpo do jogador e
    /// da poça realmente se cruzam. Se a Unity avisar por engano que o jogador
    /// ainda está na poça depois de sair, o dano é ignorado.
    /// </remarks>
    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("ToxicSlime"))
        {
            return;
        }

        if (bodyCollider != null && !bodyCollider.bounds.Intersects(other.bounds))
        {
            return;
        }

        TakeDamage(slimeDamagePerSecond * Time.fixedDeltaTime, "lodo tóxico (" + other.name + ")");
    }

    /// <summary>
    /// Detecta quando um detrito pesado bate no jogador com força.
    /// </summary>
    /// <param name="collision">Dados da colisão, incluindo a velocidade do impacto.</param>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// Só causa dano se o objeto tem a tag HeavyDebris e o PRÓPRIO detrito está
    /// se movendo rápido (velocidade do Rigidbody dele acima de debrisMinSpeed).
    /// Assim, encostar numa pedra parada, empurrar um caixote ou cair em cima
    /// de um entulho depois de um pulo não machuca; só um detrito caindo ou
    /// voando na direção do jogador machuca.
    /// </remarks>
    void OnCollisionEnter(Collision collision)
    {
        Rigidbody debrisRb = collision.rigidbody;

        if (collision.gameObject.CompareTag("HeavyDebris") && debrisRb != null && debrisRb.linearVelocity.magnitude > debrisMinSpeed)
        {
            TakeDamage(debrisDamage, "detrito (" + collision.gameObject.name + ")");
        }
    }

    /// <summary>
    /// Tira HP do jogador e verifica se ele foi destruído.
    /// </summary>
    /// <param name="amount">Quantidade de HP a remover.</param>
    /// <param name="source">Descrição do que causou o dano (aparece no Console).</param>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// 1. Ignora se o jogador já morreu.
    /// 2. Registra a causa no Console (no máximo uma vez por segundo para a
    ///    mesma causa, para não lotar o Console com o dano contínuo do lodo).
    /// 3. Subtrai o dano e limita o HP entre 0 e o máximo (Mathf.Clamp).
    /// 4. Se chegou a 0, chama Die().
    /// </remarks>
    public void TakeDamage(float amount, string source = "desconhecido")
    {
        if (isDead)
        {
            return;
        }

        if (source != lastDamageSource || Time.time - lastDamageLogTime > 1f)
        {
            Debug.Log("Dano recebido de " + source + ". HP: " + Mathf.CeilToInt(currentHealth - amount));
            lastDamageSource = source;
            lastDamageLogTime = Time.time;
        }

        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// Recupera HP do jogador (usado pelo cheat E-C-O).
    /// </summary>
    /// <param name="amount">Quantidade de HP a recuperar.</param>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>Soma e limita ao máximo. Não faz nada se o jogador já morreu.</remarks>
    public void Heal(float amount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
    }

    /// <summary>
    /// Destrói o Scrap-E: mostra o curto-circuito e avisa o GameManager.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// Marca o jogador como morto, cria o efeito de faíscas (se houver) na
    /// posição dele e chama GameManager.Instance.GameOver().
    /// </remarks>
    void Die()
    {
        isDead = true;

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }
}
