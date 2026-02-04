using UnityEngine;

public class EnemyDeadState : IState
{
    private readonly EnemyController _enemy;

    public EnemyDeadState(EnemyController enemy)
    {
        _enemy = enemy;
    }

    public void Enter()
    {
        _enemy.AutoMove.Stop();
        _enemy.AnimHandler.PlayDeath();
        Object.Destroy(_enemy.gameObject, 1f);
    }

    public void Execute() { }

    public void Exit() { }
}