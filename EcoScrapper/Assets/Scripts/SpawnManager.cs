using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cria os Bio-Filtros e as sucatas em posições sorteadas no início da fase.
/// Usa vetores de prefabs e de pontos de spawn, sorteando com Random.Range
/// sem repetir o mesmo ponto, nem entre filtros e sucatas.
/// </summary>
public class SpawnManager : MonoBehaviour
{
    public GameObject[] bioFilterPrefabs;   // vetor de prefabs dos Bio-Filtros
    public Transform[] filterSpawnPoints;   // pontos onde os filtros podem aparecer
    public int filtersToSpawn = 5;          // quantos filtros criar

    public GameObject[] scrapPrefabs;       // vetor de prefabs das sucatas (opcional)
    public Transform[] scrapSpawnPoints;    // pontos onde as sucatas podem aparecer
    public int scrapToSpawn = 8;            // quantas sucatas criar

    private HashSet<Transform> usedPoints = new HashSet<Transform>(); // pontos já ocupados nesta fase

    /// <summary>
    /// Cria os filtros e as sucatas quando a fase começa.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// Chama SpawnObjects duas vezes: primeiro para os Bio-Filtros, depois para as
    /// sucatas. Os pontos usados pelos filtros ficam marcados como ocupados, então
    /// as sucatas só são sorteadas entre os pontos que sobraram. Como o sorteio
    /// acontece a cada vez que a cena carrega, reiniciar a fase muda as posições.
    /// </remarks>
    void Start()
    {
        int filters = SpawnObjects(bioFilterPrefabs, filterSpawnPoints, filtersToSpawn);
        int scraps = SpawnObjects(scrapPrefabs, scrapSpawnPoints, scrapToSpawn);
        Debug.Log("SpawnManager: " + filters + " filtros e " + scraps + " sucatas criados.");
    }

    /// <summary>
    /// Sorteia pontos sem repetir e cria um prefab sorteado em cada um.
    /// </summary>
    /// <param name="prefabs">Vetor de prefabs que podem ser criados.</param>
    /// <param name="points">Vetor de pontos de spawn disponíveis.</param>
    /// <param name="amount">Quantidade de objetos a criar.</param>
    /// <returns>int: quantos objetos foram realmente criados.</returns>
    /// <remarks>
    /// 1. Se não houver prefabs ou pontos, não cria nada e retorna 0.
    /// 2. Monta a lista de pontos livres: todos os pontos do vetor, menos os que
    ///    já foram ocupados antes (por exemplo, pelos filtros).
    /// 3. Repete até criar a quantidade pedida (ou acabarem os pontos):
    ///    - sorteia um ponto da lista com Random.Range;
    ///    - sorteia qual prefab do vetor será criado;
    ///    - cria o objeto com Instantiate na posição do ponto;
    ///    - remove o ponto da lista e marca como ocupado, para dois objetos
    ///      não nascerem no mesmo lugar.
    /// </remarks>
    int SpawnObjects(GameObject[] prefabs, Transform[] points, int amount)
    {
        if (prefabs == null || prefabs.Length == 0 || points == null || points.Length == 0)
        {
            return 0;
        }

        List<Transform> availablePoints = new List<Transform>();
        foreach (Transform point in points)
        {
            if (point != null && !usedPoints.Contains(point))
            {
                availablePoints.Add(point);
            }
        }

        int created = 0;

        while (created < amount && availablePoints.Count > 0)
        {
            int pointIndex = Random.Range(0, availablePoints.Count);
            int prefabIndex = Random.Range(0, prefabs.Length);

            Transform chosenPoint = availablePoints[pointIndex];
            Instantiate(prefabs[prefabIndex], chosenPoint.position, prefabs[prefabIndex].transform.rotation);

            availablePoints.RemoveAt(pointIndex);
            usedPoints.Add(chosenPoint);
            created++;
        }

        if (created < amount)
        {
            Debug.LogWarning("SpawnManager: pontos insuficientes. Pedidos " + amount + ", criados " + created + ".");
        }

        return created;
    }
}
