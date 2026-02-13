using System;
using System.Collections.Generic;
using UnityEngine;

// 플레이어의 스킬 슬롯 3개를 관리, FSM과 독립적으로 자동 발사
public class SkillSlotManager : MonoBehaviour
{
    private const int MaxSlots = 3;

    [SerializeField] private SkillData[] _initialSkills;

    private readonly List<SkillSlot> _slots = new();
    private Transform _owner;
    private Func<Transform> _targetProvider;
    private Func<Transform, Transform> _retargetFunc;

    public int SlotCount => _slots.Count;

    public void Initialize(Transform owner, Func<Transform> targetProvider, Func<Transform, Transform> retargetFunc)
    {
        _owner = owner;
        _targetProvider = targetProvider;
        _retargetFunc = retargetFunc;

        foreach (SkillData data in _initialSkills)
        {
            if (data != null)
            {
                AddSkill(data);
            }
        }
    }

    public bool AddSkill(SkillData data)
    {
        if (_slots.Count >= MaxSlots) return false;

        var executor = new SkillExecutor(data, _retargetFunc);
        _slots.Add(new SkillSlot(data, executor));
        return true;
    }

    public void Tick(float deltaTime)
    {
        Transform target = _targetProvider?.Invoke();

        for (int i = 0; i < _slots.Count; i++)
        {
            _slots[i].UpdateCooldown(deltaTime);
            _slots[i].TryExecute(_owner, target);
        }
    }

    // 장착된 스킬 중 최소 사거리를 반환. Combat 상태 전환 기준.
    public float GetShortestRange()
    {
        float min = float.MaxValue;

        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].Data.range < min)
            {
                min = _slots[i].Data.range;
            }
        }

        return min;
    }
}