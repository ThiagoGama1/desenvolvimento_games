using UnityEngine;

/// <summary>
/// Faz o objeto girar continuamente no próprio eixo vertical.
/// Usado nos Bio-Filtros e nas sucatas para chamar a atenção do jogador.
/// </summary>
public class Rotator : MonoBehaviour
{
    public float rotationSpeed = 90f; // graus por segundo

    /// <summary>
    /// Gira o objeto um pouco a cada frame.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// Usa Transform.Rotate em torno do eixo Y (Vector3.up). Multiplicar por
    /// Time.deltaTime faz a velocidade ser a mesma em qualquer computador.
    /// </remarks>
    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
