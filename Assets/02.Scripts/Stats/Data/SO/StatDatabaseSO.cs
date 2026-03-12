using UnityEngine;

namespace DungeonRush.Stats.Data.SO
{
    [CreateAssetMenu(fileName = "StatDatabase", menuName = "DungeonRush/Stats/Stat Database")]
    public class StatDatabaseSO : ScriptableObject
    {
        [SerializeField] private CharacterSpecSO[] _characters;
        [SerializeField] private SkillSpecSO[] _skills;

        public CharacterSpecSO[] Characters => _characters;
        public SkillSpecSO[] Skills => _skills;

#if UNITY_EDITOR
        public void SetCharacters(CharacterSpecSO[] characters)
        {
            _characters = characters;
        }

        public void SetSkills(SkillSpecSO[] skills)
        {
            _skills = skills;
        }
#endif
    }
}
