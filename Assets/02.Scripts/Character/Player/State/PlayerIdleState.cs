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

        float distance = Vector2.Distance(_player.transform.position, target.position);
        float shortestRange = _player.SkillSlotManager.GetShortestRange();

        if (distance <= shortestRange)
        {
            _player.StateMachine.ChangeState(_player.CombatState);
        }
        else
        {
            _player.StateMachine.ChangeState(_player.MoveState);
        }
    }

    public void Exit() { }
}