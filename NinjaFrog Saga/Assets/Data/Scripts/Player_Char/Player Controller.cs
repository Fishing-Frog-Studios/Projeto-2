using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerControls controls;
    private Vector2 moveInput;
    private bool isAttacking;

    private float speed;
    private float attackSpeed;
    private float attackDamage;

    [Header("Configurações de Ataque")]
    public Transform attackPoint; 
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        controls.Player.Ataque.performed += ctx => Attack();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    void Start()
    {
        // Tenta carregar os status a partir de um DataPlayer, se existir.
        if (DataPlayer.Instance != null)
        {
            speed = DataPlayer.Instance.moveSpeed;
            attackSpeed = DataPlayer.Instance.ataqueSpeed;
            attackDamage = DataPlayer.Instance.danoBase;
        }
        else
        {
            Debug.LogWarning("DataPlayer não encontrado. Usando valores padrão para PlayerController.");
            speed = 5f;
            attackSpeed = 1f;
            attackDamage = 25f;
        }
    }

    void Update()
    {
        Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0f);
        transform.Translate(movement * speed * Time.deltaTime);
    }
    
    private void Attack()
    {
        if (isAttacking) return;
        isAttacking = true;

        // Animação de ataque aqui
        
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            HealthSystem enemyHealth = enemyCollider.GetComponent<HealthSystem>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);

                // <<< LÓGICA CORRIGIDA E ROBUSTA >>>
                // Pede ao PlayerManager a referência do controlador da barra de vida.
                if (PlayerManager.Instance != null && PlayerManager.Instance.healthBarController != null)
                {
                    PlayerManager.Instance.healthBarController.SetTarget(enemyHealth);
                }
            }
        }
        
        Invoke(nameof(EndAttack), attackSpeed);
    }
    
    private void EndAttack() => isAttacking = false;
    
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
