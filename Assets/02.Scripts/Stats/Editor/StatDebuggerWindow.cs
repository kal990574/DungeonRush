#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DungeonRush.Stats.Data;
using DungeonRush.Stats.Data.SO;
using DungeonRush.Stats.Repository.SO;
using DungeonRush.Stats.Runtime;

namespace DungeonRush.Stats.Editor
{
    public class StatDebuggerWindow : EditorWindow
    {
        // Setup.
        private StatDatabaseSO _database;
        private StatDatabaseSO _prevDatabase;
        private GameData _gameData;
        private RunState _runState;

        // 캐릭터 목록.
        private string[] _characterIds;
        private string[] _characterNames;
        private int _selectedCharacterIndex;

        // 공격 스킬 목록.
        private string[] _attackSkillIds;
        private string[] _attackSkillNames;

        // 모디파이어 입력.
        private int _modStatIndex;
        private int _modTypeIndex;
        private float _modValue;
        private string _modSourceKey = "Debug:test";
        private readonly List<StatModifier> _addedModifiers = new();

        // 무기 슬롯.
        private const int WeaponSlotCount = 3;
        private readonly int[] _weaponSkillIndex = new int[WeaponSlotCount];

        // 스크롤 / Foldout.
        private Vector2 _scrollPos;
        private readonly Dictionary<string, bool> _statFoldouts = new();

        // 자동 리프레시.
        private double _lastRepaintTime;
        private const double RepaintInterval = 0.1;

        [MenuItem("Window/DungeonRush/Stat Debugger")]
        public static void ShowWindow()
        {
            GetWindow<StatDebuggerWindow>("Stat Debugger");
        }

        private void OnEnable()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            if (EditorApplication.timeSinceStartup - _lastRepaintTime < RepaintInterval)
            {
                return;
            }

            _lastRepaintTime = EditorApplication.timeSinceStartup;
            Repaint();
        }

        private void OnGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            DrawSetupSection();
            EditorGUILayout.Space(8);

            if (_runState != null && _runState.IsRunning)
            {
                DrawCharacterStatsSection();
                EditorGUILayout.Space(8);
                DrawModifierControlSection();
                EditorGUILayout.Space(8);
                DrawWeaponSlotSection();
            }

            EditorGUILayout.EndScrollView();
        }

        // ─────────────────────────────────────────
        // 1. Setup 섹션
        // ─────────────────────────────────────────
        private void DrawSetupSection()
        {
            EditorGUILayout.LabelField("Setup", EditorStyles.boldLabel);

            using (new EditorGUILayout.VerticalScope("box"))
            {
                _database = (StatDatabaseSO)EditorGUILayout.ObjectField(
                    "StatDatabase", _database, typeof(StatDatabaseSO), false);

                if (_database != _prevDatabase)
                {
                    _prevDatabase = _database;
                    RebuildFromDatabase();
                }

                if (_gameData == null || _characterNames == null)
                {
                    EditorGUILayout.HelpBox("StatDatabaseSO를 할당하세요.", MessageType.Info);
                    return;
                }

                _selectedCharacterIndex = EditorGUILayout.Popup(
                    "캐릭터", _selectedCharacterIndex, _characterNames);

                bool isRunning = _runState != null && _runState.IsRunning;

                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUI.DisabledScope(isRunning))
                    {
                        if (GUILayout.Button("Start Run"))
                        {
                            StartRun();
                        }
                    }

                    using (new EditorGUI.DisabledScope(!isRunning))
                    {
                        if (GUILayout.Button("End Run"))
                        {
                            EndRun();
                        }
                    }
                }

                var statusStyle = new GUIStyle(EditorStyles.label)
                {
                    fontStyle = FontStyle.Bold
                };
                statusStyle.normal.textColor = isRunning ? Color.green : Color.gray;
                EditorGUILayout.LabelField("상태", isRunning ? "실행 중" : "대기 중", statusStyle);
            }
        }

        // ─────────────────────────────────────────
        // 2. 캐릭터 스탯 섹션
        // ─────────────────────────────────────────
        private void DrawCharacterStatsSection()
        {
            EditorGUILayout.LabelField("캐릭터 스탯", EditorStyles.boldLabel);

            using (new EditorGUILayout.VerticalScope("box"))
            {
                // 헤더.
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("스탯", EditorStyles.boldLabel, GUILayout.Width(120));
                    EditorGUILayout.LabelField("기본값", EditorStyles.boldLabel, GUILayout.Width(80));
                    EditorGUILayout.LabelField("최종값", EditorStyles.boldLabel, GUILayout.Width(80));
                }

                foreach (string key in BaseStats.AllKeys)
                {
                    StatBreakdown breakdown = _runState.GetBreakdown(key);

                    if (!_statFoldouts.ContainsKey(key))
                    {
                        _statFoldouts[key] = false;
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        _statFoldouts[key] = EditorGUILayout.Foldout(
                            _statFoldouts[key], key, true);
                        GUILayout.Space(120 - EditorGUIUtility.labelWidth);
                        EditorGUILayout.LabelField(
                            breakdown.BaseValue.ToString("F2"), GUILayout.Width(80));

                        var finalStyle = new GUIStyle(EditorStyles.label);
                        if (breakdown.FinalValue > breakdown.BaseValue)
                        {
                            finalStyle.normal.textColor = new Color(0.2f, 0.8f, 0.2f);
                        }
                        else if (breakdown.FinalValue < breakdown.BaseValue)
                        {
                            finalStyle.normal.textColor = new Color(0.9f, 0.2f, 0.2f);
                        }

                        EditorGUILayout.LabelField(
                            breakdown.FinalValue.ToString("F2"), finalStyle, GUILayout.Width(80));
                    }

                    if (_statFoldouts[key] && breakdown.Entries != null && breakdown.Entries.Count > 0)
                    {
                        EditorGUI.indentLevel += 2;
                        foreach (var entry in breakdown.Entries)
                        {
                            EditorGUILayout.LabelField(
                                $"{entry.SourceKey}",
                                $"{entry.ModifyType} {entry.Value:+0.##;-0.##}");
                        }
                        EditorGUI.indentLevel -= 2;
                    }
                }
            }
        }

        // ─────────────────────────────────────────
        // 3. 모디파이어 컨트롤 섹션
        // ─────────────────────────────────────────
        private void DrawModifierControlSection()
        {
            EditorGUILayout.LabelField("모디파이어 컨트롤", EditorStyles.boldLabel);

            using (new EditorGUILayout.VerticalScope("box"))
            {
                // 입력 행.
                using (new EditorGUILayout.HorizontalScope())
                {
                    _modStatIndex = EditorGUILayout.Popup(_modStatIndex, BaseStats.AllKeys, GUILayout.Width(100));
                    _modTypeIndex = EditorGUILayout.Popup(_modTypeIndex, new[] { "Flat", "Percent" }, GUILayout.Width(70));
                    _modValue = EditorGUILayout.FloatField(_modValue, GUILayout.Width(60));
                    _modSourceKey = EditorGUILayout.TextField(_modSourceKey, GUILayout.Width(120));

                    if (GUILayout.Button("추가", GUILayout.Width(50)))
                    {
                        AddModifier();
                    }
                }

                EditorGUILayout.Space(4);

                // 활성 모디파이어 목록 (소스별 그룹핑).
                if (_addedModifiers.Count > 0)
                {
                    EditorGUILayout.LabelField("활성 모디파이어", EditorStyles.miniLabel);

                    var sourceGroups = new Dictionary<string, List<StatModifier>>();
                    foreach (var mod in _addedModifiers)
                    {
                        if (!sourceGroups.ContainsKey(mod.SourceKey))
                        {
                            sourceGroups[mod.SourceKey] = new List<StatModifier>();
                        }
                        sourceGroups[mod.SourceKey].Add(mod);
                    }

                    string sourceToRemove = null;
                    foreach (var kvp in sourceGroups)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField($"[{kvp.Key}]", EditorStyles.boldLabel, GUILayout.Width(140));
                            if (GUILayout.Button("제거", GUILayout.Width(50)))
                            {
                                sourceToRemove = kvp.Key;
                            }
                        }

                        EditorGUI.indentLevel++;
                        foreach (var mod in kvp.Value)
                        {
                            EditorGUILayout.LabelField(
                                $"  {mod.StatKey}: {mod.ModifyType} {mod.Value:+0.##;-0.##}");
                        }
                        EditorGUI.indentLevel--;
                    }

                    if (sourceToRemove != null)
                    {
                        RemoveModifiersBySource(sourceToRemove);
                    }
                }
            }
        }

        // ─────────────────────────────────────────
        // 4. 무기 슬롯 섹션
        // ─────────────────────────────────────────
        private void DrawWeaponSlotSection()
        {
            EditorGUILayout.LabelField("무기 슬롯", EditorStyles.boldLabel);

            for (int slot = 0; slot < WeaponSlotCount; slot++)
            {
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    var weapon = _runState.GetWeapon(slot);
                    bool isEquipped = weapon != null;

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField($"슬롯 {slot}", EditorStyles.boldLabel, GUILayout.Width(60));

                        _weaponSkillIndex[slot] = EditorGUILayout.Popup(
                            _weaponSkillIndex[slot], _attackSkillNames, GUILayout.Width(160));

                        using (new EditorGUI.DisabledScope(isEquipped))
                        {
                            if (GUILayout.Button("장착", GUILayout.Width(50)))
                            {
                                EquipWeapon(slot);
                            }
                        }

                        using (new EditorGUI.DisabledScope(!isEquipped))
                        {
                            if (GUILayout.Button("해제", GUILayout.Width(50)))
                            {
                                _runState.UnequipWeapon(slot);
                            }

                            if (GUILayout.Button("레벨업", GUILayout.Width(60)))
                            {
                                _runState.LevelUpWeapon(slot);
                            }
                        }
                    }

                    if (!isEquipped)
                    {
                        continue;
                    }

                    EditorGUILayout.LabelField($"스킬: {weapon.SkillId}  Lv.{weapon.CurrentLevel}");

                    var attackData = _runState.GetWeaponStats(slot);
                    if (attackData == null)
                    {
                        continue;
                    }

                    EditorGUI.indentLevel++;

                    // 15개 파라미터 표시.
                    foreach (string param in SkillAttackData.AllParams)
                    {
                        EditorGUILayout.LabelField(param, attackData.GetParam(param).ToString("F2"));
                    }

                    // 추가 필드.
                    EditorGUILayout.LabelField("CritEnabled", attackData.CritEnabled.ToString());
                    EditorGUILayout.LabelField("LifestealEnabled", attackData.LifestealEnabled.ToString());
                    EditorGUILayout.LabelField("FirePattern", attackData.FirePattern ?? "(none)");

                    EditorGUI.indentLevel--;
                }
            }
        }

        // ─────────────────────────────────────────
        // 내부 로직
        // ─────────────────────────────────────────
        private void RebuildFromDatabase()
        {
            _gameData = null;
            _runState = null;
            _characterIds = null;
            _characterNames = null;
            _attackSkillIds = null;
            _attackSkillNames = null;
            _addedModifiers.Clear();

            if (_database == null)
            {
                return;
            }

            var charRepo = new SoCharacterRepository(_database.Characters);
            var skillRepo = new SoSkillRepository(_database.Skills);
            _gameData = new GameData(charRepo, skillRepo);
            _runState = new RunState(_gameData);

            // 캐릭터 목록 캐싱.
            var characters = _gameData.GetAllCharacters();
            _characterIds = new string[characters.Count];
            _characterNames = new string[characters.Count];
            for (int i = 0; i < characters.Count; i++)
            {
                _characterIds[i] = characters[i].Id;
                _characterNames[i] = $"{characters[i].Name} ({characters[i].Id})";
            }

            // 공격 스킬 목록 캐싱.
            var skills = _gameData.GetAllSkills();
            var attackIds = new List<string>();
            var attackNames = new List<string>();
            foreach (var skill in skills)
            {
                if (skill.IsAttack)
                {
                    attackIds.Add(skill.Id);
                    attackNames.Add($"{skill.Name} ({skill.Id})");
                }
            }
            _attackSkillIds = attackIds.ToArray();
            _attackSkillNames = attackNames.ToArray();

            _selectedCharacterIndex = 0;
            for (int i = 0; i < WeaponSlotCount; i++)
            {
                _weaponSkillIndex[i] = 0;
            }
        }

        private void StartRun()
        {
            if (_characterIds == null || _characterIds.Length == 0)
            {
                return;
            }

            _addedModifiers.Clear();
            _statFoldouts.Clear();
            _runState.StartRun(_characterIds[_selectedCharacterIndex]);
        }

        private void EndRun()
        {
            _runState.EndRun();
            _addedModifiers.Clear();
            _statFoldouts.Clear();
        }

        private void AddModifier()
        {
            string statKey = BaseStats.AllKeys[_modStatIndex];
            var modType = _modTypeIndex == 0 ? ModifyType.Flat : ModifyType.Percent;
            var mod = new StatModifier(_modSourceKey, statKey, modType, _modValue);
            _runState.AddModifier(mod);
            _addedModifiers.Add(mod);
        }

        private void RemoveModifiersBySource(string sourceKey)
        {
            _runState.RemoveModifiersBySource(sourceKey);
            _addedModifiers.RemoveAll(m => m.SourceKey == sourceKey);
        }

        private void EquipWeapon(int slot)
        {
            if (_attackSkillIds == null || _attackSkillIds.Length == 0)
            {
                return;
            }

            string skillId = _attackSkillIds[_weaponSkillIndex[slot]];
            _runState.EquipWeapon(slot, skillId);
        }
    }
}
#endif
