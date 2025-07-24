using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla uma barra de vida em World Space que flutua sobre um alvo.
/// A barra só é ativada se a skill correspondente no SkillTreeManager for comprada.
/// </summary>
public class EnemyHealthBarController : MonoBehaviour
{
    [Tooltip("A referência para o componente Slider que representa a barra de vida.")]
    public Slider healthSlider;

    [Tooltip("O deslocamento vertical para a barra flutuar acima do alvo.")]
    public float verticalOffset = 1.5f;

    private HealthSystem currentTarget;
    private Camera mainCamera;

    private void Awake()
    {
        // Garante que a barra de vida sempre comece o jogo escondida.
        gameObject.SetActive(false);
    }

    private void Start()
    {
        // Guarda a referência da câmera principal para otimização.
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (currentTarget == null || mainCamera == null) return;

        // Faz a barra de vida seguir a posição do alvo no mundo, com um deslocamento vertical.
        transform.position = currentTarget.transform.position + Vector3.up * verticalOffset;
        
        // Faz a barra de vida sempre olhar para a câmera.
        transform.rotation = mainCamera.transform.rotation;
    }
    
    public void SetTarget(HealthSystem newTarget)
    {
        if (currentTarget != null)
        {
            currentTarget.OnHealthChanged.RemoveListener(UpdateHealthBar);
        }

        currentTarget = newTarget;

        if (newTarget == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        if (SkillTreeManager.CanSeeEnemyHealth)
        {
            gameObject.SetActive(true);
            newTarget.OnHealthChanged.AddListener(UpdateHealthBar);
            UpdateHealthBar(newTarget.GetCurrentHealth(), newTarget.GetMaxHealth());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    
    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthSlider == null || currentTarget == null) return;
        
        if (maxHealth > 0)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }
}
