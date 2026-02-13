using UnityEngine;

// 최소 사거리 이내에 적이 있을 때 정지, 스킬 발사는 SkillSlotManager
public class PlayerCombatState : IState
{
    private readonly PlayerController _player;

    public PlayerCombatState(PlayerController player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.AutoMove.Stop();
        _player.AnimHandler.PlayAttack();
    }

    public void Execute()
    {
        Transform target = _player.TargetFinder.FindNearestTarget();

        if (target == null)
        {
            _player.StateMachine.ChangeState(_player.IdleState);
            return;
        }

        float distance = Vector2.Distance(_player.transform.position, target.position);
        float shortestRange = _player.SkillSlotManager.GetShortestRange();

        if (distance > shortestRange)
        {
            _player.StateMachine.ChangeState(_player.MoveState);
            return;
        }

        Vector2 direction = (target.position - _player.transform.position).normalized;
        _player.Flip.FaceDirection(direction);
    }

    public void Exit() { }
}