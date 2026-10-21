using System.Collections;
using UnityEngine;

/// <summary>
/// Trapaça de desenvolvedor (GDD 3.4): digitar E, C, O em sequência recupera
/// toda a vida do Scrap-E e dobra a velocidade por alguns segundos.
/// Serve para facilitar a demonstração do jogo em bancada.
/// </summary>
public class CheatCode : MonoBehaviour
{
    public float maxTimeBetweenKeys = 1.5f;  // tempo máximo entre uma tecla e a próxima
    public float boostMultiplier = 2f;       // quanto a velocidade é multiplicada
    public float boostDuration = 5f;         // quanto tempo o impulso dura (segundos)
    public AudioClip cheatSound;             // som de confirmação (opcional)

    private KeyCode[] sequence = { KeyCode.E, KeyCode.C, KeyCode.O };
    private int currentIndex = 0;            // quantas teclas da sequência já foram acertadas
    private float lastKeyTime = 0f;

    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    private float originalSpeed;
    private float originalRunSpeed;
    private bool isBoosting = false;
    private Coroutine boostRoutine;

    /// <summary>
    /// Pega os scripts de vida e de movimento do próprio jogador.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>Os dois precisam estar no mesmo objeto (o Player).</remarks>
    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    /// <summary>
    /// Acompanha as teclas digitadas e verifica se formam a sequência E-C-O.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// 1. Se passou tempo demais desde a última tecla certa, recomeça a sequência.
    /// 2. Se a próxima tecla esperada foi apertada, avança uma posição.
    ///    Ao completar as três, ativa a trapaça e recomeça.
    /// 3. Se apertou uma das letras do código fora de ordem, recomeça
    ///    (e aceita essa tecla como início, caso seja o E).
    /// </remarks>
    void Update()
    {
        if (currentIndex > 0 && Time.time - lastKeyTime > maxTimeBetweenKeys)
        {
            currentIndex = 0;
        }

        if (Input.GetKeyDown(sequence[currentIndex]))
        {
            currentIndex++;
            lastKeyTime = Time.time;

            if (currentIndex >= sequence.Length)
            {
                currentIndex = 0;
                ActivateCheat();
            }
        }
        else if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.O))
        {
            currentIndex = Input.GetKeyDown(KeyCode.E) ? 1 : 0;
            lastKeyTime = Time.time;
        }
    }

    /// <summary>
    /// Aplica os efeitos da trapaça: vida cheia e impulso de velocidade.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// 1. Recupera toda a vida com PlayerHealth.Heal.
    /// 2. Inicia (ou reinicia) a corrotina do impulso de velocidade.
    /// 3. Toca o som de confirmação e registra no Console.
    /// </remarks>
    void ActivateCheat()
    {
        if (playerHealth != null)
        {
            playerHealth.Heal(playerHealth.maxHealth);
        }

        if (playerMovement != null)
        {
            if (boostRoutine != null)
            {
                StopCoroutine(boostRoutine);
            }

            boostRoutine = StartCoroutine(SpeedBoost());
        }

        if (cheatSound != null)
        {
            AudioSource.PlayClipAtPoint(cheatSound, transform.position);
        }

        Debug.Log("Cheat E-C-O ativado: vida cheia e velocidade x" + boostMultiplier + " por " + boostDuration + "s.");
    }

    /// <summary>
    /// Dobra a velocidade do jogador por um tempo e depois volta ao normal.
    /// </summary>
    /// <returns>IEnumerator: corrotina que a Unity executa ao longo de vários frames.</returns>
    /// <remarks>
    /// 1. Na primeira ativação, guarda as velocidades originais (se o cheat for
    ///    usado de novo durante o impulso, não multiplica em cima do já multiplicado).
    /// 2. Multiplica speed e runSpeed do PlayerMovement por boostMultiplier.
    /// 3. Espera boostDuration segundos (WaitForSeconds).
    /// 4. Restaura as velocidades originais.
    /// </remarks>
    IEnumerator SpeedBoost()
    {
        if (!isBoosting)
        {
            originalSpeed = playerMovement.speed;
            originalRunSpeed = playerMovement.runSpeed;
            isBoosting = true;
        }

        playerMovement.speed = originalSpeed * boostMultiplier;
        playerMovement.runSpeed = originalRunSpeed * boostMultiplier;

        yield return new WaitForSeconds(boostDuration);

        playerMovement.speed = originalSpeed;
        playerMovement.runSpeed = originalRunSpeed;
        isBoosting = false;
        boostRoutine = null;
    }
}
