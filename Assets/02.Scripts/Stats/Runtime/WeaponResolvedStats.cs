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
                _resolved.BaseCoolTime *= (1f - cdr);
                if (_resolved.BaseCoolTime < 0.1f)
                {
                    _resolved.BaseCoolTime = 0.1f;
                }
            }

            // 캐릭터 크리티컬 보정.
            float critRate = _characterStats.GetFinal("CritRate");
            float critDamage = _characterStats.GetFinal("CritDamage");

            // 전역 투사체 수 보너스.
            float projBonus = _characterStats.GetFinal("BaseProj");
            if (projBonus > 0f)
            {
                _resolved.BaseProj += (int)projBonus;
            }

            // 전역 관통 보너스.
            float pierceBonus = _characterStats.GetFinal("PierceCount");
            if (pierceBonus > 0f)
            {
                _resolved.PierceCount += (int)pierceBonus;
            }

            // 전역 넉백 보너스.
            float knockbackBonus = _characterStats.GetFinal("BaseKnockback");
            if (knockbackBonus > 0f)
            {
                _resolved.BaseKnockback += knockbackBonus;
            }

            // 전역 바운스 보너스.
            float bounceBonus = _characterStats.GetFinal("BounceCount");
            if (bounceBonus > 0f)
            {
                _resolved.BounceCount += (int)bounceBonus;
            }

            _isDirty = false;
            return _resolved;
        }
    }
}
