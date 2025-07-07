using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Este é o cérebro do sistema. Coloque em um objeto vazio na cena.
public class SkillTreeManager : MonoBehaviour
{
    [Header("Data & Player Stats")]
    [Tooltip("Arraste o asset SkillTree_SO que contém a lista de todas as skills.")]
    public SkillTree_SO playerSkillTree;
    [Tooltip("Pontos de habilidade que o jogador possui atualmente.")]
    public int playerSkillPoints = 10;

    [Header("UI References - Info Panel")]
    [Tooltip("O texto que exibe a descrição da habilidade selecionada.")]
    public TextMeshProUGUI descriptionText;
    [Tooltip("O texto que exibe o nome da habilidade selecionada.")]
    public TextMeshProUGUI skillNameText;
    [Tooltip("A imagem que exibe o ícone da habilidade selecionada.")]
    public Image skillIcon;
    [Tooltip("O texto que exibe a quantidade de pontos de habilidade.")]
    public TextMeshProUGUI pointsText;
    [Tooltip("O botão para comprar a habilidade selecionada.")]
    public Button evolveButton;

    [Header("UI References - Node List")]
    [Tooltip("A lista de todos os GameObjects de nós da árvore que você posicionou manualmente na cena.")]
    public List<SkillNodeUI> manualNodes;

    private SkillNodeUI selectedNode;

    void Start()
    {
        // 1. Prepara os dados das skills
        InitializeSkillTree();
        
        // 2. Conecta os dados aos nós da UI que já estão na cena
        SetupManualNodes(); 
        
        // 3. Atualiza a UI inicial
        UpdatePointsUI();
        
        // 4. Adiciona a função ao clique do botão
        evolveButton.onClick.AddListener(EvolveSelectedSkill);
        
        // 5. Garante que nada esteja selecionado no início
        ClearSelection();
    }
    
    // Reseta o estado de todas as skills no início do jogo.
    void InitializeSkillTree()
    {
        foreach (var skill in playerSkillTree.allSkills)
        {
            // Se a skill não tem pré-requisito, ela está disponível. Senão, está bloqueada.
            if (skill.prerequisite == null)
            {
                skill.state = SkillData.SkillState.Available;
            }
            else
            {
                skill.state = SkillData.SkillState.Locked;
            }
        }
    }

    // Associa cada nó da UI posicionado manualmente a um dado de skill.
    void SetupManualNodes()
    {
        if (manualNodes.Count != playerSkillTree.allSkills.Count)
        {
            Debug.LogError("ERRO: O número de nós na UI (" + manualNodes.Count + ") é diferente do número de skills nos dados (" + playerSkillTree.allSkills.Count + ")!");
            return;
        }

        for (int i = 0; i < playerSkillTree.allSkills.Count; i++)
        {
            // Pega o nó da UI e o dado da skill na mesma ordem e os associa.
            manualNodes[i].Setup(playerSkillTree.allSkills[i], this);
        }
    }

    // Chamado pelo script SkillNodeUI quando um nó é clicado.
    public void OnSkillNodeSelected(SkillNodeUI node)
    {
        selectedNode = node;

        // Atualiza o painel de informações com os dados da skill clicada.
        descriptionText.text = node.assignedSkill.description;
        skillNameText.text = node.assignedSkill.skillName;
        skillIcon.sprite = node.assignedSkill.icon;
        skillIcon.enabled = true; // Mostra a imagem do ícone

        // Habilita ou desabilita o botão "Evoluir" baseado nas condições.
        evolveButton.interactable = (node.assignedSkill.state == SkillData.SkillState.Available && playerSkillPoints >= node.assignedSkill.cost);
    }

    // Chamado quando o botão "Evoluir" é clicado.
  public void EvolveSelectedSkill()
    {
        if (selectedNode == null || selectedNode.assignedSkill.state != SkillData.SkillState.Available) return;
        if (playerSkillPoints < selectedNode.assignedSkill.cost) return;

        // 1. Subtrai os pontos e atualiza o estado da skill comprada.
        playerSkillPoints -= selectedNode.assignedSkill.cost;
        selectedNode.assignedSkill.state = SkillData.SkillState.Unlocked;

        // 2. Lógica de bloqueio de caminhos alternativos.
        if (selectedNode.assignedSkill.isPathChoice)
        {
            foreach (var path in selectedNode.assignedSkill.pathsToLock)
            {
                if (path != null && path.state == SkillData.SkillState.Available) 
                {
                    path.state = SkillData.SkillState.Locked;
                }
            }
        }
        
        // 3. Libera as próximas skills na árvore.
        foreach (var skillToUnlock in selectedNode.assignedSkill.skillsToUnlock)
        {
             if (skillToUnlock != null) 
             {
                skillToUnlock.state = SkillData.SkillState.Available;
             }
        }
        
        // 4. Atualiza toda a UI para refletir as mudanças.
        RefreshAllNodes();
        UpdatePointsUI();
        ClearSelection(); // Limpa a seleção para evitar compras repetidas.
    }
    
    // Limpa o painel de informações.
    void ClearSelection()
    {
        selectedNode = null;
        descriptionText.text = "Selecione uma habilidade para ver a descrição.";
        skillNameText.text = "";
        skillIcon.enabled = false; // Esconde a imagem do ícone
        evolveButton.interactable = false;
    }

    // Pede para cada nó da UI se redesenhar com base no seu novo estado.
    void RefreshAllNodes()
    {
        foreach (var nodeUI in manualNodes)
        {
            nodeUI.UpdateVisuals();
        }
    }
    
    // Atualiza o texto que mostra os pontos de habilidade.
    void UpdatePointsUI()
    {
        pointsText.text = playerSkillPoints.ToString() + " pt";
    }
}
