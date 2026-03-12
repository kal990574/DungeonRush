#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using DungeonRush.Stats.Config;
using DungeonRush.Stats.Data;
using DungeonRush.Stats.Data.SO;
using DungeonRush.Stats.Repository.Csv;

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

            var charRepo = new CsvCharacterRepository(charCsv.text);
            var skillRepo = new CsvSkillRepository(skillCsv.text, levelUpCsv.text);

            var characterSOs = ImportCharacters(charRepo.GetAll());
            var skillSOs = ImportSkills(skillRepo);

            UpdateDatabase(characterSOs, skillSOs);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[CsvToSoImporter] 임포트 완료: 캐릭터 {characterSOs.Length}개, 스킬 {skillSOs.Length}개.");
        }

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

        private static CharacterSpecSO[] ImportCharacters(IReadOnlyList<CharacterSpec> characters)
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

        private static SkillSpecSO[] ImportSkills(CsvSkillRepository skillRepo)
        {
            var result = new List<SkillSpecSO>();

            foreach (var spec in skillRepo.GetAll())
            {
                string assetPath = $"{SkillOutputPath}/{spec.Id}.asset";
                var so = AssetDatabase.LoadAssetAtPath<SkillSpecSO>(assetPath);

                if (so == null)
                {
                    so = ScriptableObject.CreateInstance<SkillSpecSO>();
                    so.SetFromSkillSpec(spec);
                    so.SetLevelUpEntries(new List<SkillLevelUpData>(skillRepo.GetLevelUps(spec.Id)));
                    AssetDatabase.CreateAsset(so, assetPath);
                }
                else
                {
                    so.SetFromSkillSpec(spec);
                    so.SetLevelUpEntries(new List<SkillLevelUpData>(skillRepo.GetLevelUps(spec.Id)));
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
