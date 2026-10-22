using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controla o Menu Principal, a vitrine 3D do Scrap-E e os botões de navegação
/// de telas (Iniciar, Reiniciar, Menu Principal, Controles, Eco-Timer e Sair).
/// </summary>
public class MainMenuController : MonoBehaviour
{
    public static bool useEcoTimer = false;

    public string nomeCenaJogo = "Nivel1";
    public string nomeCenaMenu = "MenuPrincipal";
    public GameObject painelControles;
    public Text textoStatusEcoTimer;
    public Transform roboVitrine;
    public float velocidadeGiroVitrine = 25f;

    /// <summary>
    /// Inicializa o estado do menu quando a cena começa, destravando o tempo
    /// e atualizando o texto do modo Eco-Timer se houver referência configurada.
    /// </summary>
    /// <returns>void, sem retorno.</returns>
    /// <remarks>
    /// 1. Garante que Time.timeScale volte para 1f caso o jogador tenha vindo de uma tela pausada.
    /// 2. Se textoStatusEcoTimer estiver atribuído no Inspector, atualiza o rótulo com o estado atual de useEcoTimer.
    /// </remarks>
    void Start()
    {
        Time.timeScale = 1f;

        if (textoStatusEcoTimer != null)
        {
            textoStatusEcoTimer.text = useEcoTimer ? "ECO-TIMER: ATIVADO (3 MIN)" : "ECO-TIMER: DESATIVADO";
        }
    }

    /// <summary>
    /// Gira continuamente o robô da vitrine 3D no menu e verifica o atalho de reinício rápido (tecla R).
    /// </summary>
    /// <returns>void, sem retorno.</returns>
    /// <remarks>
    /// 1. Se roboVitrine estiver atribuído no Inspector, aplica Transform.Rotate no eixo Y (Vector3.up) a cada frame.
    /// 2. Se o jogador pressionar a tecla R fora do MenuPrincipal, recarrega a cena atual imediatamente.
    /// </remarks>
    void Update()
    {
        if (roboVitrine != null)
        {
            roboVitrine.Rotate(Vector3.up * velocidadeGiroVitrine * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.R) && SceneManager.GetActiveScene().name != nomeCenaMenu)
        {
            ReiniciarJogo();
        }
    }

    /// <summary>
    /// Inicia a partida carregando a cena principal do jogo (Nivel1).
    /// </summary>
    /// <returns>void, sem retorno.</returns>
    /// <remarks>
    /// 1. Garante que Time.timeScale esteja em 1f.
    /// 2. Chama SceneManager.LoadScene passando o nome configurado em nomeCenaJogo.
    /// </remarks>
    public void IniciarJogo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nomeCenaJogo);
    }

    /// <summary>
    /// Retorna para a cena do Menu Principal a partir de qualquer tela.
    /// </summary>
    /// <returns>void, sem retorno.</returns>
    /// <remarks>
    /// 1. Restaura Time.timeScale para 1f para não manter o jogo congelado.
    /// 2. Carrega a cena definida em nomeCenaMenu ("MenuPrincipal").
    /// </remarks>
    public void CarregarMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nomeCenaMenu);
    }

    /// <summary>
    /// Reinicia a cena atual do zero, restaurando o estado inicial da fase.
    /// </summary>
    /// <returns>void, sem retorno.</returns>
    /// <remarks>
    /// 1. Restaura Time.timeScale para 1f.
    /// 2. Obtém o nome da cena ativa com SceneManager.GetActiveScene().name e a recarrega.
    /// </remarks>
    public void ReiniciarJogo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Abre ou fecha o painel de controles e instruções no Menu Principal.
    /// </summary>
    /// <returns>void, sem retorno.</returns>
    /// <remarks>
    /// 1. Verifica se painelControles foi atribuído no Inspector.
    /// 2. Inverte o estado ativo do GameObject usando SetActive(!painelControles.activeSelf).
    /// </remarks>
    public void AlternarPainelControles()
    {
        if (painelControles != null)
        {
            painelControles.SetActive(!painelControles.activeSelf);
        }
    }

    /// <summary>
    /// Liga ou desliga a opção de desafio Eco-Timer (limite de 3 minutos) e atualiza o texto na interface.
    /// </summary>
    /// <returns>void, sem retorno.</returns>
    /// <remarks>
    /// 1. Inverte o valor booleano estático useEcoTimer.
    /// 2. Se textoStatusEcoTimer estiver atribuído, atualiza o texto indicando se o Eco-Timer está ativado ou desativado.
    /// </remarks>
    public void AlternarEcoTimer()
    {
        useEcoTimer = !useEcoTimer;

        if (textoStatusEcoTimer != null)
        {
            textoStatusEcoTimer.text = useEcoTimer ? "ECO-TIMER: ATIVADO (3 MIN)" : "ECO-TIMER: DESATIVADO";
        }
    }

    /// <summary>
    /// Encerra a execução do jogo (ou registra mensagem no Console quando testado no Editor).
    /// </summary>
    /// <returns>void, sem retorno.</returns>
    /// <remarks>
    /// 1. Exibe mensagem no Console com Debug.Log confirmando o acionamento do botão Sair.
    /// 2. Chama Application.Quit() para fechar o executável do jogo.
    /// </remarks>
    public void SairDoJogo()
    {
        Debug.Log("Saindo do Eco-Scrapper: Reclamation...");
        Application.Quit();
    }
}
