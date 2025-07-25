using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    // <<< VARIÁVEIS ADICIONADAS >>>
    public float speed = 15f; // Velocidade do projétil.
    public float rotationSpeed = 200f; // Velocidade da rotação para seguir o alvo.
    private Transform target;
    private Rigidbody2D rb;
    private PlayerController playerController;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (DataPlayer.Instance != null && DataPlayer.Instance.playerTransform != null)
        {
            playerController = DataPlayer.Instance.playerTransform.GetComponent<PlayerController>();
        }
        Destroy(gameObject, 5f);
    }

    // Usamos FixedUpdate para manipulação de física (velocidade).
    void FixedUpdate()
    {
        // Só executa a lógica teleguiada se a skill estiver ativa.
        if (DataPlayer.Instance != null && DataPlayer.Instance.projectilesAreHoming)
        {
            // Procura o inimigo mais próximo a cada frame (ou a cada X segundos para otimizar).
            FindClosestEnemy();

            if (target != null)
            {
                // Move-se em direção ao alvo.
                Vector2 direction = (Vector2)target.position - rb.position;
                direction.Normalize();
                rb.linearVelocity = direction * speed;

                // (Opcional, mas legal) Rotaciona para "olhar" para o alvo.
                float rotateAmount = Vector3.Cross(direction, transform.up).z;
                rb.angularVelocity = -rotateAmount * rotationSpeed;
            }
        }
    }

    // <<< NOVA FUNÇÃO >>>
    // Encontra o inimigo mais próximo do projétil.
    void FindClosestEnemy()
    {
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    // A lógica de colisão permanece a mesma.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<HealthSystem>(out var healthSystem) && playerController != null)
            {
                float damageToDeal = playerController.CalculateFinalDamage();
                healthSystem.TakeDamage(damageToDeal);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
