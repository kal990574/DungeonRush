using UnityEngine;

public class EnemyDeadState : IState
{
    private readonly EnemyController _enemy;
    private float _deathTimer;
    private bool _returned;
    private const float DeathDelay = 1f;

    public EnemyDeadState(EnemyController enemy)
    {
        _enemy = enemy;
    }

    public void Enter()
    {
        _deathTimer = 0f;
        _returned = false;
        _enemy.AutoMove.Stop();
        _enemy.AnimHandler.PlayDeath();
        _enemy.Collider.enabled = false;
    }

    public void Execute()
    {
        if (_returned) return;

        _deathTimer += Time.deltaTime;

        if (_deathTimer >= DeathDelay)
        {
            _returned = true;
            _enemy.ReturnToPool();
        }
    }

    public void Exit()
    {
        _deathTimer = 0f;
        _returned = false;
    }
}