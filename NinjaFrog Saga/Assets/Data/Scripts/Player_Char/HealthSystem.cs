using UnityEngine;
using UnityEngine.Events; // Para usar UnityEvents

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    // Eventos que outros scripts podem "ouvir"
    public UnityEvent<float, float> OnHealthChanged; // Envia (vidaAtual, vidaMaxima)
    public UnityEvent OnDied;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        currentHealth += amount; // Também cura o jogador pelo valor aumentado
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    public void SetInitialHealth(float newMaxHealth)
{
    maxHealth = newMaxHealth;
    currentHealth = newMaxHealth;

    OnHealthChanged?.Invoke(currentHealth, maxHealth);
}

    private void Die()
    {
        OnDied?.Invoke();
        // Desativar o objeto ou tocar uma animação de morte, etc.
        Debug.Log(gameObject.name + " morreu!");
    }
}
