using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerStatData _statData;

    private StateMachine _stateMachine;
    private HealthComponent _health;
    private SPUMAnimationHandler _animHandler;
    private CharacterFlip _flip;
    private TargetFinder _targetFinder;
    private AutoMoveController _autoMove;
    private SkillSlotManager _skillSlotManager;

    private PlayerIdleState _idleState;
    private PlayerMoveState _moveState;
    private PlayerCombatState _combatState;
    private PlayerDeadState _deadState;

    public StateMachine StateMachine => _stateMachine;
    public SPUMAnimationHandler AnimHandler => _animHandler;
    public CharacterFlip Flip => _flip;
    public TargetFinder TargetFinder => _targetFinder;
    public AutoMoveController AutoMove => _autoMove;
    public SkillSlotManager SkillSlotManager => _skillSlotManager;

    public PlayerIdleState IdleState => _idleState;
    public PlayerMoveState MoveState => _moveState;
    public PlayerCombatState CombatState => _combatState;
    public PlayerDeadState DeadState => _deadState;

    private void Start()
    {
        CacheComponents();
        InitializeStats();
        InitializeStateMachine();
        InitializeSkills();

        _health.OnDied += HandleDeath;
        _stateMachine.ChangeState(_idleState);
    }

    private void Update()
    {
        _stateMachine.Execute();

        if (_health.IsAlive)
        {
            _skillSlotManager.Tick(Time.deltaTime);
        }
    }

    private void CacheComponents()
    {
        _health = GetComponent<HealthComponent>();
        _animHandler = GetComponent<SPUMAnimationHandler>();
        _flip = GetComponent<CharacterFlip>();
        _targetFinder = GetComponent<TargetFinder>();
        _autoMove = GetComponent<AutoMoveController>();
        _skillSlotManager = GetComponent<SkillSlotManager>();
    }

    private void InitializeStats()
    {
        _health.Initialize(_statData.maxHp);
        _targetFinder.Initialize(_statData.detectRange);
        _autoMove.Initialize(_statData.moveSpeed);
    }

    private void InitializeStateMachine()
    {
        _stateMachine = new StateMachine();
        _idleState = new PlayerIdleState(this);
        _moveState = new PlayerMoveState(this);
        _combatState = new PlayerCombatState(this);
        _deadState = new PlayerDeadState(this);
    }

    private void InitializeSkills()
    {
        _skillSlotManager.Initialize(
            transform,
            () => _targetFinder.FindNearestTarget()
        );
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