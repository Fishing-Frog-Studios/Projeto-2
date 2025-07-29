using UnityEngine;

// Enum para o modo de ataque, pode ser usado por outros scripts.
public enum AttackMode { Standard, Orbiting }

public class DataPlayer : MonoBehaviour
{
    // A instância estática que permite acesso global.
    public static DataPlayer Instance { get; private set; }

    [Header("Data Sources")]
    public SkillTree_SO playerSkillTree;

    [Header("Player Stats")]
    public string nome = "Werbet";
    public int level = 0;
    public float vidaMaxima = 700f;
    public float manaMaxima = 1f;
    public int ouro = 0;
    public float moveSpeed = 5f;
    public float ataqueSpeed = 0.4f;
    public float danoBase = 60f;

    [Header("Skill System")]
    public int skillPoints = 6;
    public bool pathHasBeenChosen = false;
    public bool hasPostInvisibilityBuffSkill = false; 

    [Header("Projectile Skills")]
    public int projectileCount = 1;
    public int maxAmmo = 5;
    public int currentAmmo;
    public float ammoRegenRate = 2f;
    public bool projectilesAreHoming = false;

    [Header("Invisibility Skills")]
    public bool canBecomeInvisible = false;
    public float timeToBecomeInvisible = 3f;
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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // A rotina de reset agora garante que a árvore esteja sempre limpa no início.
        ResetGameDataAndSkillTree();
        
        currentAmmo = maxAmmo;
    }

    /// <summary>
    /// <<< MÉTODO CORRIGIDO E MELHORADO >>>
    /// Reseta o estado de todas as skills e os dados do jogador para o início.
    /// Isso previne que o estado 'Unlocked' salvo no editor quebre o jogo.
    /// </summary>
    private void ResetGameDataAndSkillTree()
    {
        if (playerSkillTree != null && playerSkillTree.allSkills != null)
        {
            foreach (var skill in playerSkillTree.allSkills)
            {
                // Força TODAS as skills a começarem como 'Locked', ignorando o que está salvo no arquivo.
                skill.state = SkillState.Locked;
            }

            pathHasBeenChosen = false;
            projectilesAreHoming = false;
            hasPostInvisibilityBuffSkill = false;
            
            // Você também pode resetar os stats base aqui, se quiser
            // Ex: vidaMaxima = 700; danoBase = 60; etc.

            Debug.Log("DataPlayer: Estado do Jogo e da Skill Tree foi resetado para o início.");
        }
    }

    public void RegisterPlayerReferences(HealthSystem health, Transform trans, Camera cam)
    {
        playerHealthSystem = health;
        playerTransform = trans;
        if (cam != null) mainCamera = cam;
        InitializePlayer();
    }

    private void InitializePlayer()
    {
        if (playerHealthSystem != null)
        {
            playerHealthSystem.SetInitialHealth(vidaMaxima);
        }
    }
}
