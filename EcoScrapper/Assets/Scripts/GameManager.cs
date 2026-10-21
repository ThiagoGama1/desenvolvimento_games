using UnityEngine;

/// <summary>
/// Controla o estado geral da fase: pausa, vitória, derrota, Eco-Timer e pontuação.
/// Existe um único GameManager por cena, acessível por GameManager.Instance.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;   // acesso fácil a partir de outros scripts

    public PlayerInventory playerInventory;  // inventário do jogador (para a pontuação)
    public MonoBehaviour[] playerControls;   // scripts desligados quando o jogo para (PlayerMovement, InteractionController)
    public UIManager uiManager;              // interface (HUD e painéis)

    public float ecoTimerSeconds = 180f;     // 3 minutos, como no GDD
    public float victoryScreenDelay = 2f;    // tempo para ver a explosão de partículas antes da tela de vitória

    public int scorePerScrap = 100;
    public int scoreForCompletion = 1000;
    public int scorePerSecondLeft = 10;

    private bool isPaused = false;
    private bool isGameEnded = false;
    private bool useEcoTimer = false;
    private float timeLeft;
    private float elapsedTime = 0f;

    /// <summary>
    /// Registra este objeto como o GameManager da cena.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// Guarda a referência em Instance para que a torre e o HP do jogador
    /// consigam chamar Victory() e GameOver() sem precisar procurar o objeto.
    /// </remarks>
    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Prepara a fase: libera o tempo e liga o Eco-Timer se ele foi ativado no menu.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// 1. Garante que o tempo do jogo está correndo (Time.timeScale = 1).
    /// 2. Lê a escolha feita no Menu Principal (MainMenuController.useEcoTimer).
    /// 3. Se o Eco-Timer estiver ligado, começa a contagem e mostra o cronômetro.
    /// </remarks>
    void Start()
    {
        Time.timeScale = 1f;
        useEcoTimer = MainMenuController.useEcoTimer;
        timeLeft = ecoTimerSeconds;

        if (uiManager != null)
        {
            uiManager.ShowTimer(useEcoTimer);
        }
    }

    /// <summary>
    /// A cada frame: verifica a tecla de pausa e atualiza os cronômetros.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// 1. Se a fase já terminou, não faz nada.
    /// 2. Esc liga e desliga a pausa.
    /// 3. Se não está pausado, soma o tempo jogado e, no modo Eco-Timer,
    ///    desconta o tempo restante. Se ele zerar, é Game Over.
    /// </remarks>
    void Update()
    {
        if (isGameEnded)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (isPaused)
        {
            return;
        }

        elapsedTime += Time.deltaTime;

        if (useEcoTimer)
        {
            timeLeft -= Time.deltaTime;

            if (uiManager != null)
            {
                uiManager.UpdateTimer(timeLeft);
            }

            if (timeLeft <= 0f)
            {
                timeLeft = 0f;
                GameOver();
            }
        }
    }

    /// <summary>
    /// Pausa ou despausa o jogo.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// Inverte o estado de pausa, congela ou libera o tempo (Time.timeScale),
    /// liga ou desliga os controles do jogador e mostra ou esconde o painel de pausa.
    /// Também é chamado pelo botão Continuar.
    /// </remarks>
    public void TogglePause()
    {
        if (isGameEnded)
        {
            return;
        }

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        SetPlayerControls(!isPaused);

        if (uiManager != null)
        {
            uiManager.ShowPause(isPaused);
        }
    }

    /// <summary>
    /// Encerra a fase com vitória. Chamado pela Torre Purificadora.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// 1. Ignora se a fase já terminou (evita vitória dupla).
    /// 2. Desliga os controles do jogador.
    /// 3. Espera alguns segundos (victoryScreenDelay) para o jogador ver a
    ///    transformação do cenário e então mostra a tela de vitória.
    /// </remarks>
    public void Victory()
    {
        if (isGameEnded)
        {
            return;
        }

        isGameEnded = true;
        SetPlayerControls(false);
        Invoke(nameof(ShowVictoryScreen), victoryScreenDelay);
    }

    /// <summary>
    /// Mostra a tela de vitória com a pontuação final e congela o jogo.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// Calcula a pontuação, envia para o UIManager junto com o tempo jogado
    /// e para o tempo do jogo (Time.timeScale = 0).
    /// </remarks>
    void ShowVictoryScreen()
    {
        if (uiManager != null)
        {
            uiManager.ShowVictory(CalculateScore(), elapsedTime);
        }

        Time.timeScale = 0f;
    }

    /// <summary>
    /// Encerra a fase com derrota. Chamado pelo HP do jogador ou pelo Eco-Timer.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// Ignora se a fase já terminou. Caso contrário, desliga os controles,
    /// mostra a tela de Game Over e congela o jogo.
    /// </remarks>
    public void GameOver()
    {
        if (isGameEnded)
        {
            return;
        }

        isGameEnded = true;
        SetPlayerControls(false);

        if (uiManager != null)
        {
            uiManager.ShowGameOver();
        }

        Time.timeScale = 0f;
    }

    /// <summary>
    /// Calcula a pontuação final da fase.
    /// </summary>
    /// <returns>int: pontos da sucata + bônus de conclusão + bônus de tempo (só no Eco-Timer).</returns>
    /// <remarks>
    /// Cada sucata vale scorePerScrap (100). Concluir vale scoreForCompletion (1000).
    /// No modo Eco-Timer, cada segundo restante vale scorePerSecondLeft (10).
    /// </remarks>
    public int CalculateScore()
    {
        int score = scoreForCompletion;

        if (playerInventory != null)
        {
            score += playerInventory.scrapCollected * scorePerScrap;
        }

        if (useEcoTimer)
        {
            score += Mathf.CeilToInt(timeLeft) * scorePerSecondLeft;
        }

        return score;
    }

    /// <summary>
    /// Liga ou desliga os scripts de controle do jogador.
    /// </summary>
    /// <param name="enabled">true para o jogador poder se mover; false para travar.</param>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// Percorre o vetor playerControls (normalmente PlayerMovement e
    /// InteractionController) e muda o enabled de cada um. Um script
    /// desligado não roda o Update, então o jogador não anda nem dispara o pulso.
    /// </remarks>
    void SetPlayerControls(bool enabled)
    {
        foreach (MonoBehaviour control in playerControls)
        {
            if (control != null)
            {
                control.enabled = enabled;
            }
        }
    }
}
