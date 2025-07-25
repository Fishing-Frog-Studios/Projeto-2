using UnityEngine;

// Enum para o modo de ataque, pode ser usado por outros scripts.
public enum AttackMode { Standard, Orbiting }

public class DataPlayer : MonoBehaviour
{
    // A instância estática que permite acesso global.
    public static DataPlayer Instance { get; private set; }

    [Header("Data Sources")]
    // Referência à árvore de skills, essencial para a lógica de reset.
    public SkillTree_SO playerSkillTree;

    [Header("Player Stats")]
    public string nome = "Werbet";
    public int level = 0;
    public float vidaMaxima = 500f;
    public float manaMaxima = 1f;
    public int ouro = 0;
    public float moveSpeed = 5f;
    public float ataqueSpeed = 2.0f;
    public float danoBase = 50f;

    [Header("Skill System")]
    public int skillPoints = 10;
    public bool pathHasBeenChosen = false;

    [Header("Projectile Skills")]
    public int projectileCount = 1;
    public int maxAmmo = 5;
    public int currentAmmo;
    public float ammoRegenRate = 2f;
    // Flag para ativar/desativar a lógica de projéteis teleguiados.
    public bool projectilesAreHoming = false;

    [Header("Invisibility Skills")]
    public bool canBecomeInvisible = false;
    public float timeToBecomeInvisible = 5f;
    public bool isInvisible = false;
    public bool nextAttackHasBonusDamage = false;

    [Header("Orbiting Attack Skills")]
    public AttackMode currentAttackMode = AttackMode.Standard;
    public int orbitingProjectilesCount = 5;
    public float orbitDuration = 1f;
    public float orbitRadius = 2.5f;

    [Header("Player References (preenchido em tempo de execução)")]
    public HealthSystem playerHealthSystem;
    public Transform playerTransform;

    [Header("System References (preenchido em tempo de execução)")]
    public Camera mainCamera;

    private void Awake()
    {
        // Lógica do Singleton para garantir que só exista uma instância.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // --- ROTINA DE RESET DO JOGO ---
        // Executada uma única vez quando o jogo inicia.
        ResetSkillTree();
        
        // Inicia a munição do jogador.
        currentAmmo = maxAmmo;
    }

    /// <summary>
    /// Reseta o estado de todas as skills para 'Locked' e a escolha de caminho.
    /// Essencial para que os testes no editor comecem sempre com uma árvore limpa.
    /// </summary>
    private void ResetSkillTree()
    {
        if (playerSkillTree != null && playerSkillTree.allSkills != null)
        {
            foreach (var skill in playerSkillTree.allSkills)
            {
                // Força o estado de todas as skills a começarem como bloqueadas.
                skill.state = SkillState.Locked;
            }

            // Reseta a flag de escolha de caminho.
            pathHasBeenChosen = false;
            
            // Desativa a flag de projéteis teleguiados no início de cada jogo.
            projectilesAreHoming = false;

            Debug.Log("DataPlayer: Estado da Skill Tree foi resetado para o início do jogo.");
        }
    }

    /// <summary>
    /// Método chamado por outros scripts (como o PlayerRegistrar) para conectar o jogador.
    /// </summary>
    public void RegisterPlayerReferences(HealthSystem health, Transform trans, Camera cam)
    {
        playerHealthSystem = health;
        playerTransform = trans;
        if (cam != null) mainCamera = cam;
        InitializePlayer();
    }

    /// <summary>
    /// Define os status iniciais do jogador com base nos valores do DataPlayer.
    /// </summary>
    private void InitializePlayer()
    {
        if (playerHealthSystem != null)
        {
            playerHealthSystem.SetInitialHealth(vidaMaxima);
        }
    }
}
