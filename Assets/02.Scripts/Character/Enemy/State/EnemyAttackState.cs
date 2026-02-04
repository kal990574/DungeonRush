using UnityEngine;

public class EnemyAttackState : IState
{
    private readonly EnemyController _enemy;

    public EnemyAttackState(EnemyController enemy)
    {
        _enemy = enemy;
    }

    public void Enter()
    {
        _enemy.AutoMove.Stop();
    }

    public void Execute()
    {
        Transform target = _enemy.PlayerTarget;
        if (target == null) return;

        if (!_enemy.Attack.IsInRange(target))
        {
            _enemy.StateMachine.ChangeState(_enemy.ChaseState);
            return;
        }

        Vector2 direction = (target.position - _enemy.transform.position).normalized;
        _enemy.Flip.FaceDirection(direction);

        if (_enemy.Attack.CanAttack())
        {
            _enemy.AnimHandler.PlayAttack();
            _enemy.Attack.TryAttack(target);
        }
    }

    public void Exit() { }
}