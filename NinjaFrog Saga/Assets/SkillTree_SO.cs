using System.Collections.Generic;
using UnityEngine;

// Isso cria uma opção no menu Assets -> Create -> RPG -> Skill Tree
[CreateAssetMenu(fileName = "NewSkillTree", menuName = "RPG/Skill Tree")]
public class SkillTree_SO : ScriptableObject
{
    public List<SkillData> allSkills;
}
