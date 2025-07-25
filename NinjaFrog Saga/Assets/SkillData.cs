using UnityEngine;

// A enum para os estados da skill é essencial e está correta.
public enum SkillState { Locked, Available, Unlocked }

// O atributo CreateAssetMenu permite criar instâncias deste objeto no editor.
[CreateAssetMenu(fileName = "NewSkillData", menuName = "Skill Tree/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("Informações Básicas")]
    public string skillName;
    [TextArea(3, 5)]
    public string description;
    public Sprite icon;
    public int cost = 1;

    [Header("Estado da Skill (gerenciado em tempo de execução)")]
    // O estado inicial padrão é 'Locked'.
    public SkillState state = SkillState.Locked;

    [Header("Estrutura da Árvore")]
    // Pré-requisito: A skill que precisa ser comprada ANTES desta.
    // Deixe como 'None' para as skills iniciais da árvore.
    public SkillData prerequisite;

    // <<< CORREÇÃO E SIMPLIFICAÇÃO >>>
    // Esta é a única variável que precisamos para a lógica de bloqueio de caminhos.
    // Marque esta caixa no Inspector APENAS para as 3 skills que iniciam um caminho.
    public bool isPathStarter;

    // As variáveis 'skillsToUnlock' e 'pathsToLock' foram removidas.
    // A lógica agora é centralizada no SkillTreeManager, que apenas olha
    // para os 'prerequisite' para construir a árvore. Isso é mais robusto
    // e menos propenso a erros de configuração manual.
}
