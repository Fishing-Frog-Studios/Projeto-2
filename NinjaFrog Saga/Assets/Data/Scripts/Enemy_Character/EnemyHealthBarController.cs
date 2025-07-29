using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class EnemyHealthBarController : MonoBehaviour
{
    [Tooltip("A referência para o componente Slider que representa a barra de vida.")]
    public Slider healthSlider;

    [Tooltip("O deslocamento vertical para a barra flutuar acima do alvo.")]
    public float verticalOffset = 1.5f;

    private HealthSystem currentTarget;
    private Camera mainCamera;
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        mainCamera = Camera.main;

        // O Canvas começa desativado por padrão.
        if (canvas != null)
        {
            canvas.enabled = false;
        }
    }

    // <<< LÓGICA DO UPDATE CORRIGIDA E SIMPLIFICADA >>>
    private void Update()
    {
        // Se não temos um alvo, não há o que fazer.
        if (currentTarget == null) return;

        // A visibilidade do Canvas agora espelha DIRETAMENTE o estado da skill.
        // Se a skill for true, o canvas é enabled. Se for false, é disabled.
        // Isso funciona a cada frame, garantindo que a barra apareça e desapareça corretamente.
        canvas.enabled = SkillTreeManager.CanSeeEnemyHealth;
    }

    private void LateUpdate()
    {
        // A lógica de seguir o alvo e a câmera está perfeita.
        if (currentTarget == null || mainCamera == null) return;
        
        transform.position = currentTarget.transform.position + Vector3.up * verticalOffset;
        transform.rotation = mainCamera.transform.rotation;
    }
    
    public void SetTarget(HealthSystem newTarget)
    {
        if (newTarget == null) return;
        
        // Esta lógica de configuração está perfeita.
        currentTarget = newTarget;
        currentTarget.OnHealthChanged.AddListener(UpdateHealthBar);

        // Atualiza a barra uma vez para mostrar a vida inicial do inimigo.
        UpdateHealthBar(currentTarget.GetCurrentHealth(), currentTarget.GetMaxHealth());
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
