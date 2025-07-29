using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillTreeManager : MonoBehaviour
{
    [Header("Data Source")]
    public SkillTree_SO playerSkillTree;
    
    public static bool CanSeeEnemyHealth { get; private set; }

    [Header("UI References")]
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI skillNameText;
    public Image skillIcon;
    public TextMeshProUGUI pointsText;
    public Button evolveButton;
    public List<SkillNodeUI> manualNodes;

    private SkillNodeUI selectedNode;
    
    private void Awake()
    {
        CanSeeEnemyHealth = false;
    }

    private void OnEnable()
    {
        InitializeSkillTreeState();
        SetupManualNodes(); 
        UpdatePointsUI();
        if (evolveButton != null)
        {
            evolveButton.onClick.RemoveAllListeners(); 
            evolveButton.onClick.AddListener(EvolveSelectedSkill);
        }
        ClearSelection();
    }
    
    void InitializeSkillTreeState()
    {
        if (playerSkillTree == null || playerSkillTree.allSkills == null) return;
        bool pathChosen = (DataPlayer.Instance != null) && DataPlayer.Instance.pathHasBeenChosen;

        foreach (var skill in playerSkillTree.allSkills)
        {
            if (skill.state == SkillState.Unlocked) continue;
            if (skill.isPathStarter && pathChosen)
            {
                skill.state = SkillState.Locked;
                continue; 
            }
            if (skill.prerequisite == null)
            {
                skill.state = SkillState.Available;
            }
            else
            {
                skill.state = (skill.prerequisite.state == SkillState.Unlocked)
                    ? SkillState.Available
                    : SkillState.Locked;
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
        
        if (DataPlayer.Instance != null)
        {
            evolveButton.interactable = (node.assignedSkill.state == SkillState.Available && DataPlayer.Instance.skillPoints >= node.assignedSkill.cost);
        }
    }

    public void EvolveSelectedSkill()
    {
        if (selectedNode == null || selectedNode.assignedSkill.state != SkillState.Available) return;
        if (DataPlayer.Instance == null || DataPlayer.Instance.skillPoints < selectedNode.assignedSkill.cost) return;

        if (selectedNode.assignedSkill.isPathStarter)
        {
            DataPlayer.Instance.pathHasBeenChosen = true;
        }

        DataPlayer.Instance.skillPoints -= selectedNode.assignedSkill.cost;
        selectedNode.assignedSkill.state = SkillState.Unlocked;
        
        ApplySkillEffect(selectedNode.assignedSkill);
        
        InitializeSkillTreeState();
        RefreshAllNodes();
        UpdatePointsUI();
        ClearSelection();
    }
    
    // <<< VERSÃO FINAL COM TODAS AS 11 SKILLS IMPLEMENTADAS >>>
    private void ApplySkillEffect(SkillData skill)
    {
        if (skill == null || DataPlayer.Instance == null) return;
        
        switch (skill.skillName.Trim())
        {
            case "skill 1": // +20% Dano
                DataPlayer.Instance.danoBase *= 1.20f;
                break;

            case "skill 2": // +200 HP
                DataPlayer.Instance.vidaMaxima += 200;
                if (DataPlayer.Instance.playerHealthSystem != null)
                {
                    DataPlayer.Instance.playerHealthSystem.IncreaseMaxHealth(200);
                }
                break;

            case "skill 3": // Ver vida dos inimigos
                CanSeeEnemyHealth = true;
                break;

            case "skill 4": // 2 projéteis
                DataPlayer.Instance.projectileCount = 2;
                break;

            case "skill 5": // Pode ficar invisível
                DataPlayer.Instance.canBecomeInvisible = true;
                break;
                
            case "skill 6": // +3 de espaço de projétil
                DataPlayer.Instance.maxAmmo += 3;
                DataPlayer.Instance.currentAmmo += 3; // Também adiciona munição atual para o bônus ser imediato
                break;
                
            case "skill 7": // 3 projéteis giram ao redor
                DataPlayer.Instance.currentAttackMode = AttackMode.Orbiting;
                DataPlayer.Instance.orbitingProjectilesCount = 3;
                break;

            case "skill 8": // Buff pós-invisibilidade (Não é -20% de recarga)
                DataPlayer.Instance.hasPostInvisibilityBuffSkill = true;
                break;
                
            case "skill 9": // +20% de dano e projéteis teleguiados
                DataPlayer.Instance.danoBase *= 1.20f;
                DataPlayer.Instance.projectilesAreHoming = true;
                break;

            case "skill 10": // 6 projéteis que giram mais rápido
                DataPlayer.Instance.orbitingProjectilesCount = 6;
                DataPlayer.Instance.orbitDuration *= 0.75f; // Reduz a duração em 25% para girar mais rápido
                break;

            case "skill 11": // Invisibilidade melhorada e +100 HP
                DataPlayer.Instance.timeToBecomeInvisible -= 2f;
                if (DataPlayer.Instance.timeToBecomeInvisible < 1f)
                {
                    DataPlayer.Instance.timeToBecomeInvisible = 1f;
                }
                DataPlayer.Instance.vidaMaxima += 100;
                if (DataPlayer.Instance.playerHealthSystem != null)
                {
                    DataPlayer.Instance.playerHealthSystem.IncreaseMaxHealth(100);
                }
                break;

            default:
                Debug.LogWarning($"Skill '{skill.skillName}' comprada, mas nenhum efeito foi implementado.");
                break;
        }
    }
    
    void ClearSelection()
    {
        selectedNode = null;
        descriptionText.text = "Selecione uma habilidade para ver os detalhes.";
        skillNameText.text = "Nenhuma Habilidade Selecionada";
        
        if(skillIcon != null) skillIcon.enabled = false;
        if(evolveButton != null) evolveButton.interactable = false;
    }

    void RefreshAllNodes()
    {
        foreach (SkillNodeUI node in manualNodes)
        {
            if(node != null) node.UpdateVisuals();
        }
    }

    void UpdatePointsUI() 
    {
        if (pointsText != null && DataPlayer.Instance != null)
        {
            pointsText.text = $"Pontos: {DataPlayer.Instance.skillPoints}";
        }
    }
}
