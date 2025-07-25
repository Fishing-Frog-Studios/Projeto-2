using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillNodeUI : MonoBehaviour
{
    public SkillData assignedSkill;
    public SkillTreeManager manager;

    [Header("UI Elements (Conecte no Inspector do Prefab)")]
    public Image backgroundFrameImage; // A imagem da Borda
    public Image iconImage;            // A imagem do Ícone
    public TextMeshProUGUI nameText;
    public Button skillButton;

    [Header("State Sprites (Conecte no Inspector do Prefab)")]
    public Sprite lockedIcon;
    public Sprite unlockedBackgroundSprite; // O sprite da borda que aparece ao comprar

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

    // ATUALIZADO: Agora esconde/mostra a borda
    public void UpdateVisuals()
    {
        if (assignedSkill == null || iconImage == null || backgroundFrameImage == null) return;

        switch (assignedSkill.state)
        {
            case SkillState.Locked:
                iconImage.sprite = lockedIcon;
                skillButton.interactable = false;
                backgroundFrameImage.enabled = false; // <<< ESCONDE A BORDA
                break;

            case SkillState.Available:
                iconImage.sprite = assignedSkill.icon;
                skillButton.interactable = true;
                backgroundFrameImage.enabled = false; // <<< ESCONDE A BORDA
                break;

            case SkillState.Unlocked:
                iconImage.sprite = assignedSkill.icon;
                skillButton.interactable = false; // Já foi comprada
                
                // Define o sprite da borda e a torna visível
                if (unlockedBackgroundSprite != null)
                {
                    backgroundFrameImage.sprite = unlockedBackgroundSprite;
                    backgroundFrameImage.enabled = true; // <<< MOSTRA A BORDA
                }
                break;
        }
    }

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
