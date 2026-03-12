using UnityEngine;
using DungeonRush.Stats.Data;
using DungeonRush.Stats.Data.SO;
using DungeonRush.Stats.Repository;
using DungeonRush.Stats.Repository.SO;
using DungeonRush.Stats.Runtime;

namespace DungeonRush.Stats
{
    // Play Mode 검증용 MonoBehaviour.
    // 빈 GameObject에 붙여서 Play하면 Console에 결과 출력.
    public class StatSystemVerification : MonoBehaviour
    {
        [Header("SO 기반 로드 (필수 할당)")]
        [SerializeField] private StatDatabaseSO _statDatabase;

        private int _pass;
        private int _fail;

        private void Awake()
        {
            RunAllTests();
        }

        private void RunAllTests()
        {
            _pass = 0;
            _fail = 0;

            var gameData = CreateGameData();
            if (gameData == null)
            {
                return;
            }

            // 2. 캐릭터 조회.
            var wizard = gameData.GetCharacter("Player_001");
            Assert("캐릭터 조회 - not null", wizard != null);
            Assert("캐릭터 ATK", wizard.Stats.ATK == 12f);
            Assert("캐릭터 HP", wizard.Stats.HP == 100f);
            Assert("캐릭터 Evasion", wizard.Stats.Evasion == 0f);
            Assert("캐릭터 Lifesteal", wizard.Stats.Lifesteal == 0f);
            Assert("캐릭터 StartActiveSkillId", wizard.StartActiveSkillId == "SK_A_001");
            Assert("캐릭터 Grade", wizard.Grade == 0);

            var warrior = gameData.GetCharacter("Player_004");
            Assert("검사 HP", warrior.Stats.HP == 150f);
            Assert("검사 Armor", Mathf.Approximately(warrior.Stats.Armor, 0.1f));

            // 3. 스킬 조회 (AttackData).
            var fireball = gameData.GetSkill("SK_A_001");
            Assert("스킬 조회 - not null", fireball != null);
            Assert("스킬 IsAttack", fireball.IsAttack);
            Assert("스킬 SkillCoef", Mathf.Approximately(fireball.AttackData.SkillCoef, 1.2f));
            Assert("스킬 BaseCoolTime", Mathf.Approximately(fireball.AttackData.BaseCoolTime, 1.5f));
            Assert("스킬 BaseProj", fireball.AttackData.BaseProj == 1);
            Assert("스킬 IconPath", fireball.IconPath == "Icons/Skill/fireball");
            Assert("스킬 CritEnabled", fireball.AttackData.CritEnabled);
            Assert("스킬 LifestealEnabled", fireball.AttackData.LifestealEnabled);

            // 4. 패시브 조회.
            var powerRune = gameData.GetSkill("SK_P_001");
            Assert("패시브 조회 - not null", powerRune != null);
            Assert("패시브 IsPassive", powerRune.IsPassive);
            Assert("패시브 TargetStat", powerRune.PassiveData.TargetStat == "ATK");
            Assert("패시브 ModifyType", powerRune.PassiveData.ModifyType == "Mult");
            Assert("패시브 ModifyValue", Mathf.Approximately(powerRune.PassiveData.ModifyValue, 0.08f));
            Assert("패시브 PassiveEffectType", powerRune.PassiveData.PassiveEffectType == "StatModify");

            // 5. 레벨업 누적.
            float coefLv3 = gameData.GetSkillParam("SK_A_001", "SkillCoef", 3);
            Assert("레벨업 누적 SkillCoef Lv3", Mathf.Approximately(coefLv3, 1.55f));

            float coolLv2 = gameData.GetSkillParam("SK_A_001", "BaseCoolTime", 2);
            Assert("레벨업 누적 BaseCoolTime Lv2", Mathf.Approximately(coolLv2, 1.4f));

            // 6. RunState 기본 동작.
            var runState = new RunState(gameData);
            runState.StartRun("Player_001");
            Assert("RunState ATK", runState.GetStat("ATK") == 12f);
            Assert("RunState HP", runState.GetStat("HP") == 100f);
            Assert("RunState IsRunning", runState.IsRunning);

            // 7. 무기 장착 + 스펙 조회.
            runState.EquipWeapon(0, "SK_A_001");
            var wpnStats = runState.GetWeaponStats(0);
            Assert("무기 장착 - not null", wpnStats != null);
            Assert("무기 SkillCoef", Mathf.Approximately(wpnStats.SkillCoef, 1.2f));

            // 8. 무기 레벨업.
            runState.LevelUpWeapon(0);
            wpnStats = runState.GetWeaponStats(0);
            Assert("무기 레벨업 SkillCoef Lv2", Mathf.Approximately(wpnStats.SkillCoef, 1.35f));

            // 9. 룬 modifier + 스탯 합산.
            runState.AddModifier(new StatModifier("Rune:힘의룬_Lv1", "ATK", ModifyType.Percent, 0.08f));
            float expectedAtk = 12f * 1.08f;
            Assert("룬 적용 ATK", Mathf.Approximately(runState.GetStat("ATK"), expectedAtk));

            // 10. 분해 조회.
            var breakdown = runState.GetBreakdown("ATK");
            Assert("분해 BaseValue", breakdown.BaseValue == 12f);
            Assert("분해 Entries", breakdown.Entries.Count >= 1);

            // 11. 전역 보너스 (투사체 수).
            runState.AddModifier(new StatModifier("Rune:투사체의룬", "BaseProj", ModifyType.Flat, 1f));
            wpnStats = runState.GetWeaponStats(0);
            Assert("전역 투사체 보너스", wpnStats.BaseProj >= 2);

            // 12. 런 종료.
            runState.EndRun();
            Assert("런 종료", !runState.IsRunning);

            Debug.Log($"[StatSystem] 검증 완료 (SO): {_pass} PASS, {_fail} FAIL");
        }

        private GameData CreateGameData()
        {
            if (_statDatabase == null)
            {
                Debug.LogError("[StatSystem] StatDatabaseSO가 할당되지 않았습니다. Inspector에서 할당하세요.");
                return null;
            }

            ICharacterRepository charRepo = new SoCharacterRepository(_statDatabase.Characters);
            ISkillRepository skillRepo = new SoSkillRepository(_statDatabase.Skills);

            return new GameData(charRepo, skillRepo);
        }

        private void Assert(string testName, bool condition)
        {
            if (condition)
            {
                _pass++;
                Debug.Log($"[StatSystem] PASS: {testName}");
            }
            else
            {
                _fail++;
                Debug.LogError($"[StatSystem] FAIL: {testName}");
            }
        }
    }
}
