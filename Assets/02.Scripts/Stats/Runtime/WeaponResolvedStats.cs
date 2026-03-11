using DungeonRush.Stats.Data;

namespace DungeonRush.Stats.Runtime
{
    // 무기(스킬) 최종 스펙. Dirty Flag 패턴으로 lazy 재계산.
    public class WeaponResolvedStats
    {
        private readonly GameData _gameData;
        private readonly EquippedWeapon _weapon;
        private readonly StatSheet _characterStats;

        private SkillAttackData _resolved;
        private bool _isDirty = true;

        public WeaponResolvedStats(GameData gameData, EquippedWeapon weapon, StatSheet characterStats)
        {
            _gameData = gameData;
            _weapon = weapon;
            _characterStats = characterStats;
        }

        public bool IsDirty => _isDirty;

        public void MarkDirty()
        {
            _isDirty = true;
        }

        public SkillAttackData Resolve()
        {
            if (!_isDirty && _resolved != null)
            {
                return _resolved;
            }

            var baseData = _gameData.GetResolvedAttackData(_weapon.SkillId, _weapon.CurrentLevel);
            if (baseData == null)
            {
                _resolved = null;
                _isDirty = false;
                return null;
            }

            _resolved = baseData.Clone();

            // 캐릭터 CDR 적용.
            float cdr = _characterStats.GetFinal("CDR");
            if (cdr > 0f)
            {
                _resolved.Cooldown *= (1f - cdr);
                if (_resolved.Cooldown < 0.1f)
                {
                    _resolved.Cooldown = 0.1f;
                }
            }

            // 캐릭터 공격속도 적용.
            float attackSpeed = _characterStats.GetFinal("AttackSpeed");
            if (attackSpeed > 0f && attackSpeed != 1f)
            {
                _resolved.Cooldown /= attackSpeed;
                if (_resolved.Cooldown < 0.1f)
                {
                    _resolved.Cooldown = 0.1f;
                }
            }

            // 캐릭터 투사체 속도 보정.
            float projSpeedBonus = _characterStats.GetFinal("ProjectileSpeed");
            if (projSpeedBonus > 0f)
            {
                _resolved.ProjectileSpeed += projSpeedBonus;
            }

            // 캐릭터 영역 크기 보정.
            float areaSize = _characterStats.GetFinal("AreaSize");
            if (areaSize > 0f && areaSize != 1f)
            {
                _resolved.AreaSize *= areaSize;
            }

            // 캐릭터 크리티컬 보너스.
            float critChance = _characterStats.GetFinal("CritChance");
            _resolved.CritChanceBonus += critChance;

            float critDamage = _characterStats.GetFinal("CritDamage");
            _resolved.CritDamageBonus += critDamage;

            _isDirty = false;
            return _resolved;
        }
    }
}
