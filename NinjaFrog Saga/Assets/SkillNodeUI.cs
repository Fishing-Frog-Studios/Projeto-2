using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Este script vai no GameObject raiz de cada nó/botão de habilidade.
public class SkillNodeUI : MonoBehaviour
{
    // --- Variáveis de Lógica (preenchidas pelo SkillTreeManager) ---
    public SkillData assignedSkill;
    public SkillTreeManager manager;

    [Header("UI Elements (Conecte no Inspector do Prefab)")]
    [Tooltip("A imagem que serve como moldura/fundo do nó. Geralmente, a imagem do próprio objeto principal.")]
    public Image backgroundFrameImage; // <<--- ESTE CAMPO ESTÁ FALTANDO NO SEU SCRIPT ATUAL
    [Tooltip("A imagem que mostra o ícone da skill ou o cadeado. Geralmente, uma imagem filha.")]
    public Image iconImage;
    [Tooltip("O texto para o nome da habilidade (opcional).")]
    public TextMeshProUGUI nameText;
    [Tooltip("O componente Button deste objeto.")]
    public Button skillButton;

    [Header("State Sprites (Conecte no Inspector do Prefab)")]
    [Tooltip("O sprite para quando a skill está bloqueada (cadeado).")]
    public Sprite lockedIcon;
    [Tooltip("O sprite de fundo para quando a skill for comprada.")]
    public Sprite unlockedBackgroundSprite; 

    // Função chamada pelo SkillTreeManager para inicializar este nó.
    public void Setup(SkillData skill, SkillTreeManager stManager)
    {
        assignedSkill = skill;
        manager = stManager;
        
        if (nameText != null)
        {
            nameText.text = assignedSkill.skillName;
        }

        skillButton.onClick.AddListener(OnNodeClicked);
        UpdateVisuals();
    }

    // Atualiza a aparência do nó baseado no seu estado atual.
    public void UpdateVisuals()
    {
        if (assignedSkill == null) return;
        
        // Armazena o sprite original da moldura para podermos restaurá-lo se necessário.
        Sprite originalBackground = null;
        if(backgroundFrameImage != null)
        {
            originalBackground = backgroundFrameImage.sprite;
        }


        switch (assignedSkill.state)
        {
            case SkillData.SkillState.Locked:
                iconImage.sprite = lockedIcon;
                skillButton.interactable = false;
                if(backgroundFrameImage != null && originalBackground != null)
                {
                    backgroundFrameImage.sprite = originalBackground; // Garante que o fundo é o padrão
                }
                break;

            case SkillData.SkillState.Available:
                iconImage.sprite = assignedSkill.icon;
                skillButton.interactable = true;
                if(backgroundFrameImage != null && originalBackground != null)
                {
                   backgroundFrameImage.sprite = originalBackground; // Garante que o fundo é o padrão
                }
                break;

            case SkillData.SkillState.Unlocked:
                iconImage.sprite = assignedSkill.icon;
                skillButton.interactable = false; // Já foi comprada.
                
                // Se um sprite de fundo de desbloqueado foi definido, troque o sprite da moldura.
                if (unlockedBackgroundSprite != null && backgroundFrameImage != null)
                {
                    backgroundFrameImage.sprite = unlockedBackgroundSprite;
                }
                break;
        }
    }

    // Quando o botão é clicado, ele avisa o manager qual nó foi selecionado.
    public void OnNodeClicked()
    {
        if (manager != null)
        {
            manager.OnSkillNodeSelected(this);
        }
        else
        {
            Debug.LogError("ERRO: A referência ao SkillTreeManager está NULA no nó: " + gameObject.name);
        }
    }
}
