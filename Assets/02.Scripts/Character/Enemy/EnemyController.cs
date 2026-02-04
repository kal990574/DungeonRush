using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyStatData _statData;

    private StateMachine _stateMachine;
    private HealthComponent _health;
    private SPUMAnimationHandler _animHandler;
    private CharacterFlip _flip;
    private AutoMoveController _autoMove;
    private AttackController _attack;
    private Transform _playerTarget;

    private EnemyChaseState _chaseState;
    private EnemyAttackState _attackState;
    private EnemyDeadState _deadState;

    public StateMachine StateMachine => _stateMachine;
    public SPUMAnimationHandler AnimHandler => _animHandler;
    public CharacterFlip Flip => _flip;
    public AutoMoveController AutoMove => _autoMove;
    public AttackController Attack => _attack;
    public Transform PlayerTarget => _playerTarget;

    public EnemyChaseState ChaseState => _chaseState;
    public EnemyAttackState AttackState => _attackState;
    public EnemyDeadState DeadState => _deadState;

    private void Start()
    {
        CacheComponents();
        InitializeStats();
        FindPlayer();
        InitializeStateMachine();

        _health.OnDied += HandleDeath;
        _stateMachine.ChangeState(_chaseState);
    }

    private void Update()
    {
        _stateMachine.Execute();
    }

    private void CacheComponents()
    {
        _health = GetComponent<HealthComponent>();
        _animHandler = GetComponent<SPUMAnimationHandler>();
        _flip = GetComponent<CharacterFlip>();
        _autoMove = GetComponent<AutoMoveController>();
        _attack = GetComponent<AttackController>();
    }

    private void InitializeStats()
    {
        _health.Initialize(_statData.maxHp);
        _autoMove.Initialize(_statData.moveSpeed);
        _attack.Initialize(_statData.attackDamage, _statData.attackRange, _statData.attackCooldown);
    }

    private void FindPlayer()
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            _playerTarget = player.transform;
        }
    }

    private void InitializeStateMachine()
    {
        _stateMachine = new StateMachine();
        _chaseState = new EnemyChaseState(this);
        _attackState = new EnemyAttackState(this);
        _deadState = new EnemyDeadState(this);
    }

    private void HandleDeath()
    {
        _stateMachine.ChangeState(_deadState);
    }

    private void OnDestroy()
    {
        if (_health != null)
        {
            _health.OnDied -= HandleDeath;
        }
    }
}