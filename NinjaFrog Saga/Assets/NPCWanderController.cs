using System.Collections;
using UnityEngine;

public class NPCWanderController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("A velocidade de movimento do NPC.")]
    public float moveSpeed = 1.5f;

    [Tooltip("A área onde o NPC pode caminhar. Arraste o objeto com o BoxCollider2D aqui.")]
    public BoxCollider2D walkableArea;

    [Header("Wandering Behavior")]
    [Tooltip("O tempo mínimo que o NPC espera antes de escolher um novo destino.")]
    public float minWaitTime = 2f;

    [Tooltip("O tempo máximo que o NPC espera antes de escolher um novo destino.")]
    public float maxWaitTime = 5f;

    private Vector2 targetPosition;
    private Rigidbody2D rb;
    private Bounds areaBounds;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (walkableArea == null)
        {
            Debug.LogError("A área de caminhada (WalkableArea) não foi definida para este NPC! O script será desativado.");
            this.enabled = false; 
            return;
        }

        // Armazena os limites da área de caminhada para uso posterior.
        areaBounds = walkableArea.bounds;

        // Inicia a rotina de comportamento do NPC.
        StartCoroutine(WanderRoutine());
    }

    // Coroutine é perfeita para ações baseadas em tempo (andar, esperar, repetir).
    private IEnumerator WanderRoutine()
    {
        // Loop infinito para manter o NPC sempre em movimento/espera.
        while (true)
        {
            // 1. Escolhe um novo destino.
            targetPosition = GetRandomPositionInArea();
            
            // 2. Caminha até o destino.
            // Enquanto a distância for maior que um pequeno valor, continue se movendo.
            while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
            {
                // Move o NPC na direção do alvo, de forma suave e independente da taxa de quadros.
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                // Pausa a coroutine até o próximo frame, para não travar o jogo.
                yield return null; 
            }

            // Garante que o NPC chegue exatamente na posição final.
            transform.position = targetPosition;
            Debug.Log("NPC chegou ao destino. Esperando...");

            // 3. Espera por um tempo aleatório no destino antes de começar de novo.
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);
            Debug.Log("NPC vai escolher um novo destino.");
        }
    }

    // Função auxiliar para pegar um ponto aleatório dentro dos limites da área definida.
    private Vector2 GetRandomPositionInArea()
    {
        float randomX = Random.Range(areaBounds.min.x, areaBounds.max.x);
        float randomY = Random.Range(areaBounds.min.y, areaBounds.max.y);
        return new Vector2(randomX, randomY);
    }
}
