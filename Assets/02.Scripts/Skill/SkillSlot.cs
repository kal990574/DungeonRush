using UnityEngine;

// 개별 스킬 슬롯, 쿨다운 관리, 발사 조건 판단 담당
public class SkillSlot
{
    private readonly SkillData _data;
    private readonly SkillExecutor _executor;
    private float _cooldownTimer;

    public SkillData Data => _data;
    public bool IsReady => _cooldownTimer <= 0f;

    public SkillSlot(SkillData data, SkillExecutor executor)
    {
        _data = data;
        _executor = executor;
    }

    public void UpdateCooldown(float deltaTime)
    {
        if (_cooldownTimer > 0f)
        {
            _cooldownTimer -= deltaTime;
        }
    }

    public bool TryExecute(Transform owner, Transform target)
    {
        if (!IsReady) return false;
        if (target == null) return false;

        float distance = Vector2.Distance(owner.position, target.position);
        if (distance > _data.range) return false;

        _executor.Execute(owner, target);
        _cooldownTimer = _data.cooldown;
        return true;
    }
}