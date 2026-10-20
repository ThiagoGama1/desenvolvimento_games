using UnityEngine;

/// <summary>
/// Controla o Pulso Cinético do Scrap-E: ao apertar E ou o botão esquerdo do mouse,
/// arremessa os entulhos (tag HeavyDebris) que estão na frente do robô, usando
/// AddForce (empurrão) e AddTorque (giro).
/// </summary>
public class InteractionController : MonoBehaviour
{
    public float pulseForce = 12f;          // força do arremesso
    public float pulseTorque = 8f;          // força do giro aplicado no objeto
    public float pulseRadius = 2f;          // tamanho da área atingida
    public float pulseDistance = 1.5f;      // distância à frente do robô onde fica o centro da área
    public float cooldown = 0.5f;           // tempo mínimo entre dois pulsos (segundos)
    public GameObject impactParticlePrefab; // partícula de pedrinhas (prefab do Arthur, opcional)
    public AudioClip pulseSound;            // som do pulso (opcional)

    private float nextPulseTime = 0f;

    /// <summary>
    /// Verifica a cada frame se o jogador apertou o botão do pulso.
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// Se E ou o botão esquerdo do mouse foi apertado e o tempo de recarga
    /// já passou, dispara o pulso e marca quando o próximo será permitido.
    /// </remarks>
    void Update()
    {
        bool pressedPulse = Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0);

        if (pressedPulse && Time.time >= nextPulseTime)
        {
            FireKineticPulse();
            nextPulseTime = Time.time + cooldown;
        }
    }

    /// <summary>
    /// Dispara o Pulso Cinético e arremessa os entulhos próximos.
    /// </summary>
    /// <returns>int: quantidade de objetos atingidos pelo pulso.</returns>
    /// <remarks>
    /// 1. Calcula o centro da área do pulso, um pouco à frente do robô.
    /// 2. Usa Physics.OverlapSphere para listar todos os colliders dentro dessa esfera.
    /// 3. Para cada um com a tag HeavyDebris e com Rigidbody:
    ///    - calcula a direção do robô até o objeto, inclinada um pouco para cima;
    ///    - aplica AddForce nessa direção (ForceMode.Impulse = de uma vez só);
    ///    - aplica AddTorque num eixo perpendicular, para o objeto rolar e girar;
    ///    - cria a partícula de impacto no ponto mais próximo do objeto.
    /// 4. Toca o som do pulso, se houver.
    /// </remarks>
    public int FireKineticPulse()
    {
        Vector3 pulseCenter = transform.position + transform.forward * pulseDistance;
        Collider[] hits = Physics.OverlapSphere(pulseCenter, pulseRadius);
        int affectedCount = 0;

        foreach (Collider hit in hits)
        {
            Rigidbody targetRb = hit.attachedRigidbody;

            if (hit.CompareTag("HeavyDebris") && targetRb != null)
            {
                Vector3 direction = (hit.transform.position - transform.position).normalized;
                direction = (direction + Vector3.up * 0.3f).normalized;

                targetRb.AddForce(direction * pulseForce, ForceMode.Impulse);

                Vector3 torqueAxis = Vector3.Cross(Vector3.up, direction);
                targetRb.AddTorque(torqueAxis * pulseTorque, ForceMode.Impulse);

                SpawnImpactEffect(hit.ClosestPoint(transform.position));
                affectedCount++;
            }
        }

        if (pulseSound != null)
        {
            AudioSource.PlayClipAtPoint(pulseSound, transform.position);
        }

        return affectedCount;
    }

    /// <summary>
    /// Cria a partícula de impacto (pedrinhas) no ponto atingido.
    /// </summary>
    /// <param name="position">Ponto do mundo onde a partícula deve aparecer.</param>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// Se nenhum prefab de partícula foi definido no Inspector, não faz nada.
    /// Caso contrário, instancia o prefab na posição e o destrói depois de 2
    /// segundos, para não acumular objetos na cena.
    /// </remarks>
    void SpawnImpactEffect(Vector3 position)
    {
        if (impactParticlePrefab == null)
        {
            return;
        }

        GameObject effect = Instantiate(impactParticlePrefab, position, Quaternion.identity);
        Destroy(effect, 2f);
    }

    /// <summary>
    /// Desenha no editor a esfera da área do pulso (só aparece na aba Scene).
    /// </summary>
    /// <returns>void (sem retorno).</returns>
    /// <remarks>
    /// Ajuda a visualizar e ajustar pulseRadius e pulseDistance no Inspector.
    /// Não aparece no jogo, só com o Player selecionado na aba Scene.
    /// </remarks>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + transform.forward * pulseDistance, pulseRadius);
    }
}
