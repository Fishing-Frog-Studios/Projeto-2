using UnityEngine;

// Garante que o inimigo SEMPRE tenha um HealthSystem.
[RequireComponent(typeof(HealthSystem))] 
public class BaseEnemy : MonoBehaviour
{
    // Referências para os componentes que este script irá gerenciar.
    // Elas são privadas porque nenhum outro script precisa mexer nelas diretamente.
    private HealthSystem healthSystem;
    private EnemyHealthBarController healthBarController;
    private VisionCamp visionCamp; // Opcional, mas bom para ter controle sobre a IA

    // Awake é chamado antes de Start, é o lugar perfeito para configurar referências.
    void Awake()
    {
        // --- BUSCANDO OS COMPONENTES ---

        // Pega o componente HealthSystem que está NESTE MESMO GameObject.
        healthSystem = GetComponent<HealthSystem>();

        // Procura nos OBJETOS FILHOS por um componente EnemyHealthBarController.
        healthBarController = GetComponentInChildren<EnemyHealthBarController>();
        
        // Procura nos OBJETOS FILHOS por um componente VisionCamp.
        visionCamp = GetComponentInChildren<VisionCamp>();

        // --- CONECTANDO OS COMPONENTES ---

        // É ESSENCIAL verificar se o controlador da barra de vida foi encontrado antes de usá-lo.
        if (healthBarController != null)
        {
            // Esta é a linha mais importante.
            // Ela "diz" ao controlador da barra de vida qual sistema de vida ele deve observar.
            healthBarController.SetTarget(healthSystem);
        }
        else
        {
            // Uma mensagem de aviso caso você esqueça de colocar o prefab da barra de vida como filho do inimigo.
            Debug.LogWarning("AVISO: Inimigo '" + gameObject.name + "' não tem um HealthBarController nos seus filhos.");
        }
    }

    // A partir daqui, você pode adicionar a lógica principal do inimigo,
    // como movimento, ataque, etc. Este script se torna o "cérebro" central.
    void Update()
    {
        // Exemplo: Se o inimigo vê o jogador, ele pode começar a perseguir.
        if (visionCamp != null && visionCamp.PlayerInSight())
        {
            // Coloque aqui a sua lógica de perseguição.
            // Ex: ChasePlayer();
        }
    }
}
