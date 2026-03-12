#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using DungeonRush.Stats.Config;
using DungeonRush.Stats.Data;
using DungeonRush.Stats.Data.SO;
using ValueType = DungeonRush.Stats.Data.ValueType;

namespace DungeonRush.Stats.Editor
{
    public static class CsvToSoImporter
    {
        private const string CharacterOutputPath = "Assets/10.ScriptableObject/Stats/Characters";
        private const string SkillOutputPath = "Assets/10.ScriptableObject/Stats/Skills";
        private const string DatabaseOutputPath = "Assets/10.ScriptableObject/Stats";
        private const string DatabaseAssetName = "StatDatabase.asset";

        [MenuItem("DungeonRush/Stats/Import CSV → ScriptableObjects")]
        public static void ImportAll()
        {
            var charCsv = Resources.Load<TextAsset>(StatConfig.CharacterCsvPath);
            var skillCsv = Resources.Load<TextAsset>(StatConfig.SkillCsvPath);
            var levelUpCsv = Resources.Load<TextAsset>(StatConfig.SkillLevelUpCsvPath);

            if (charCsv == null || skillCsv == null || levelUpCsv == null)
            {
                Debug.LogError("[CsvToSoImporter] CSV 파일 누락. Resources/Data/ 경로를 확인하세요.");
                return;
            }

            EnsureDirectories();

            var characters = ParseCharacters(charCsv.text);
            var skills = ParseSkills(skillCsv.text);
            var levelUps = ParseLevelUps(levelUpCsv.text);

            var characterSOs = ImportCharacters(characters);
            var skillSOs = ImportSkills(skills, levelUps);

            UpdateDatabase(characterSOs, skillSOs);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[CsvToSoImporter] 임포트 완료: 캐릭터 {characterSOs.Length}개, 스킬 {skillSOs.Length}개.");
        }

        // ── CSV → 도메인 객체 파싱 ──────────────────────────────

        private static List<CharacterSpec> ParseCharacters(string csvText)
        {
            var rows = CsvParser.Parse(csvText);
            if (rows.Count < 2)
            {
                return new List<CharacterSpec>();
            }

            string[] headers = rows[0];
            int idxId = CsvParser.FindColumn(headers, "PlayerID");
            int idxName = CsvParser.FindColumn(headers, "PlayerName");
            int idxGrade = CsvParser.FindColumn(headers, "Grade");
            int idxModel = CsvParser.FindColumn(headers, "PlayerModelPrefab");
            int idxActiveSkill = CsvParser.FindColumn(headers, "StartActiveSkillID");
            int idxPassiveSkill = CsvParser.FindColumn(headers, "StartPassiveSkillID");
            int idxWeaponTag = CsvParser.FindColumn(headers, "WeaponTypeTag");
            int idxTrait = CsvParser.FindColumn(headers, "CharacterTraitID");

            var result = new List<CharacterSpec>();
            for (int r = 1; r < rows.Count; r++)
            {
                string[] cols = rows[r];
                string id = CsvParser.GetField(cols, idxId);
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }

                var stats = ParseBaseStats(headers, cols);
                var spec = new CharacterSpec(
                    id,
                    CsvParser.GetField(cols, idxName),
                    CsvParser.ParseInt(CsvParser.GetField(cols, idxGrade)),
                    CsvParser.GetField(cols, idxModel),
                    stats,
                    CsvParser.GetField(cols, idxActiveSkill),
                    CsvParser.GetField(cols, idxPassiveSkill),
                    CsvParser.GetField(cols, idxWeaponTag),
                    CsvParser.GetField(cols, idxTrait));

                result.Add(spec);
            }

            return result;
        }

        private static BaseStats ParseBaseStats(string[] headers, string[] cols)
        {
            var stats = new BaseStats();
            for (int c = 0; c < headers.Length && c < cols.Length; c++)
            {
                string header = headers[c].Trim();
                string value = cols[c].Trim();
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                float parsed = CsvParser.ParseFloat(value);
                if (parsed == 0f && value != "0")
                {
                    continue;
                }

                // Base 접두사 strip. (예: BaseATK → ATK)
                string statKey = header.StartsWith("Base", StringComparison.Ordinal)
                    ? header.Substring(4)
                    : header;

                if (stats.HasKey(statKey))
                {
                    stats.SetValue(statKey, parsed);
                }
            }

            return stats;
        }

        private static List<SkillSpec> ParseSkills(string csvText)
        {
            var rows = CsvParser.Parse(csvText);
            if (rows.Count < 2)
            {
                return new List<SkillSpec>();
            }

            string[] headers = rows[0];
            int idxId = CsvParser.FindColumn(headers, "SkillID");
            int idxName = CsvParser.FindColumn(headers, "SkillName");
            int idxType = CsvParser.FindColumn(headers, "SkillType");
            int idxSubType = CsvParser.FindColumn(headers, "SkillSubType");
            int idxMaxLevel = CsvParser.FindColumn(headers, "MaxLevel");
            int idxGroup = CsvParser.FindColumn(headers, "SkillGroup");
            int idxGrade = CsvParser.FindColumn(headers, "Grade");
            int idxRate = CsvParser.FindColumn(headers, "Rate");
            int idxLinked = CsvParser.FindColumn(headers, "LinkedEffectGroupID");
            int idxDesc = CsvParser.FindColumn(headers, "DescKey");
            int idxIcon = CsvParser.FindColumn(headers, "IconPath");
            int idxPrefab = CsvParser.FindColumn(headers, "PrefabPath");
            int idxCastVfx = CsvParser.FindColumn(headers, "CastVFXPath");
            int idxHitVfx = CsvParser.FindColumn(headers, "HitVFXPath");
            int idxSfx = CsvParser.FindColumn(headers, "SFXPath");
            int idxWeaponReq = CsvParser.FindColumn(headers, "WeaponTagReq");

            var result = new List<SkillSpec>();
            for (int r = 1; r < rows.Count; r++)
            {
                string[] cols = rows[r];
                string id = CsvParser.GetField(cols, idxId);
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }

                var skillType = CsvParser.ParseEnum(CsvParser.GetField(cols, idxType), ESkillType.ActiveAttack);

                var spec = new SkillSpec
                {
                    Id = id,
                    Name = CsvParser.GetField(cols, idxName),
                    ESkillType = skillType,
                    SubType = CsvParser.GetField(cols, idxSubType),
                    MaxLevel = CsvParser.ParseInt(CsvParser.GetField(cols, idxMaxLevel), 5),
                    SkillGroup = CsvParser.ParseInt(CsvParser.GetField(cols, idxGroup)),
                    Grade = CsvParser.GetField(cols, idxGrade),
                    Rate = CsvParser.ParseFloat(CsvParser.GetField(cols, idxRate)),
                    LinkedEffectGroupIds = CsvParser.ParseStringArray(CsvParser.GetField(cols, idxLinked), ';'),
                    DescKey = CsvParser.GetField(cols, idxDesc),
                    IconPath = CsvParser.GetField(cols, idxIcon),
                    PrefabPath = CsvParser.GetField(cols, idxPrefab),
                    CastVFXPath = CsvParser.GetField(cols, idxCastVfx),
                    HitVFXPath = CsvParser.GetField(cols, idxHitVfx),
                    SFXPath = CsvParser.GetField(cols, idxSfx),
                    WeaponTagReq = CsvParser.GetField(cols, idxWeaponReq)
                };

                if (skillType == ESkillType.ActiveAttack)
                {
                    spec.AttackData = ParseAttackData(headers, cols);
                }
                else
                {
                    spec.PassiveData = ParsePassiveData(headers, cols);
                }

                result.Add(spec);
            }

            return result;
        }

        private static SkillAttackData ParseAttackData(string[] headers, string[] cols)
        {
            var data = new SkillAttackData();

            // AllParams 순회로 수치 파라미터 자동 매핑.
            foreach (string param in SkillAttackData.AllParams)
            {
                int idx = CsvParser.FindColumn(headers, param);
                if (idx >= 0)
                {
                    data.SetParam(param, CsvParser.ParseFloat(CsvParser.GetField(cols, idx)));
                }
            }

            if (data.BaseProj == 0)
            {
                data.BaseProj = 1;
            }

            if (data.MaxStack == 0)
            {
                data.MaxStack = 1;
            }

            int critIdx = CsvParser.FindColumn(headers, "CritEnabled");
            if (critIdx >= 0)
            {
                data.CritEnabled = CsvParser.ParseBool(CsvParser.GetField(cols, critIdx));
            }

            int lsIdx = CsvParser.FindColumn(headers, "LifestealEnabled");
            if (lsIdx >= 0)
            {
                data.LifestealEnabled = CsvParser.ParseBool(CsvParser.GetField(cols, lsIdx));
            }

            int fpIdx = CsvParser.FindColumn(headers, "FirePattern");
            if (fpIdx >= 0)
            {
                data.FirePattern = CsvParser.GetField(cols, fpIdx);
            }

            return data;
        }

        private static SkillPassiveData ParsePassiveData(string[] headers, string[] cols)
        {
            var data = new SkillPassiveData();

            // AllParams 순회로 수치 파라미터 자동 매핑.
            foreach (string param in SkillPassiveData.AllParams)
            {
                int idx = CsvParser.FindColumn(headers, param);
                if (idx >= 0)
                {
                    data.SetParam(param, CsvParser.ParseFloat(CsvParser.GetField(cols, idx)));
                }
            }

            SetStringField(headers, cols, "PassiveEffectType", v => data.PassiveEffectType = v);
            SetStringField(headers, cols, "TargetStat", v => data.TargetStat = v);
            SetStringField(headers, cols, "ModifyType", v => data.ModifyType = v);
            SetStringField(headers, cols, "ApplyScope", v => data.ApplyScope = v);
            SetStringField(headers, cols, "ApplySkillTag", v => data.ApplySkillTag = v);
            SetStringField(headers, cols, "ApplySkillID", v => data.ApplySkillID = v);
            SetStringField(headers, cols, "TriggerType", v => data.TriggerType = v);
            SetStringField(headers, cols, "PassivePrefabPath", v => data.PassivePrefabPath = v);

            return data;
        }

        private static void SetStringField(string[] headers, string[] cols, string columnName, Action<string> setter)
        {
            int idx = CsvParser.FindColumn(headers, columnName);
            if (idx >= 0)
            {
                setter(CsvParser.GetField(cols, idx));
            }
        }

        private static Dictionary<string, List<SkillLevelUpData>> ParseLevelUps(string csvText)
        {
            var levelUps = new Dictionary<string, List<SkillLevelUpData>>();
            if (string.IsNullOrEmpty(csvText))
            {
                return levelUps;
            }

            var rows = CsvParser.Parse(csvText);
            if (rows.Count < 2)
            {
                return levelUps;
            }

            string[] headers = rows[0];
            int idxSkillId = CsvParser.FindColumn(headers, "SkillID");
            int idxLevel = CsvParser.FindColumn(headers, "SkillLevel");
            int idxParam = CsvParser.FindColumn(headers, "ParamName");
            int idxValueType = CsvParser.FindColumn(headers, "ValueType");
            int idxValue = CsvParser.FindColumn(headers, "ParamValue");
            int idxCondType = CsvParser.FindColumn(headers, "ConditionType");
            int idxCondValue = CsvParser.FindColumn(headers, "ConditionValue");
            int idxUITextKey = CsvParser.FindColumn(headers, "UITextKey");

            for (int r = 1; r < rows.Count; r++)
            {
                string[] cols = rows[r];
                string skillId = CsvParser.GetField(cols, idxSkillId);
                if (string.IsNullOrEmpty(skillId))
                {
                    continue;
                }

                int level = CsvParser.ParseInt(CsvParser.GetField(cols, idxLevel), 1);
                string paramName = CsvParser.GetField(cols, idxParam);
                var valueType = CsvParser.ParseEnum(CsvParser.GetField(cols, idxValueType), ValueType.Add);
                float paramValue = CsvParser.ParseFloat(CsvParser.GetField(cols, idxValue));
                string condType = CsvParser.GetField(cols, idxCondType);
                string condValue = CsvParser.GetField(cols, idxCondValue);
                string uiTextKey = CsvParser.GetField(cols, idxUITextKey);

                var mod = new ParamModification(
                    paramName, valueType, paramValue,
                    string.IsNullOrEmpty(condType) ? null : condType,
                    string.IsNullOrEmpty(condValue) ? null : condValue);

                if (!levelUps.TryGetValue(skillId, out var list))
                {
                    list = new List<SkillLevelUpData>();
                    levelUps[skillId] = list;
                }

                var existing = list.Find(x => x.SkillLevel == level);
                if (existing != null)
                {
                    existing.Modifications.Add(mod);
                }
                else
                {
                    list.Add(new SkillLevelUpData(
                        skillId, level, new List<ParamModification> { mod },
                        string.IsNullOrEmpty(uiTextKey) ? null : uiTextKey));
                }
            }

            // 레벨 순 정렬.
            foreach (var list in levelUps.Values)
            {
                list.Sort((a, b) => a.SkillLevel.CompareTo(b.SkillLevel));
            }

            return levelUps;
        }

        // ── SO 임포트 ──────────────────────────────────────────

        private static void EnsureDirectories()
        {
            EnsureDirectory(CharacterOutputPath);
            EnsureDirectory(SkillOutputPath);
            EnsureDirectory(DatabaseOutputPath);
        }

        private static void EnsureDirectory(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = Path.GetDirectoryName(path).Replace('\\', '/');
                string folderName = Path.GetFileName(path);

                if (!AssetDatabase.IsValidFolder(parent))
                {
                    EnsureDirectory(parent);
                }

                AssetDatabase.CreateFolder(parent, folderName);
            }
        }

        private static CharacterSpecSO[] ImportCharacters(List<CharacterSpec> characters)
        {
            var result = new List<CharacterSpecSO>();

            foreach (var spec in characters)
            {
                string assetPath = $"{CharacterOutputPath}/{spec.Id}.asset";
                var so = AssetDatabase.LoadAssetAtPath<CharacterSpecSO>(assetPath);

                if (so == null)
                {
                    so = ScriptableObject.CreateInstance<CharacterSpecSO>();
                    so.SetFromCharacterSpec(spec);
                    AssetDatabase.CreateAsset(so, assetPath);
                }
                else
                {
                    so.SetFromCharacterSpec(spec);
                    EditorUtility.SetDirty(so);
                }

                result.Add(so);
            }

            return result.ToArray();
        }

        private static SkillSpecSO[] ImportSkills(
            List<SkillSpec> skills,
            Dictionary<string, List<SkillLevelUpData>> levelUps)
        {
            var result = new List<SkillSpecSO>();

            foreach (var spec in skills)
            {
                string assetPath = $"{SkillOutputPath}/{spec.Id}.asset";
                var so = AssetDatabase.LoadAssetAtPath<SkillSpecSO>(assetPath);

                levelUps.TryGetValue(spec.Id, out var skillLevelUps);
                var levelUpList = skillLevelUps ?? new List<SkillLevelUpData>();

                if (so == null)
                {
                    so = ScriptableObject.CreateInstance<SkillSpecSO>();
                    so.SetFromSkillSpec(spec);
                    so.SetLevelUpEntries(levelUpList);
                    AssetDatabase.CreateAsset(so, assetPath);
                }
                else
                {
                    so.SetFromSkillSpec(spec);
                    so.SetLevelUpEntries(levelUpList);
                    EditorUtility.SetDirty(so);
                }

                result.Add(so);
            }

            return result.ToArray();
        }

        private static void UpdateDatabase(CharacterSpecSO[] characters, SkillSpecSO[] skills)
        {
            string assetPath = $"{DatabaseOutputPath}/{DatabaseAssetName}";
            var database = AssetDatabase.LoadAssetAtPath<StatDatabaseSO>(assetPath);

            if (database == null)
            {
                database = ScriptableObject.CreateInstance<StatDatabaseSO>();
                database.SetCharacters(characters);
                database.SetSkills(skills);
                AssetDatabase.CreateAsset(database, assetPath);
            }
            else
            {
                database.SetCharacters(characters);
                database.SetSkills(skills);
                EditorUtility.SetDirty(database);
            }
        }
    }
}
#endif
