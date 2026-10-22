using UnityEngine;

/// <summary>
/// Guarda o que o Scrap-E coletou: Bio-Filtros (meta da fase) e sucatas (pontos).
/// A coleta acontece ao encostar nos objetos, identificados pela tag.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    public int filtersCollected = 0;   // Bio-Filtros que o jogador está carregando
    public int filtersRequired = 5;    // quantos são necessários para ativar a torre
    public int scrapCollected = 0;     // sucatas coletadas (cada uma vale pontos)
    public AudioClip collectSound;     // som de coleta (opcional)

    /// <summary>
    /// Detecta quando o jogador encosta num objeto coletável.
    /// </summary>
    /// <param name="other">Collider do objeto que entrou na área do jogador.</param>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// Os coletáveis têm collider marcado como Is Trigger, então o jogador passa
    /// através deles. Pela tag, o script decide o que fazer:
    /// - EcoItem: soma um Bio-Filtro e remove o objeto da cena;
    /// - ScrapCoin: soma uma sucata e remove o objeto da cena.
    /// </remarks>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EcoItem"))
        {
            filtersCollected++;
            Collect(other.gameObject);
            Debug.Log("Bio-Filtros: " + filtersCollected + " / " + filtersRequired);
        }
        else if (other.CompareTag("ScrapCoin"))
        {
            scrapCollected++;
            Collect(other.gameObject);
            Debug.Log("Sucata: " + scrapCollected);
        }
    }

    /// <summary>
    /// Toca o som de coleta e remove o objeto coletado da cena.
    /// </summary>
    /// <param name="item">Objeto que foi coletado.</param>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// O som é tocado na posição do item com PlayClipAtPoint, que continua
    /// tocando mesmo depois que o item é destruído.
    /// </remarks>
    void Collect(GameObject item)
    {
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, item.transform.position);
        }

        Destroy(item);
    }

    /// <summary>
    /// Informa se o jogador já tem todos os Bio-Filtros necessários.
    /// </summary>
    /// <returns>bool: true se filtersCollected chegou em filtersRequired.</returns>
    /// <remarks>
    /// Usado pela Torre Purificadora para decidir se pode ser ativada.
    /// </remarks>
    public bool HasAllFilters()
    {
        return filtersCollected >= filtersRequired;
    }
}
