using UnityEngine;

/// <summary>
/// Gerenciador Singleton que mantém referências importantes do jogo (jogador, UI, etc.)
/// e persiste entre as mudanças de cena usando DontDestroyOnLoad.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    // A instância estática que permite acesso global de qualquer script.
    public static PlayerManager Instance { get; private set; }

    [Header("Referências do Jogador")]
    [Tooltip("Arraste o GameObject do jogador que tem o HealthSystem.")]
    public HealthSystem playerHealthSystem;
    [Tooltip("Arraste o Transform do GameObject do jogador.")]
    public Transform playerTransform;
    
    [Header("Referências da UI do Jogo")]
    [Tooltip("Arraste o GameObject da Barra de Vida do Inimigo que está no HUD.")]
    public EnemyHealthBarController healthBarController;

    private void Awake()
    {
        // Lógica do Singleton para garantir que só exista uma instância do PlayerManager.
        if (Instance != null && Instance != this)
        {
            // Se já existe uma, esta é uma duplicata. Destrua-a.
            Destroy(gameObject);
            return;
        }
        
        // Se não existe, esta se torna a instância.
        Instance = this;
        
        // Marca este objeto para não ser destruído ao carregar novas cenas.
        DontDestroyOnLoad(gameObject);
    }
}
