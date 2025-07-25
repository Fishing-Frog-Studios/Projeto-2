using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))] // Garante que sempre haja um Canvas
public class EnemyHealthBarController : MonoBehaviour
{
    [Tooltip("A referência para o componente Slider que representa a barra de vida.")]
    public Slider healthSlider;

    [Tooltip("O deslocamento vertical para a barra flutuar acima do alvo.")]
    public float verticalOffset = 1.5f;

    private HealthSystem currentTarget;
    private Camera mainCamera;
    private Canvas canvas; // Referência para o Canvas

    // Flag para garantir que a configuração inicial aconteça apenas uma vez.
    private bool isInitialized = false;

    private void Awake()
    {
        // Pega as referências no Awake para garantir que existam
        canvas = GetComponent<Canvas>();
        mainCamera = Camera.main;

        // Esconde a barra de vida no início desativando o Canvas.
        // O script continua rodando, mas nada é desenhado na tela.
        if (canvas != null)
        {
            canvas.enabled = false;
        }
    }

    // Update verifica constantemente se as condições para mostrar a barra foram atendidas.
    private void Update()
    {
        // Se a barra já estiver visível, não precisamos fazer nada aqui.
        if (canvas.enabled) return;

        // Se o alvo já foi definido E o jogador comprou a skill...
        if (isInitialized && SkillTreeManager.CanSeeEnemyHealth)
        {
            // ... então ativamos o Canvas da barra de vida!
            Debug.Log("Skill ativada! Mostrando barra de vida para: " + currentTarget.name);
            canvas.enabled = true;
            UpdateHealthBar(currentTarget.GetCurrentHealth(), currentTarget.GetMaxHealth());
        }
    }

    // LateUpdate é melhor para seguir a câmera e objetos que se movem na física.
    private void LateUpdate()
    {
        if (currentTarget == null || mainCamera == null) return;
        
        transform.position = currentTarget.transform.position + Vector3.up * verticalOffset;
        transform.rotation = mainCamera.transform.rotation;
    }
    
    public void SetTarget(HealthSystem newTarget)
    {
        if (newTarget == null) return;
        
        Debug.Log("HealthBarController recebeu o alvo: " + newTarget.gameObject.name);

        currentTarget = newTarget;
        currentTarget.OnHealthChanged.AddListener(UpdateHealthBar);
        isInitialized = true;

        // Força uma verificação imediata para o caso de o inimigo ser criado
        // DEPOIS que a skill já foi comprada.
        Update(); 
    }
    
    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthSlider == null || currentTarget == null || maxHealth <= 0) return;
        
        healthSlider.value = currentHealth / maxHealth;
    }
    
    private void OnDestroy()
    {
        if (currentTarget != null)
        {
            currentTarget.OnHealthChanged.RemoveListener(UpdateHealthBar);
        }
    }
}
