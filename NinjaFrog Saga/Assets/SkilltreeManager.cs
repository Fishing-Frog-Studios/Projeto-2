using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillTreeManager : MonoBehaviour
{
    [Header("Data Source")]
    public SkillTree_SO playerSkillTree;

    public static bool CanSeeEnemyHealth { get; private set; }
    
    // Flag para garantir que o estado inicial das skills só seja definido uma vez por sessão de jogo.
    private static bool hasSkillTreeBeenInitialized = false;

    [Header("UI References")]
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI skillNameText;
    public Image skillIcon;
    public TextMeshProUGUI pointsText;
    public Button evolveButton;
    public List<SkillNodeUI> manualNodes;

    private SkillNodeUI selectedNode;

    // Usamos OnEnable porque ele é chamado toda vez que o objeto (e a cena) se torna ativo.
    // Isso é mais confiável do que Start/Awake para UIs que são carregadas e descarregadas.
    private void OnEnable()
    {
        // Se o estado da árvore de skills ainda não foi definido nesta sessão de jogo...
        if (!hasSkillTreeBeenInitialized)
        {
            InitializeSkillTreeState();
            hasSkillTreeBeenInitialized = true;
        }

        SetupManualNodes(); 
        UpdatePointsUI();
        if (evolveButton != null)
        {
            // Removemos para evitar adicionar o mesmo listener múltiplas vezes.
            evolveButton.onClick.RemoveAllListeners(); 
            evolveButton.onClick.AddListener(EvolveSelectedSkill);
        }
        ClearSelection();
    }
    
    // Define o estado inicial de todas as skills.
    void InitializeSkillTreeState()
    {
        if (playerSkillTree == null || playerSkillTree.allSkills == null) return;

        foreach (var skill in playerSkillTree.allSkills)
        {
            // O estado "desbloqueado" de uma skill deve persistir.
            if (skill.state != SkillData.SkillState.Unlocked)
            {
                // Se não tem pré-requisito, está disponível. Senão, está bloqueada.
                skill.state = (skill.prerequisite == null) 
                    ? SkillData.SkillState.Available 
                    : SkillData.SkillState.Locked;
            }
        }
    }

    void SetupManualNodes()
    {
        if (manualNodes.Count != playerSkillTree.allSkills.Count)
        {
            Debug.LogError("ERRO: O número de nós na UI é diferente do número de skills nos dados!");
            return;
        }
        for (int i = 0; i < playerSkillTree.allSkills.Count; i++)
        {
            manualNodes[i].Setup(playerSkillTree.allSkills[i], this);
        }
    }

    public void OnSkillNodeSelected(SkillNodeUI node)
    {
        selectedNode = node;
        descriptionText.text = node.assignedSkill.description;
        skillNameText.text = node.assignedSkill.skillName;
        skillIcon.sprite = node.assignedSkill.icon;
        skillIcon.enabled = true;
        
        // <<< CORRIGIDO: Usa DataPlayer.Instance >>>
        if (DataPlayer.Instance != null)
        {
            evolveButton.interactable = (node.assignedSkill.state == SkillData.SkillState.Available && DataPlayer.Instance.skillPoints >= node.assignedSkill.cost);
        }
    }

    public void EvolveSelectedSkill()
    {
        // <<< CORRIGIDO: Usa DataPlayer.Instance >>>
        if (selectedNode == null || selectedNode.assignedSkill.state != SkillData.SkillState.Available) return;
        if (DataPlayer.Instance == null || DataPlayer.Instance.skillPoints < selectedNode.assignedSkill.cost) return;

        DataPlayer.Instance.skillPoints -= selectedNode.assignedSkill.cost;
        selectedNode.assignedSkill.state = SkillData.SkillState.Unlocked;

        foreach (var skillToUnlock in selectedNode.assignedSkill.skillsToUnlock)
        {
             if (skillToUnlock != null) 
             {
                skillToUnlock.state = SkillData.SkillState.Available;
             }
        }
        
        ApplySkillEffect(selectedNode.assignedSkill);
        
        RefreshAllNodes();
        UpdatePointsUI();
        ClearSelection();
    }
    
    private void ApplySkillEffect(SkillData skill)
    {
        if (skill == null) return;
        
        // <<< CORRIGIDO: Usa DataPlayer.Instance >>>
        if (DataPlayer.Instance == null)
        {
            Debug.LogError("ERRO CRÍTICO: DataPlayer.Instance não foi encontrado!");
            return;
        }

        switch (skill.skillName)
        {
            case "skill 2": // <<< Use o nome exato da sua skill, exemplo.
                if (DataPlayer.Instance.playerHealthSystem != null)
                {
                    DataPlayer.Instance.playerHealthSystem.IncreaseMaxHealth(200);
                    DataPlayer.Instance.vidaMaxima = DataPlayer.Instance.playerHealthSystem.GetMaxHealth();
                }
                break;

            case "skill 3": // <<< Use o nome exato da sua skill, exemplo.
                CanSeeEnemyHealth = true;
                break;
            
            case "skill 1": // <<< Use o nome exato da sua skill, exemplo.
                DataPlayer.Instance.danoBase *= 1.20f;
                break;
            
            default:
                Debug.LogWarning($"Skill '{skill.skillName}' comprada, mas nenhum efeito foi implementado.");
                break;
        }
    }
    
    void ClearSelection()
    {
        selectedNode = null;
        descriptionText.text = "Selecione uma habilidade para ver a descrição.";
        skillNameText.text = "";
        skillIcon.enabled = false;
        if(evolveButton != null) evolveButton.interactable = false;
    }

    void RefreshAllNodes()
    {
        foreach (var nodeUI in manualNodes)
        {
            nodeUI.UpdateVisuals();
        }
    }
    
    void UpdatePointsUI()
    {
        // <<< CORRIGIDO: Usa DataPlayer.Instance >>>
        if (DataPlayer.Instance != null)
        {
            pointsText.text = DataPlayer.Instance.skillPoints.ToString() + " pt";
        }
    }
}
