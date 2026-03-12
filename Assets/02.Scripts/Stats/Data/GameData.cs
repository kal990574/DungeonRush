using System;
using System.Collections.Generic;
using DungeonRush.Stats.Repository;

namespace DungeonRush.Stats.Data
{
    public class GameData
    {
        private readonly ICharacterRepository _characterRepo;
        private readonly ISkillRepository _skillRepo;

        // 레벨업 누적 캐시: (skillId, level) → 누적 적용된 SkillAttackData/SkillPassiveData.
        private readonly Dictionary<(string, int), SkillAttackData> _attackCache = new();
        private readonly Dictionary<(string, int), SkillPassiveData> _passiveCache = new();

        public GameData(ICharacterRepository characterRepo, ISkillRepository skillRepo)
        {
            _characterRepo = characterRepo ?? throw new ArgumentNullException(nameof(characterRepo));
            _skillRepo = skillRepo ?? throw new ArgumentNullException(nameof(skillRepo));
        }

        public CharacterSpec GetCharacter(string id)
        {
            return _characterRepo.GetById(id);
        }

        public IReadOnlyList<CharacterSpec> GetAllCharacters()
        {
            return _characterRepo.GetAll();
        }

        public SkillSpec GetSkill(string id)
        {
            return _skillRepo.GetById(id);
        }

        public IReadOnlyList<SkillSpec> GetAllSkills()
        {
            return _skillRepo.GetAll();
        }

        // 지정 레벨까지 누적 적용한 공격 파라미터를 반환.
        public float GetSkillParam(string skillId, string paramName, int level)
        {
            var spec = _skillRepo.GetById(skillId);
            if (spec == null)
            {
                throw new ArgumentException($"Skill not found: {skillId}");
            }

            if (spec.IsAttack)
            {
                var resolved = GetResolvedAttackData(skillId, level);
                return resolved.GetParam(paramName);
            }
            else
            {
                var resolved = GetResolvedPassiveData(skillId, level);
                return resolved.GetParam(paramName);
            }
        }

        public SkillAttackData GetResolvedAttackData(string skillId, int level)
        {
            if (_attackCache.TryGetValue((skillId, level), out var cached))
            {
                return cached;
            }

            var spec = _skillRepo.GetById(skillId);
            if (spec?.AttackData == null)
            {
                return null;
            }

            var result = spec.AttackData.Clone();
            var levelUps = _skillRepo.GetLevelUps(skillId);

            foreach (var levelUp in levelUps)
            {
                if (levelUp.Level > level)
                {
                    break;
                }

                foreach (var mod in levelUp.Modifications)
                {
                    ApplyModToAttack(result, mod);
                }
            }

            _attackCache[(skillId, level)] = result;
            return result;
        }

        public SkillPassiveData GetResolvedPassiveData(string skillId, int level)
        {
            if (_passiveCache.TryGetValue((skillId, level), out var cached))
            {
                return cached;
            }

            var spec = _skillRepo.GetById(skillId);
            if (spec?.PassiveData == null)
            {
                return null;
            }

            var result = spec.PassiveData.Clone();
            var levelUps = _skillRepo.GetLevelUps(skillId);

            foreach (var levelUp in levelUps)
            {
                if (levelUp.Level > level)
                {
                    break;
                }

                foreach (var mod in levelUp.Modifications)
                {
                    ApplyModToPassive(result, mod);
                }
            }

            _passiveCache[(skillId, level)] = result;
            return result;
        }

        private static void ApplyModToAttack(SkillAttackData data, ParamModification mod)
        {
            float current = data.GetParam(mod.ParamName);
            float newValue = ApplyMod(current, mod);
            data.SetParam(mod.ParamName, newValue);
        }

        private static void ApplyModToPassive(SkillPassiveData data, ParamModification mod)
        {
            float current = data.GetParam(mod.ParamName);
            float newValue = ApplyMod(current, mod);
            data.SetParam(mod.ParamName, newValue);
        }

        private static float ApplyMod(float current, ParamModification mod)
        {
            return mod.ValueType switch
            {
                ValueType.Set => mod.Value,
                ValueType.Flat => current + mod.Value,
                ValueType.Percent => current * (1f + mod.Value),
                _ => current
            };
        }
    }
}
