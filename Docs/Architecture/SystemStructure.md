# DungeonRush 시스템 구조 상세 설계

> **Architect 관점**: 확장성, 유지보수성, 테스트 가능성을 고려한 시스템 설계

---

## 1. 아키텍처 패턴 선택

### 1.1 적용 패턴

| 패턴 | 적용 위치 | 목적 |
|------|----------|------|
| **Singleton** | GameManager, AudioManager | 전역 상태 관리 |
| **Observer (Event Bus)** | GameEventBus | 시스템 간 느슨한 결합 |
| **Object Pool** | 적, 투사체, 이펙트 | GC 최소화, 성능 최적화 |
| **State Machine** | GameState, CharacterState | 상태 전환 명확화 |
| **Strategy** | SkillBase 하위 클래스 | 스킬 행동 다형성 |
| **Factory** | EnemyFactory, ProjectileFactory | 객체 생성 캡슐화 |
| **Mediator** | BattleMediator | 전투 시스템 조율 |

### 1.2 패턴 적용 이유

```
┌─────────────────────────────────────────────────────────────────┐
│                    WHY THESE PATTERNS?                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Singleton (GameManager)                                        │
│  └─ 게임 전역에서 단일 진입점 필요                                │
│  └─ 상태 일관성 보장                                            │
│                                                                 │
│  Event Bus (GameEventBus)                                       │
│  └─ 시스템 간 결합도 낮춤                                        │
│  └─ 새로운 시스템 추가 시 기존 코드 수정 불필요                    │
│  └─ 단위 테스트 용이                                            │
│                                                                 │
│  Object Pool                                                    │
│  └─ 모바일 환경에서 GC 스파이크 방지                              │
│  └─ Instantiate/Destroy 비용 절감                               │
│                                                                 │
│  Strategy (Skills)                                              │
│  └─ 새 스킬 추가 시 기존 코드 변경 없음 (OCP)                     │
│  └─ 스킬 조합/교체 유연성                                        │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## 2. 계층별 상세 구조

### 2.1 Infrastructure Layer (인프라 계층)

```
┌─────────────────────────────────────────────────────────────────┐
│                    INFRASTRUCTURE LAYER                          │
│                                                                  │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │                     Singleton<T>                            │ │
│  │  ┌──────────────┐                                          │ │
│  │  │ + Instance   │◀─────────────────────────────────┐       │ │
│  │  │ + Awake()    │                                  │       │ │
│  │  └──────────────┘                                  │       │ │
│  └────────────────────────────────────────────────────│───────┘ │
│                                                       │         │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐│         │
│  │ GameManager  │  │ AudioManager │  │ SaveManager  ││         │
│  │ : Singleton  │  │ : Singleton  │  │ : Singleton  ││         │
│  └──────┬───────┘  └──────────────┘  └──────────────┘│         │
│         │                                             │         │
│         │ owns                                        │ inherits│
│         ▼                                             │         │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐│         │
│  │ GameConfig   │  │ ObjectPool   │  │CoroutineRunner│         │
│  │    (SO)      │  │   <T>        │  │              ││         │
│  └──────────────┘  └──────────────┘  └──────────────┘│         │
│                                                       │         │
└───────────────────────────────────────────────────────┴─────────┘
```

#### Singleton 구현 명세

```csharp
// 제네릭 싱글톤 기반 클래스
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T s_instance;
    private static readonly object s_lock = new object();
    private static bool s_applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (s_applicationIsQuitting)
            {
                return null;
            }

            lock (s_lock)
            {
                if (s_instance == null)
                {
                    s_instance = FindObjectOfType<T>();

                    if (s_instance == null)
                    {
                        var singletonObject = new GameObject();
                        s_instance = singletonObject.AddComponent<T>();
                        singletonObject.name = $"[Singleton] {typeof(T)}";
                    }
                }
                return s_instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (s_instance != this)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnApplicationQuit()
    {
        s_applicationIsQuitting = true;
    }
}
```

#### ObjectPool 구현 명세

```csharp
public class ObjectPool<T> where T : Component, IPoolable
{
    private readonly Queue<T> _pool = new Queue<T>();
    private readonly T _prefab;
    private readonly Transform _parent;
    private readonly int _maxSize;

    public int ActiveCount { get; private set; }
    public int PooledCount => _pool.Count;

    public ObjectPool(T prefab, Transform parent, int initialSize, int maxSize)
    {
        _prefab = prefab;
        _parent = parent;
        _maxSize = maxSize;

        Prewarm(initialSize);
    }

    public void Prewarm(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var obj = CreateNew();
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    public T Get()
    {
        T obj;

        if (_pool.Count > 0)
        {
            obj = _pool.Dequeue();
        }
        else
        {
            obj = CreateNew();
        }

        obj.gameObject.SetActive(true);
        obj.OnSpawn();
        ActiveCount++;
        return obj;
    }

    public void Return(T obj)
    {
        if (_pool.Count >= _maxSize)
        {
            Object.Destroy(obj.gameObject);
        }
        else
        {
            obj.OnDespawn();
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
        ActiveCount--;
    }

    private T CreateNew()
    {
        return Object.Instantiate(_prefab, _parent);
    }
}
```

---

### 2.2 Game Logic Layer (게임 로직 계층)

```
┌─────────────────────────────────────────────────────────────────┐
│                      GAME LOGIC LAYER                            │
│                                                                  │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │                    GameEventBus (Static)                    │ │
│  │  ┌─────────────────────────────────────────────────────┐   │ │
│  │  │ Game Events    │ Stage Events   │ Combat Events     │   │ │
│  │  │ ─────────────  │ ─────────────  │ ──────────────    │   │ │
│  │  │ OnGameStart    │ OnWaveStart    │ OnDamageDealt     │   │ │
│  │  │ OnGamePause    │ OnWaveComplete │ OnEnemyKilled     │   │ │
│  │  │ OnGameOver     │ OnChapterClear │ OnBossKilled      │   │ │
│  │  └─────────────────────────────────────────────────────┘   │ │
│  │  ┌─────────────────────────────────────────────────────┐   │ │
│  │  │ Player Events  │ Skill Events   │ Upgrade Events    │   │ │
│  │  │ ─────────────  │ ─────────────  │ ──────────────    │   │ │
│  │  │ OnHPChanged    │ OnSkillUsed    │ OnLevelUp         │   │ │
│  │  │ OnXPChanged    │ OnCooldownTick │ OnCardSelected    │   │ │
│  │  │ OnPlayerDeath  │ OnSkillEquip   │ OnRerollUsed      │   │ │
│  │  └─────────────────────────────────────────────────────┘   │ │
│  └────────────────────────────────────────────────────────────┘ │
│                              │                                   │
│           ┌──────────────────┼──────────────────┐               │
│           ▼                  ▼                  ▼               │
│  ┌──────────────┐   ┌──────────────┐   ┌──────────────┐        │
│  │ StageManager │   │BattleMediator│   │UpgradeManager│        │
│  └──────┬───────┘   └──────┬───────┘   └──────┬───────┘        │
│         │                  │                  │                 │
│         ▼                  ▼                  ▼                 │
│  ┌──────────────┐   ┌──────────────┐   ┌──────────────┐        │
│  │WaveController│   │AutoBattleCtrl│   │LevelUpManager│        │
│  │ SpawnManager │   │DamageCalculator│ │CardSelectSys │        │
│  │              │   │SkillExecutor │   │ RerollSystem │        │
│  └──────────────┘   └──────────────┘   └──────────────┘        │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

#### BattleMediator 구현 명세 (신규)

```csharp
// 전투 시스템 조율자 - 복잡한 전투 상호작용 관리
public class BattleMediator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _player;
    [SerializeField] private AutoBattleController _autoBattle;
    [SerializeField] private SkillExecutor _skillExecutor;
    [SerializeField] private ProjectileManager _projectileManager;

    private readonly List<EnemyController> _activeEnemies = new List<EnemyController>();

    public IReadOnlyList<EnemyController> ActiveEnemies => _activeEnemies;
    public PlayerController Player => _player;

    private void OnEnable()
    {
        GameEventBus.OnEnemySpawned += RegisterEnemy;
        GameEventBus.OnEnemyKilled += UnregisterEnemy;
        GameEventBus.OnGameStateChanged += HandleStateChange;
    }

    private void OnDisable()
    {
        GameEventBus.OnEnemySpawned -= RegisterEnemy;
        GameEventBus.OnEnemyKilled -= UnregisterEnemy;
        GameEventBus.OnGameStateChanged -= HandleStateChange;
    }

    // 가장 가까운 적 찾기
    public EnemyController FindNearestEnemy(Vector3 position)
    {
        EnemyController nearest = null;
        float minDistance = float.MaxValue;

        foreach (var enemy in _activeEnemies)
        {
            if (enemy.IsDead) continue;

            float distance = Vector3.Distance(position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }

        return nearest;
    }

    // 범위 내 적 찾기
    public List<EnemyController> FindEnemiesInRange(Vector3 center, float radius)
    {
        var result = new List<EnemyController>();

        foreach (var enemy in _activeEnemies)
        {
            if (enemy.IsDead) continue;

            if (Vector3.Distance(center, enemy.transform.position) <= radius)
            {
                result.Add(enemy);
            }
        }

        return result;
    }

    // 스킬 실행 요청
    public void RequestSkillExecution(SkillBase skill, IDamageable target)
    {
        _skillExecutor.Execute(skill, _player.Stats, target);
    }

    // 투사체 발사 요청
    public void RequestProjectile(ProjectileData data, Vector3 origin, Vector3 direction)
    {
        _projectileManager.Fire(data, origin, direction);
    }

    private void RegisterEnemy(EnemyController enemy)
    {
        _activeEnemies.Add(enemy);
    }

    private void UnregisterEnemy(EnemyController enemy, int xp)
    {
        _activeEnemies.Remove(enemy);
    }

    private void HandleStateChange(GameState newState)
    {
        _autoBattle.SetActive(newState == GameState.Playing);
    }
}
```

---

### 2.3 Entity Layer (엔티티 계층)

```
┌─────────────────────────────────────────────────────────────────┐
│                        ENTITY LAYER                              │
│                                                                  │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │              <<interface>> IDamageable                      │ │
│  │  ┌─────────────────────────────────────────────────────┐   │ │
│  │  │ + CurrentHP: float                                   │   │ │
│  │  │ + MaxHP: float                                       │   │ │
│  │  │ + IsDead: bool                                       │   │ │
│  │  │ + TakeDamage(damage, type): void                     │   │ │
│  │  │ + Heal(amount): void                                 │   │ │
│  │  └─────────────────────────────────────────────────────┘   │ │
│  └──────────────────────────┬─────────────────────────────────┘ │
│                             │ implements                         │
│         ┌───────────────────┼───────────────────┐               │
│         ▼                   ▼                   ▼               │
│  ┌──────────────┐   ┌──────────────┐   ┌──────────────┐        │
│  │CharacterBase │   │              │   │              │        │
│  │  (abstract)  │   │  Projectile  │   │  Destructible│        │
│  └──────┬───────┘   └──────────────┘   └──────────────┘        │
│         │                                                        │
│         │ extends                                                │
│    ┌────┴────┐                                                  │
│    ▼         ▼                                                  │
│ ┌──────┐ ┌──────────┐                                          │
│ │Player│ │EnemyBase │                                          │
│ │Ctrl  │ │(abstract)│                                          │
│ └──────┘ └────┬─────┘                                          │
│               │ extends                                         │
│          ┌────┴────┐                                           │
│          ▼         ▼                                           │
│    ┌──────────┐ ┌──────────┐                                   │
│    │  Enemy   │ │   Boss   │                                   │
│    │Controller│ │Controller│                                   │
│    └──────────┘ └──────────┘                                   │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

#### CharacterBase 구현 명세

```csharp
public abstract class CharacterBase : MonoBehaviour, IDamageable, IAttacker
{
    [Header("Stats")]
    [SerializeField] protected CharacterStats _stats;

    [Header("Components")]
    protected SpriteRenderer _spriteRenderer;
    protected Animator _animator;
    protected Rigidbody2D _rigidbody;

    // IDamageable
    public float CurrentHP => _stats.currentHP;
    public float MaxHP => _stats.maxHP;
    public bool IsDead => _stats.IsDead;

    // IAttacker
    public float AttackPower => _stats.TotalATK;
    public float AttackRange => _attackRange;

    [SerializeField] protected float _attackRange = 1.5f;

    // State
    protected CharacterState _currentState = CharacterState.Idle;
    public CharacterState CurrentState => _currentState;

    protected virtual void Awake()
    {
        CacheComponents();
    }

    protected virtual void CacheComponents()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public virtual void TakeDamage(float damage, DamageType type)
    {
        if (IsDead) return;

        float actualDamage = CalculateActualDamage(damage, type);
        _stats.currentHP = Mathf.Max(0, _stats.currentHP - actualDamage);

        OnDamageTaken(actualDamage, type);

        if (IsDead)
        {
            OnDeath();
        }
    }

    protected virtual float CalculateActualDamage(float damage, DamageType type)
    {
        if (type == DamageType.True)
        {
            return damage;
        }

        return Mathf.Max(1f, damage - _stats.TotalDEF);
    }

    public virtual void Heal(float amount)
    {
        if (IsDead) return;

        _stats.currentHP = Mathf.Min(_stats.maxHP, _stats.currentHP + amount);
        OnHealed(amount);
    }

    public abstract void Attack(IDamageable target);

    protected abstract void OnDamageTaken(float damage, DamageType type);
    protected abstract void OnHealed(float amount);
    protected abstract void OnDeath();

    protected void SetState(CharacterState newState)
    {
        if (_currentState == newState) return;

        var oldState = _currentState;
        _currentState = newState;
        OnStateChanged(oldState, newState);
    }

    protected virtual void OnStateChanged(CharacterState oldState, CharacterState newState)
    {
        // Override in subclasses for state-specific behavior
    }
}
```

---

### 2.4 Skill System 구조 (Strategy Pattern)

```
┌─────────────────────────────────────────────────────────────────┐
│                       SKILL SYSTEM                               │
│                                                                  │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │                   SkillBase (abstract)                      │ │
│  │  ┌─────────────────────────────────────────────────────┐   │ │
│  │  │ + Data: SkillData                                    │   │ │
│  │  │ + Level: int                                         │   │ │
│  │  │ + CooldownRemaining: float                           │   │ │
│  │  │ + IsReady: bool                                      │   │ │
│  │  │ ─────────────────────────────────────────────────    │   │ │
│  │  │ + Execute(caster, target): void {abstract}           │   │ │
│  │  │ + UpdateCooldown(deltaTime): void                    │   │ │
│  │  │ # OnExecute(): void {virtual}                        │   │ │
│  │  │ # CalculateDamage(baseDamage): float                 │   │ │
│  │  └─────────────────────────────────────────────────────┘   │ │
│  └──────────────────────────┬─────────────────────────────────┘ │
│                             │                                    │
│         ┌───────────────────┼───────────────────┐               │
│         ▼                   ▼                   ▼               │
│  ┌──────────────┐   ┌──────────────┐   ┌──────────────┐        │
│  │ ActiveSkill  │   │  BuffSkill   │   │UltimateSkill │        │
│  ├──────────────┤   ├──────────────┤   ├──────────────┤        │
│  │ projectiles  │   │ buffType     │   │ chargeTime   │        │
│  │ penetration  │   │ duration     │   │ aoeRadius    │        │
│  │ hitCount     │   │ tickDamage   │   │ isCharging   │        │
│  └──────────────┘   └──────────────┘   └──────────────┘        │
│                                                                  │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │                   SkillSlotManager                          │ │
│  │  ┌─────────────────────────────────────────────────────┐   │ │
│  │  │ - slots: SkillBase[3]                                │   │ │
│  │  │ - autoUseEnabled: bool                               │   │ │
│  │  │ ─────────────────────────────────────────────────    │   │ │
│  │  │ + EquipSkill(slot, skill): bool                      │   │ │
│  │  │ + UnequipSkill(slot): void                           │   │ │
│  │  │ + UseSkill(slot, target): void                       │   │ │
│  │  │ + GetReadySkill(): SkillBase                         │   │ │
│  │  │ + UpdateAllCooldowns(deltaTime): void                │   │ │
│  │  └─────────────────────────────────────────────────────┘   │ │
│  └────────────────────────────────────────────────────────────┘ │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

#### SkillBase 구현 명세

```csharp
public abstract class SkillBase
{
    public SkillData Data { get; private set; }
    public int Level { get; private set; } = 1;
    public float CooldownRemaining { get; private set; }
    public bool IsReady => CooldownRemaining <= 0;

    public SkillBase(SkillData data)
    {
        Data = data;
    }

    public void Execute(CharacterStats caster, IDamageable target)
    {
        if (!IsReady) return;

        OnExecute(caster, target);
        StartCooldown();

        GameEventBus.RaiseSkillUsed(this);
    }

    protected abstract void OnExecute(CharacterStats caster, IDamageable target);

    public void UpdateCooldown(float deltaTime)
    {
        if (CooldownRemaining > 0)
        {
            CooldownRemaining -= deltaTime;
            GameEventBus.RaiseSkillCooldownUpdate(this, CooldownRemaining);
        }
    }

    protected void StartCooldown()
    {
        CooldownRemaining = Data.cooldown;
    }

    protected float CalculateDamage(float baseDamage)
    {
        float levelBonus = 1f + (Level - 1) * Data.damagePerLevel;
        return baseDamage * Data.damageMultiplier * levelBonus;
    }

    public void LevelUp()
    {
        if (Level < Data.maxLevel)
        {
            Level++;
        }
    }
}
```

---

## 3. 데이터 흐름 아키텍처

### 3.1 단방향 데이터 흐름

```
┌─────────────────────────────────────────────────────────────────┐
│                  UNIDIRECTIONAL DATA FLOW                        │
│                                                                  │
│  ┌────────────┐                                                 │
│  │   INPUT    │  User Touch / Auto Battle Trigger               │
│  └─────┬──────┘                                                 │
│        │                                                         │
│        ▼                                                         │
│  ┌────────────┐                                                 │
│  │   ACTION   │  AttackCommand, UseSkillCommand, SelectCard     │
│  └─────┬──────┘                                                 │
│        │                                                         │
│        ▼                                                         │
│  ┌────────────┐                                                 │
│  │  MANAGER   │  BattleMediator, UpgradeManager                 │
│  │  (Logic)   │  Process action, validate, calculate            │
│  └─────┬──────┘                                                 │
│        │                                                         │
│        ▼                                                         │
│  ┌────────────┐                                                 │
│  │   STATE    │  CharacterStats, GameState, StageProgress       │
│  │  (Update)  │  Update internal state                          │
│  └─────┬──────┘                                                 │
│        │                                                         │
│        ▼                                                         │
│  ┌────────────┐                                                 │
│  │   EVENT    │  GameEventBus.OnXXX                             │
│  │  (Notify)  │  Broadcast state changes                        │
│  └─────┬──────┘                                                 │
│        │                                                         │
│        ▼                                                         │
│  ┌────────────┐                                                 │
│  │    UI      │  HPBar, XPBar, SkillButtons                     │
│  │  (Render)  │  Update visual representation                   │
│  └────────────┘                                                 │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

### 3.2 예시: 적 처치 데이터 흐름

```
Player Attack ──▶ EnemyController.TakeDamage()
                          │
                          ▼
                  [HP 계산, 사망 판정]
                          │
                          ▼
                  EnemyController.OnDeath()
                          │
                          ▼
              GameEventBus.OnEnemyKilled(enemy, xp)
                          │
         ┌────────────────┼────────────────┐
         ▼                ▼                ▼
    StageManager    LevelUpManager      UI (킬 카운트)
    (killCount++)   (AddXP)             (업데이트)
         │                │
         ▼                ▼
  [웨이브 완료 체크]  [레벨업 체크]
         │                │
         ▼                ▼
  OnWaveComplete    OnPlayerLevelUp
```

---

## 4. 모듈 간 의존성 규칙

### 4.1 의존성 규칙 (Dependency Rule)

```
┌─────────────────────────────────────────────────────────────────┐
│                    DEPENDENCY DIRECTION                          │
│                                                                  │
│                         ┌───────┐                               │
│                         │  UI   │                               │
│                         └───┬───┘                               │
│                             │ depends on                         │
│                             ▼                                    │
│                    ┌─────────────────┐                          │
│                    │   Game Logic    │                          │
│                    └────────┬────────┘                          │
│                             │ depends on                         │
│                             ▼                                    │
│                    ┌─────────────────┐                          │
│                    │    Entities     │                          │
│                    └────────┬────────┘                          │
│                             │ depends on                         │
│                             ▼                                    │
│                    ┌─────────────────┐                          │
│                    │      Data       │                          │
│                    │ (ScriptableObj) │                          │
│                    └────────┬────────┘                          │
│                             │ depends on                         │
│                             ▼                                    │
│                    ┌─────────────────┐                          │
│                    │ Infrastructure  │                          │
│                    └─────────────────┘                          │
│                                                                  │
│  규칙: 화살표 방향으로만 의존 가능                                  │
│  위반: Infrastructure → UI (금지)                                │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

### 4.2 허용/금지 의존성 표

| From \ To | Infrastructure | Data | Entity | Logic | UI |
|-----------|:--------------:|:----:|:------:|:-----:|:--:|
| **Infrastructure** | ✅ | ❌ | ❌ | ❌ | ❌ |
| **Data** | ✅ | ✅ | ❌ | ❌ | ❌ |
| **Entity** | ✅ | ✅ | ✅ | ❌ | ❌ |
| **Logic** | ✅ | ✅ | ✅ | ✅ | ❌ |
| **UI** | ✅ | ✅ | ✅ (읽기만) | ✅ (이벤트) | ✅ |

---

## 5. 확장 포인트 (Extension Points)

### 5.1 새 스킬 추가

```csharp
// 1. SkillBase 상속
public class NewSkill : SkillBase
{
    public NewSkill(SkillData data) : base(data) { }

    protected override void OnExecute(CharacterStats caster, IDamageable target)
    {
        // 새 스킬 로직 구현
    }
}

// 2. SkillFactory에 등록
public static class SkillFactory
{
    public static SkillBase Create(SkillData data)
    {
        return data.skillType switch
        {
            SkillType.Active => new ActiveSkill(data),
            SkillType.Buff => new BuffSkill(data),
            SkillType.Ultimate => new UltimateSkill(data),
            SkillType.NewType => new NewSkill(data),  // 추가
            _ => throw new ArgumentException()
        };
    }
}

// 3. ScriptableObject 에셋 생성
// Assets/10.ScriptableObjects/Skills/SK_NewSkill.asset
```

### 5.2 새 적 타입 추가

```csharp
// 1. EnemyBase 상속 또는 BossController 확장
public class EliteEnemy : EnemyController
{
    [SerializeField] private SkillData[] _eliteSkills;

    protected override void OnDeath()
    {
        base.OnDeath();
        // 엘리트 전용 드랍 로직
    }
}

// 2. EnemyData에 EnemyType.Elite 추가
// 3. SpawnManager에서 엘리트 스폰 로직 처리
```

---

## 6. 테스트 가능성 설계

### 6.1 인터페이스 기반 설계

```csharp
// 테스트를 위한 인터페이스 분리
public interface ITargetFinder
{
    IDamageable FindNearestTarget(Vector3 position);
    IEnumerable<IDamageable> FindTargetsInRange(Vector3 center, float radius);
}

public interface IDamageCalculator
{
    float Calculate(float baseDamage, CharacterStats attacker, CharacterStats defender);
}

// 실제 구현
public class BattleTargetFinder : ITargetFinder { ... }
public class StandardDamageCalculator : IDamageCalculator { ... }

// 테스트용 Mock
public class MockTargetFinder : ITargetFinder { ... }
public class MockDamageCalculator : IDamageCalculator { ... }
```

### 6.2 의존성 주입 포인트

```csharp
public class AutoBattleController : MonoBehaviour
{
    private ITargetFinder _targetFinder;
    private IDamageCalculator _damageCalculator;

    // 런타임 주입
    public void Initialize(ITargetFinder finder, IDamageCalculator calc)
    {
        _targetFinder = finder;
        _damageCalculator = calc;
    }

    // 테스트 시 Mock 주입 가능
}
```

---

## 7. 성능 최적화 구조

### 7.1 Update 최적화

```csharp
// BAD: 매 프레임 모든 적 검색
void Update()
{
    var enemies = FindObjectsOfType<EnemyController>();  // 비효율
}

// GOOD: 캐시된 리스트 사용
void Update()
{
    var enemies = _battleMediator.ActiveEnemies;  // 캐시된 리스트
}
```

### 7.2 이벤트 기반 업데이트

```csharp
// BAD: 매 프레임 HP 체크
void Update()
{
    _hpBar.SetValue(_player.CurrentHP / _player.MaxHP);
}

// GOOD: 이벤트 기반 업데이트
void OnEnable()
{
    GameEventBus.OnPlayerHPChanged += UpdateHPBar;
}

void UpdateHPBar(float current, float max)
{
    _hpBar.SetValue(current / max);
}
```

---

---

## 8. 시너지 시스템 구조 (신규)

> **핵심 컨셉**: 특정 스킬 + 스탯 조합이 시너지 효과 발생. 플레이어는 자유롭게 빌드를 선택하고 그 결과를 경험.

### 8.1 Synergy System 구조

```
┌─────────────────────────────────────────────────────────────────┐
│                      SYNERGY SYSTEM                               │
│                                                                  │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │                    SynergyManager                           │ │
│  │  ┌─────────────────────────────────────────────────────┐   │ │
│  │  │ - _allSynergies: SynergyData[]                       │   │ │
│  │  │ - _activeSynergies: List<ActiveSynergy>              │   │ │
│  │  │ ─────────────────────────────────────────────────    │   │ │
│  │  │ + CheckAllSynergies(buildState): void                │   │ │
│  │  │ + GetActiveSynergies(): List<SynergyData>            │   │ │
│  │  │ + ApplySynergyEffects(stats): void                   │   │ │
│  │  │ + RemoveSynergyEffects(stats): void                  │   │ │
│  │  └─────────────────────────────────────────────────────┘   │ │
│  └────────────────────────────────────────────────────────────┘ │
│                              │                                   │
│                              │ raises events                     │
│                              ▼                                   │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │                    GameEventBus                             │ │
│  │  ┌─────────────────────────────────────────────────────┐   │ │
│  │  │ + OnSynergyActivated(SynergyData)                    │   │ │
│  │  │ + OnSynergyDeactivated(SynergyData)                  │   │ │
│  │  └─────────────────────────────────────────────────────┘   │ │
│  └────────────────────────────────────────────────────────────┘ │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

### 8.2 SynergyManager 구현 명세

```csharp
public class SynergyManager : MonoBehaviour
{
    [Header("Synergy Data")]
    [SerializeField] private SynergyData[] _allSynergies;

    private List<ActiveSynergy> _trackedSynergies = new List<ActiveSynergy>();
    private CharacterStats _playerStats;

    private void Start()
    {
        // 모든 시너지 추적 목록에 추가
        foreach (var data in _allSynergies)
        {
            _trackedSynergies.Add(new ActiveSynergy(data));
        }
    }

    public void CheckAllSynergies(PlayerBuildState buildState)
    {
        foreach (var synergy in _trackedSynergies)
        {
            bool meetsCondition = synergy.data.CheckConditions(buildState);

            if (meetsCondition && !synergy.isActive)
            {
                ActivateSynergy(synergy, buildState);
            }
            else if (!meetsCondition && synergy.isActive)
            {
                DeactivateSynergy(synergy, buildState);
            }
        }
    }

    private void ActivateSynergy(ActiveSynergy synergy, PlayerBuildState buildState)
    {
        synergy.Activate();
        buildState.activeSynergies.Add(synergy.data.synergyType);

        // 효과 적용
        ApplySynergyEffect(synergy.data);

        // 이벤트 발행
        GameEventBus.RaiseSynergyActivated(synergy.data);

        // 이펙트/사운드
        if (synergy.data.activateEffect != null)
        {
            // 이펙트 재생
        }
        if (synergy.data.activateSound != null)
        {
            // 사운드 재생
        }
    }

    private void DeactivateSynergy(ActiveSynergy synergy, PlayerBuildState buildState)
    {
        synergy.Deactivate();
        buildState.activeSynergies.Remove(synergy.data.synergyType);

        // 효과 제거
        RemoveSynergyEffect(synergy.data);

        // 이벤트 발행
        GameEventBus.RaiseSynergyDeactivated(synergy.data);
    }

    private void ApplySynergyEffect(SynergyData data)
    {
        switch (data.effectType)
        {
            case SynergyEffectType.DotDamageBonus:
                // DoT 데미지 증가 적용
                break;
            case SynergyEffectType.PoisonStackBonus:
                // 독 중첩 한도 증가
                break;
            case SynergyEffectType.DamageReduction:
                // 받는 피해 감소 적용
                break;
            case SynergyEffectType.CritExtraHit:
                // 크리티컬 추가 타격 활성화
                break;
            case SynergyEffectType.CooldownReduction:
                // 스킬 쿨다운 감소
                break;
            case SynergyEffectType.LifestealBonus:
                // 흡혈량 증가
                break;
        }
    }

    private void RemoveSynergyEffect(SynergyData data)
    {
        // ApplySynergyEffect의 역연산
    }

    public List<SynergyData> GetActiveSynergies()
    {
        return _trackedSynergies
            .Where(s => s.isActive)
            .Select(s => s.data)
            .ToList();
    }

    public List<SynergyData> GetAvailableSynergies(PlayerBuildState buildState)
    {
        return _trackedSynergies
            .Where(s => !s.isActive && s.data.CheckConditions(buildState))
            .Select(s => s.data)
            .ToList();
    }
}
```

### 8.3 DifficultyScaler 구현 명세

```csharp
public class DifficultyScaler : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private DifficultyConfig _config;

    private PlayerBuildState _buildState;
    private ChapterDifficulty _currentDifficulty;

    private float _dpsCheckTimer;
    private bool _isDpsCheckActive;

    public void SetChapter(int chapter)
    {
        _currentDifficulty = _config.GetDifficulty(chapter);
        ResetDpsCheck();
    }

    // 적 스탯 스케일링
    public float GetEnemyStatMultiplier()
    {
        return _currentDifficulty?.enemyStatMultiplier ?? 1f;
    }

    // DPS 체크 시작
    public void StartDpsCheck()
    {
        if (_currentDifficulty != null && _currentDifficulty.hasDpsCheck)
        {
            _isDpsCheckActive = true;
            _dpsCheckTimer = _currentDifficulty.dpsCheckTime;
        }
    }

    private void Update()
    {
        if (_isDpsCheckActive)
        {
            _dpsCheckTimer -= Time.deltaTime;

            if (_dpsCheckTimer <= 0)
            {
                // DPS 체크 실패
                GameEventBus.RaiseDpsCheckFailed();
                _isDpsCheckActive = false;
            }
        }
    }

    public void OnBossDefeated()
    {
        _isDpsCheckActive = false;
    }

    // 빌드 요구사항 충족 검증
    public bool ValidateBuildRequirements()
    {
        if (_currentDifficulty == null || !_currentDifficulty.hasBuildRequirement)
        {
            return true;
        }

        bool hasEnoughSkills = _buildState.equippedSkills.Count >= _currentDifficulty.minSkillCount;
        bool hasEnoughSynergies = _buildState.activeSynergies.Count >= _currentDifficulty.minSynergyCount;

        return hasEnoughSkills && hasEnoughSynergies;
    }

    // 생존 체크
    public bool ValidateSurvivalRequirements(CharacterStats playerStats, GameConfig gameConfig)
    {
        if (_currentDifficulty == null || !_currentDifficulty.hasSurvivalCheck)
        {
            return true;
        }

        float currentHpRatio = playerStats.maxHP / gameConfig.initialHP;
        float currentDefRatio = playerStats.def / gameConfig.initialDEF;

        return currentHpRatio >= _currentDifficulty.minHpRatio &&
               currentDefRatio >= _currentDifficulty.minDefRatio;
    }

    private void ResetDpsCheck()
    {
        _isDpsCheckActive = false;
        _dpsCheckTimer = 0f;
    }
}
```

### 8.4 시너지 시스템 데이터 흐름

```
카드 선택 ──▶ PlayerBuildState 업데이트
                      │
                      └── SynergyManager.CheckAllSynergies()
                                │
                                ├── 조건 충족 시너지 활성화
                                │         │
                                │         ▼
                                │   GameEventBus.OnSynergyActivated
                                │         │
                                │         ▼
                                │   SynergyDisplayUI 업데이트
                                │
                                └── 시너지 효과 적용
                                          │
                                          ▼
                                    CharacterStats 수정
```

---

## 9. 체크리스트

### 아키텍처 검증
- [x] SOLID 원칙 준수
- [x] 디미터의 법칙 준수
- [x] 단방향 의존성
- [x] 이벤트 기반 통신
- [x] 확장 포인트 정의
- [x] 테스트 가능 구조

### 성능 고려사항
- [x] Object Pool 적용
- [x] 컴포넌트 캐싱
- [x] 이벤트 기반 업데이트
- [x] GC 최소화 구조

### 시너지 시스템 검증
- [x] SynergyManager 단일 책임 (시너지 조건 체크/적용만 담당)
- [x] SynergyManager 확장 가능 (새 시너지 추가 용이)
- [x] 이벤트 기반 UI 업데이트
- [x] DifficultyScaler 챕터별 설정 분리