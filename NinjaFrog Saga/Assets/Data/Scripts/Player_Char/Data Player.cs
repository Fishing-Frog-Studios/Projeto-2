using UnityEngine;

public class DataPlayer : MonoBehaviour
{
    // <<< CORRIGIDO: "Instance" com 'n' >>>
    public static DataPlayer Instance { get; private set; }

    [Header("Player Stats")]
    public string nome = "Werbet";
    public int level = 0;
    public float vidaMaxima = 500f;
    public float manaMaxima = 1f;
    public int ouro = 0;
    public float moveSpeed = 5f;
    public float ataqueSpeed = 0.5f;
    public float danoBase = 50f;
    
    [Header("Skill System")]
    public int skillPoints = 10;

    [Header("Player References")]
    public HealthSystem playerHealthSystem;
    public Transform playerTransform;
    
    [Header("System References")]
    public Camera mainCamera; 
    
    private void Awake()
    {
        // <<< CORRIGIDO: "Instance" com 'n' >>>
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // <<< CORRIGIDO: "Instance" com 'n' >>>
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void RegisterPlayerReferences(HealthSystem health, Transform trans, Camera cam)
    {
        playerHealthSystem = health;
        playerTransform = trans;
        mainCamera = cam;
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
