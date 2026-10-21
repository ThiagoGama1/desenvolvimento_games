using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Atualiza o HUD do Rafael (HP, filtros, sucata, cronômetro) e mostra os painéis
/// de pausa, vitória e game over. Fica no objeto HUD_Canvas e encontra sozinho
/// os elementos pelos nomes combinados no contexto do projeto.
/// </summary>
public class UIManager : MonoBehaviour
{
    public PlayerInventory playerInventory;  // de onde vêm os números de filtros e sucata
    public PlayerHealth playerHealth;        // de onde vem o HP

    private Slider barraHP;
    private Text textoFiltros;
    private Text textoSucata;
    private Text textoTimer;
    private Text textoPontuacao;
    private GameObject painelPausa;
    private GameObject painelVitoria;
    private GameObject painelGameOver;

    /// <summary>
    /// Encontra os elementos do HUD pelos nomes e liga o botão Continuar.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// 1. Usa transform.Find com os nomes do contrato (BarraHP, TextoFiltros, ...).
    ///    Assim não é preciso arrastar cada elemento no Inspector.
    /// 2. Liga o BotaoContinuar ao TogglePause do GameManager.
    /// 3. Garante que os painéis começam escondidos.
    /// </remarks>
    void Awake()
    {
        barraHP = FindComponent<Slider>("BarraHP");
        textoFiltros = FindComponent<Text>("TextoFiltros");
        textoSucata = FindComponent<Text>("TextoSucata");
        textoTimer = FindComponent<Text>("TextoTimer");
        textoPontuacao = FindComponent<Text>("PainelVitoria/TextoPontuacao");

        painelPausa = FindObject("PainelPausa");
        painelVitoria = FindObject("PainelVitoria");
        painelGameOver = FindObject("PainelGameOver");

        Button botaoContinuar = FindComponent<Button>("PainelPausa/BotaoContinuar");
        if (botaoContinuar != null)
        {
            botaoContinuar.onClick.AddListener(OnContinueClicked);
        }

        SetActive(painelPausa, false);
        SetActive(painelVitoria, false);
        SetActive(painelGameOver, false);
    }

    /// <summary>
    /// Atualiza os números do HUD a cada frame.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// Lê os valores direto do inventário e do HP do jogador e escreve nos
    /// textos e na barra. Se alguma referência estiver vazia, pula aquela parte.
    /// </remarks>
    void Update()
    {
        if (playerInventory != null)
        {
            if (textoFiltros != null)
            {
                textoFiltros.text = "Bio-Filtros: " + playerInventory.filtersCollected + " / " + playerInventory.filtersRequired;
            }

            if (textoSucata != null)
            {
                textoSucata.text = "Sucata: " + playerInventory.scrapCollected;
            }
        }

        if (playerHealth != null && barraHP != null)
        {
            barraHP.maxValue = playerHealth.maxHealth;
            barraHP.value = playerHealth.currentHealth;
        }
    }

    /// <summary>
    /// Mostra ou esconde o cronômetro do Eco-Timer.
    /// </summary>
    /// <param name="show">true para mostrar; false para esconder.</param>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>Chamado pelo GameManager no início da fase.</remarks>
    public void ShowTimer(bool show)
    {
        if (textoTimer != null)
        {
            textoTimer.gameObject.SetActive(show);
        }
    }

    /// <summary>
    /// Escreve o tempo restante do Eco-Timer no formato minutos:segundos.
    /// </summary>
    /// <param name="secondsLeft">Segundos que ainda faltam.</param>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>Arredonda para cima, para mostrar 0:01 até o último instante.</remarks>
    public void UpdateTimer(float secondsLeft)
    {
        if (textoTimer != null)
        {
            textoTimer.text = FormatTime(Mathf.Max(0f, secondsLeft));
        }
    }

    /// <summary>
    /// Mostra ou esconde o painel de pausa.
    /// </summary>
    /// <param name="show">true para mostrar; false para esconder.</param>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>Chamado pelo GameManager ao apertar Esc ou Continuar.</remarks>
    public void ShowPause(bool show)
    {
        SetActive(painelPausa, show);
    }

    /// <summary>
    /// Mostra o painel de vitória com a pontuação e o tempo de jogo.
    /// </summary>
    /// <param name="score">Pontuação final calculada pelo GameManager.</param>
    /// <param name="elapsedSeconds">Tempo total jogado, em segundos.</param>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>Escreve no TextoPontuacao e ativa o PainelVitoria.</remarks>
    public void ShowVictory(int score, float elapsedSeconds)
    {
        if (textoPontuacao != null)
        {
            textoPontuacao.text = "Pontuação: " + score + "\nTempo: " + FormatTime(elapsedSeconds);
        }

        SetActive(painelVitoria, true);
    }

    /// <summary>
    /// Mostra o painel de Game Over.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>Os botões Reiniciar e Menu já foram ligados pelo Rafael ao MainMenuController.</remarks>
    public void ShowGameOver()
    {
        SetActive(painelGameOver, true);
    }

    /// <summary>
    /// Reação ao clique no botão Continuar do painel de pausa.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>Pede ao GameManager para despausar o jogo.</remarks>
    void OnContinueClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TogglePause();
        }
    }

    /// <summary>
    /// Converte segundos em texto no formato m:ss.
    /// </summary>
    /// <param name="seconds">Quantidade de segundos.</param>
    /// <returns>string: o tempo formatado, por exemplo "2:05".</returns>
    /// <remarks>Arredonda para cima e separa minutos e segundos com divisão inteira e resto.</remarks>
    string FormatTime(float seconds)
    {
        int total = Mathf.CeilToInt(seconds);
        int minutes = total / 60;
        int secs = total % 60;
        return minutes + ":" + secs.ToString("00");
    }

    /// <summary>
    /// Procura um filho do HUD pelo caminho e devolve um componente dele.
    /// </summary>
    /// <param name="path">Caminho do objeto dentro do HUD_Canvas (ex.: "PainelPausa/BotaoContinuar").</param>
    /// <returns>O componente encontrado, ou null se o objeto não existir.</returns>
    /// <remarks>Se não encontrar, avisa no Console qual nome está faltando.</remarks>
    T FindComponent<T>(string path) where T : Component
    {
        Transform child = transform.Find(path);
        if (child == null)
        {
            Debug.LogWarning("UIManager: não encontrei '" + path + "' dentro do " + gameObject.name);
            return null;
        }

        return child.GetComponent<T>();
    }

    /// <summary>
    /// Procura um filho do HUD pelo caminho e devolve o GameObject.
    /// </summary>
    /// <param name="path">Caminho do objeto dentro do HUD_Canvas.</param>
    /// <returns>O GameObject encontrado, ou null se não existir.</returns>
    /// <remarks>Se não encontrar, avisa no Console qual nome está faltando.</remarks>
    GameObject FindObject(string path)
    {
        Transform child = transform.Find(path);
        if (child == null)
        {
            Debug.LogWarning("UIManager: não encontrei '" + path + "' dentro do " + gameObject.name);
            return null;
        }

        return child.gameObject;
    }

    /// <summary>
    /// Ativa ou desativa um objeto, se ele existir.
    /// </summary>
    /// <param name="target">Objeto a ser ativado ou desativado.</param>
    /// <param name="active">true para ativar; false para desativar.</param>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>Evita erro caso algum painel não tenha sido encontrado.</remarks>
    void SetActive(GameObject target, bool active)
    {
        if (target != null)
        {
            target.SetActive(active);
        }
    }
}
