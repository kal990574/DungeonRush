using UnityEngine;

public class PlayerIdleState : IState
{
    private readonly PlayerController _player;

    public PlayerIdleState(PlayerController player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.AutoMove.Stop();
        _player.AnimHandler.PlayIdle();
    }

    public void Execute()
    {
        Transform target = _player.TargetFinder.FindNearestTarget();

        if (target == null) return;

        if (_player.Attack.IsInRange(target))
        {
            _player.StateMachine.ChangeState(_player.AttackState);
        }
        else
        {
            _player.StateMachine.ChangeState(_player.MoveState);
        }
    }

    public void Exit() { }
}