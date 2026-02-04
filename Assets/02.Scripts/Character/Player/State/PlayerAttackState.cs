using UnityEngine;

public class PlayerAttackState : IState
{
    private readonly PlayerController _player;

    public PlayerAttackState(PlayerController player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.AutoMove.Stop();
    }

    public void Execute()
    {
        Transform target = _player.TargetFinder.FindNearestTarget();

        if (target == null)
        {
            _player.StateMachine.ChangeState(_player.IdleState);
            return;
        }

        if (!_player.SkillExecutor.IsInRange(target))
        {
            _player.StateMachine.ChangeState(_player.MoveState);
            return;
        }

        Vector2 direction = (target.position - _player.transform.position).normalized;
        _player.Flip.FaceDirection(direction);

        if (_player.SkillExecutor.CanExecute())
        {
            _player.AnimHandler.PlayAttack();
            _player.SkillExecutor.TryExecute(target);
        }
    }

    public void Exit() { }
}