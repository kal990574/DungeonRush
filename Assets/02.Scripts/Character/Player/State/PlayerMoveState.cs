using UnityEngine;

public class PlayerMoveState : IState
{
    private readonly PlayerController _player;

    public PlayerMoveState(PlayerController player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.AnimHandler.PlayMove();
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

        if (distance <= shortestRange)
        {
            _player.StateMachine.ChangeState(_player.CombatState);
            return;
        }

        Vector2 direction = (target.position - _player.transform.position).normalized;
        _player.Flip.FaceDirection(direction);
        _player.AutoMove.MoveTo(target.position);
    }

    public void Exit()
    {
        _player.AutoMove.Stop();
    }
}