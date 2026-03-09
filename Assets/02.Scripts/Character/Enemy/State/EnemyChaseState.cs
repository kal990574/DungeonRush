using UnityEngine;

public class EnemyChaseState : IState
{
    private readonly EnemyController _enemy;

    public EnemyChaseState(EnemyController enemy)
    {
        _enemy = enemy;
    }

    public void Enter()
    {
        _enemy.AnimHandler.PlayMove();
    }

    public void Execute()
    {
        Transform target = _enemy.PlayerTarget;
        if (target == null) return;

        if (_enemy.Attack.IsInRange(target))
        {
            _enemy.StateMachine.ChangeState(_enemy.AttackState);
            return;
        }

        Vector2 direction = (target.position - _enemy.transform.position).normalized;
        _enemy.Flip.FaceDirection(direction);
        _enemy.AutoMove.MoveTo(target.position);
    }

    public void Exit()
    {
        _enemy.AutoMove.Stop();
    }
}