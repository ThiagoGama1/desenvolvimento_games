using UnityEngine;

/// <summary>
/// Torre Purificadora: quando o jogador está no console com os 5 Bio-Filtros
/// e aperta E, a torre é ativada, o cenário é purificado e a fase é vencida.
/// Deve ficar num objeto com collider marcado como Is Trigger (a área do console).
/// </summary>
public class PurifierTower : MonoBehaviour
{
    public GameObject purificationEffectPrefab; // explosão de partículas verdes (prefab do Arthur, opcional)
    public Transform effectPoint;               // onde a explosão aparece (se vazio, usa a própria torre)
    public AudioClip activationSound;           // som da água / ativação (opcional)
    public GameObject[] blockingDebris;         // entulhos que precisam sair do canal (opcional)
    public Collider channelArea;                // área do canal que precisa ficar livre (opcional)
    public bool disableFogOnVictory = true;     // a névoa some quando a torre liga

    private bool playerInRange = false;
    private bool isPurified = false;
    private PlayerInventory playerInventory;

    /// <summary>
    /// Detecta quando o jogador entra na área do console.
    /// </summary>
    /// <param name="other">Collider que entrou na área.</param>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// Se for o jogador (tag Player), guarda o inventário dele e marca que
    /// está no alcance. Mostra no Console o que falta para ativar.
    /// </remarks>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerInventory = other.GetComponent<PlayerInventory>();
            Debug.Log("Torre: aperte E para ativar. " + GetMissingRequirement());
        }
    }

    /// <summary>
    /// Detecta quando o jogador sai da área do console.
    /// </summary>
    /// <param name="other">Collider que saiu da área.</param>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>Se for o jogador, marca que ele não está mais no alcance.</remarks>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    /// <summary>
    /// Verifica a cada frame se o jogador apertou E perto do console.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// Só tenta ativar se o jogador estiver no alcance e a torre ainda não foi ativada.
    /// Com o jogo pausado (Time.timeScale = 0) a tecla é ignorada, para a vitória
    /// não acontecer por trás do painel de pausa.
    /// </remarks>
    void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        if (playerInRange && !isPurified && Input.GetKeyDown(KeyCode.E))
        {
            TryActivate();
        }
    }

    /// <summary>
    /// Tenta ativar a torre, conferindo se todas as condições foram cumpridas.
    /// </summary>
    /// <returns>bool: true se a torre foi ativada; false se ainda falta algo.</returns>
    /// <remarks>
    /// Usa GetMissingRequirement(). Se não falta nada, chama Activate().
    /// Se falta algo, mostra no Console o que é.
    /// </remarks>
    public bool TryActivate()
    {
        string missing = GetMissingRequirement();

        if (missing != "")
        {
            Debug.Log("Torre: ainda não. " + missing);
            return false;
        }

        Activate();
        return true;
    }

    /// <summary>
    /// Verifica o que ainda impede a ativação da torre.
    /// </summary>
    /// <returns>string: descrição do que falta, ou "" se está tudo pronto.</returns>
    /// <remarks>
    /// 1. O jogador precisa ter todos os Bio-Filtros (PlayerInventory.HasAllFilters).
    /// 2. Se channelArea e blockingDebris foram definidos, cada entulho precisa
    ///    estar fora da área do canal (bounds.Contains verifica se um ponto está
    ///    dentro da caixa do collider). Se não foram definidos, essa regra é ignorada.
    /// </remarks>
    string GetMissingRequirement()
    {
        if (playerInventory == null || !playerInventory.HasAllFilters())
        {
            int have = playerInventory != null ? playerInventory.filtersCollected : 0;
            int need = playerInventory != null ? playerInventory.filtersRequired : 5;
            return "Faltam Bio-Filtros (" + have + " / " + need + ").";
        }

        if (channelArea != null)
        {
            int stillBlocking = 0;
            foreach (GameObject debris in blockingDebris)
            {
                if (debris != null && channelArea.bounds.Contains(debris.transform.position))
                {
                    stillBlocking++;
                }
            }

            if (stillBlocking > 0)
            {
                return "Ainda há " + stillBlocking + " entulho(s) bloqueando o canal.";
            }
        }

        return "";
    }

    /// <summary>
    /// Ativa a torre: efeitos de purificação e vitória.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// 1. Marca a torre como purificada (não ativa duas vezes).
    /// 2. Cria a explosão de partículas no effectPoint (ou na torre).
    /// 3. Toca o som de ativação.
    /// 4. Desliga a névoa da cena (RenderSettings.fog), mostrando o céu limpo.
    /// 5. Avisa o GameManager da vitória.
    /// </remarks>
    void Activate()
    {
        isPurified = true;
        Vector3 position = effectPoint != null ? effectPoint.position : transform.position;

        if (purificationEffectPrefab != null)
        {
            Instantiate(purificationEffectPrefab, position, Quaternion.identity);
        }

        if (activationSound != null)
        {
            AudioSource.PlayClipAtPoint(activationSound, position);
        }

        if (disableFogOnVictory)
        {
            RenderSettings.fog = false;
        }

        Debug.Log("Torre Purificadora ativada! O Vale Estéril foi purificado.");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.Victory();
        }
    }
}
