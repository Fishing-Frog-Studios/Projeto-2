using UnityEngine;
using System.Collections.Generic; // Precisamos disso para a List

// AGORA É UM SCRIPTABLEOBJECT
[CreateAssetMenu(fileName = "NewSkill", menuName = "RPG/Skill")]
public class SkillData : ScriptableObject 
{
    public enum SkillState
    {
        Locked,
        Available,
        Unlocked
    }

    // Não precisamos mais de um ID, o nome do arquivo será nosso ID.
    // public string skillID; 
    public string skillName;
    [TextArea(3, 5)]
    public string description;
    public Sprite icon;
    public int cost = 1;

    public SkillState state = SkillState.Locked;

    // As conexões agora são listas de outros ScriptableObjects
    public SkillData prerequisite; 
    public List<SkillData> skillsToUnlock; // Mudamos para List<T> para ser mais flexível
    
    public bool isPathChoice = false;
    public List<SkillData> pathsToLock;
}
