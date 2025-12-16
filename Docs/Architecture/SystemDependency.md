# DungeonRush 시스템 의존성 및 이벤트 설계

## 1. 시스템 의존성 매트릭스

### 1.1 의존성 방향 규칙
- **상위 → 하위**: 허용 (Core → 다른 시스템)
- **동일 레벨**: 이벤트 버스 통한 간접 통신만 허용
- **하위 → 상위**: 금지 (역방향 의존성)

### 1.2 의존성 매트릭스

| 시스템 | Core | Stage | Battle | Character | Skill | Upgrade | UI |
|--------|:----:|:-----:|:------:|:---------:|:-----:|:-------:|:--:|
| **Core** | - | ○ | ○ | ○ | ○ | ○ | ○ |
| **Stage** | ● | - | E | E | × | E | × |
| **Battle** | ● | E | - | ● | ● | × | × |
| **Character** | ● | × | E | - | E | E | × |
| **Skill** | ● | × | E | ● | - | × | × |
| **Upgrade** | ● | E | × | ● | ● | - | × |
| **UI** | ● | E | E | E | E | E | - |

**범례**:
- ●: 직접 의존 (인터페이스/추상화 통해)
- ○: 소유/관리 관계
- E: 이벤트 구독/발행
- ×: 의존 없음
- -: 자기 자신

---

## 2. 이벤트 버스 설계

### 2.1 GameEventBus 구조

```csharp
public static class GameEventBus
{
    // ═══════════════════════════════════════════
    // 게임 상태 이벤트
    // ═══════════════════════════════════════════
    public static event Action<GameState> OnGameStateChanged;
    public static event Action OnGameStart;
    public static event Action OnGamePause;
    public static event Action OnGameResume;
    public static event Action OnGameOver;

    // ═══════════════════════════════════════════
    // 스테이지 이벤트
    // ═══════════════════════════════════════════
    public static event Action<int, int> OnWaveStart;      // (chapter, wave)
    public static event Action<int, int> OnWaveComplete;   // (chapter, wave)
    public static event Action<int> OnChapterStart;        // (chapter)
    public static event Action<int> OnChapterComplete;     // (chapter)
    public static event Action OnBossWaveStart;

    // ═══════════════════════════════════════════
    // 전투 이벤트
    // ═══════════════════════════════════════════
    public static event Action<IDamageable, float, DamageType> OnDamageDealt;
    public static event Action<EnemyController> OnEnemySpawned;
    public static event Action<EnemyController, int> OnEnemyKilled;  // (enemy, xp)
    public static event Action<BossController> OnBossSpawned;
    public static event Action<BossController> OnBossKilled;

    // ═══════════════════════════════════════════
    // 플레이어 이벤트
    // ═══════════════════════════════════════════
    public static event Action<float, float> OnPlayerHPChanged;    // (current, max)
    public static event Action<int, int> OnPlayerXPChanged;        // (current, required)
    public static event Action<int> OnPlayerLevelUp;               // (newLevel)
    public static event Action OnPlayerDeath;

    // ═══════════════════════════════════════════
    // 스킬 이벤트
    // ═══════════════════════════════════════════
    public static event Action<SkillBase> OnSkillUsed;
    public static event Action<SkillBase, float> OnSkillCooldownUpdate;  // (skill, remaining)
    public static event Action<int, SkillData> OnSkillEquipped;    // (slotIndex, skill)
    public static event Action<int> OnSkillUnequipped;             // (slotIndex)

    // ═══════════════════════════════════════════
    // 업그레이드 이벤트
    // ═══════════════════════════════════════════
    public static event Action<CardData[]> OnCardChoicesReady;     // (3장의 카드)
    public static event Action<CardData> OnCardSelected;
    public static event Action<int, int> OnRerollUsed;             // (cost, remainingRerolls)
    public static event Action<int> OnGoldChanged;                 // (currentGold)
}
```

### 2.2 이벤트 발행/구독 패턴

```
┌─────────────────────────────────────────────────────────────────────┐
│                        이벤트 발행자 (Publisher)                      │
├─────────────────────────────────────────────────────────────────────┤
│ StageManager                                                         │
│   └─▶ OnWaveStart, OnWaveComplete, OnChapterComplete, OnBossWaveStart│
│                                                                      │
│ PlayerController                                                     │
│   └─▶ OnPlayerHPChanged, OnPlayerXPChanged, OnPlayerDeath           │
│                                                                      │
│ LevelUpManager                                                       │
│   └─▶ OnPlayerLevelUp, OnCardChoicesReady                           │
│                                                                      │
│ EnemyController                                                      │
│   └─▶ OnEnemyKilled                                                 │
│                                                                      │
│ BossController                                                       │
│   └─▶ OnBossKilled                                                  │
│                                                                      │
│ SkillSlotManager                                                     │
│   └─▶ OnSkillUsed, OnSkillCooldownUpdate, OnSkillEquipped           │
│                                                                      │
│ CardSelectSystem                                                     │
│   └─▶ OnCardSelected, OnRerollUsed                                  │
└─────────────────────────────────────────────────────────────────────┘

                              │
                              ▼
                       GameEventBus
                              │
                              ▼

┌─────────────────────────────────────────────────────────────────────┐
│                        이벤트 구독자 (Subscriber)                     │
├─────────────────────────────────────────────────────────────────────┤
│ HUDManager                                                           │
│   └─◀ OnPlayerHPChanged, OnPlayerXPChanged, OnWaveStart             │
│                                                                      │
│ HPBar                                                                │
│   └─◀ OnPlayerHPChanged                                             │
│                                                                      │
│ XPBar                                                                │
│   └─◀ OnPlayerXPChanged, OnPlayerLevelUp                            │
│                                                                      │
│ StageDisplay                                                         │
│   └─◀ OnWaveStart, OnBossWaveStart                                  │
│                                                                      │
│ SkillButtonUI                                                        │
│   └─◀ OnSkillCooldownUpdate, OnSkillEquipped                        │
│                                                                      │
│ CardSelectUI                                                         │
│   └─◀ OnCardChoicesReady, OnPlayerLevelUp                           │
│                                                                      │
│ ScoreResultUI                                                        │
│   └─◀ OnGameOver, OnPlayerDeath                                     │
│                                                                      │
│ StageManager                                                         │
│   └─◀ OnEnemyKilled, OnBossKilled                                   │
│                                                                      │
│ SpawnManager                                                         │
│   └─◀ OnWaveStart, OnBossWaveStart                                  │
│                                                                      │
│ LevelUpManager                                                       │
│   └─◀ OnPlayerXPChanged, OnEnemyKilled                              │
│                                                                      │
│ GameManager                                                          │
│   └─◀ OnPlayerDeath, OnCardSelected                                 │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 3. 시퀀스 다이어그램

### 3.1 게임 시작 시퀀스

```
User          GameManager       StageManager      SpawnManager        UI
  │                │                 │                 │               │
  │─ Start Game ──▶│                 │                 │               │
  │                │                 │                 │               │
  │                │─ Initialize ───▶│                 │               │
  │                │                 │                 │               │
  │                │◀─ Ready ────────│                 │               │
  │                │                 │                 │               │
  │                │═══ OnGameStateChanged(Playing) ══════════════════▶│
  │                │                 │                 │               │
  │                │                 │─ StartWave(1,1)▶│               │
  │                │                 │                 │               │
  │                │                 │═══ OnWaveStart(1,1) ═══════════▶│
  │                │                 │                 │               │
  │                │                 │                 │─ SpawnEnemy ─▶│
  │                │                 │                 │   (x10)       │
```

### 3.2 적 처치 → 레벨업 시퀀스

```
AutoBattle    Enemy       StageManager   LevelUpManager   CardSelectUI
    │           │              │               │               │
    │─ Attack ─▶│              │               │               │
    │           │              │               │               │
    │           │─ Die() ─────▶│               │               │
    │           │              │               │               │
    │           │══ OnEnemyKilled(enemy, 50xp) ════════════════▶
    │           │              │               │               │
    │           │              │─ killCount++ │               │
    │           │              │               │               │
    │           │              │               │─ AddXP(50) ──▶│
    │           │              │               │               │
    │           │              │               │══ OnPlayerXPChanged ══▶
    │           │              │               │               │
    │           │              │               │ [XP >= Required]
    │           │              │               │               │
    │           │              │               │══ OnPlayerLevelUp ════▶
    │           │              │               │               │
    │           │              │               │─ GenerateCards()
    │           │              │               │               │
    │           │              │               │══ OnCardChoicesReady ═▶│
    │           │              │               │               │
    │           │              │               │               │─ Show()
```

### 3.3 레벨업 카드 선택 시퀀스

```
User      CardSelectUI    CardSelectSystem    PlayerController    GameManager
  │            │                 │                   │                 │
  │─ Select ──▶│                 │                   │                 │
  │            │                 │                   │                 │
  │            │─ OnCardClick ──▶│                   │                 │
  │            │                 │                   │                 │
  │            │                 │─ ApplyCard() ────▶│                 │
  │            │                 │                   │                 │
  │            │                 │══ OnCardSelected(card) ═════════════▶
  │            │                 │                   │                 │
  │            │                 │                   │                 │─ Resume()
  │            │                 │                   │                 │
  │            │◀── Hide() ──────│                   │                 │
  │            │                 │                   │                 │
  │            │                 │                   │══ OnGameStateChanged(Playing)
```

### 3.4 보스 웨이브 시퀀스

```
StageManager    SpawnManager     BossController    UI           GameManager
     │               │                 │            │                │
     │ [wave == 10]  │                 │            │                │
     │               │                 │            │                │
     │══ OnBossWaveStart ═════════════════════════▶│                │
     │               │                 │            │                │
     │─ SpawnBoss() ▶│                 │            │                │
     │               │                 │            │                │
     │               │─ Instantiate() ▶│            │                │
     │               │                 │            │                │
     │               │══ OnBossSpawned(boss) ══════▶│                │
     │               │                 │            │                │
     │               │                 │ [HP <= 0]  │                │
     │               │                 │            │                │
     │               │                 │══ OnBossKilled ═════════════▶
     │               │                 │            │                │
     │◀═══════════ OnBossKilled ══════╡            │                │
     │               │                 │            │                │
     │─ NextChapter()│                 │            │                │
     │               │                 │            │                │
     │══ OnChapterComplete(1) ════════════════════▶│                │
```

---

## 4. 인터페이스 정의

### 4.1 전투 관련 인터페이스

```csharp
// 데미지를 받을 수 있는 객체
public interface IDamageable
{
    float CurrentHP { get; }
    float MaxHP { get; }
    bool IsDead { get; }

    void TakeDamage(float damage, DamageType type);
    void Heal(float amount);
}

// 공격을 수행할 수 있는 객체
public interface IAttacker
{
    float AttackPower { get; }
    float AttackRange { get; }
    float AttackSpeed { get; }

    void Attack(IDamageable target);
}

// 스킬을 사용할 수 있는 객체
public interface ISkillUser
{
    CharacterStats Stats { get; }
    SkillSlotManager SkillSlots { get; }

    void UseSkill(int slotIndex);
}

// 타겟팅 가능한 객체
public interface ITargetable
{
    Transform Transform { get; }
    bool IsValidTarget { get; }
}
```

### 4.2 풀링 관련 인터페이스

```csharp
// 풀링 대상 객체
public interface IPoolable
{
    void OnSpawn();    // 풀에서 꺼낼 때
    void OnDespawn();  // 풀로 반환할 때
}

// 풀 관리자
public interface IObjectPool<T> where T : IPoolable
{
    T Get();
    void Return(T obj);
    void Prewarm(int count);
}
```

### 4.3 UI 관련 인터페이스

```csharp
// 팝업 UI
public interface IPopup
{
    void Show();
    void Hide();
    bool IsVisible { get; }
}

// 값 표시 UI
public interface IValueDisplay
{
    void UpdateValue(float current, float max);
}
```

---

## 5. 의존성 주입 패턴

### 5.1 서비스 로케이터 (선택적)

```csharp
public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services = new();

    public static void Register<T>(T service)
    {
        _services[typeof(T)] = service;
    }

    public static T Get<T>()
    {
        return (T)_services[typeof(T)];
    }
}

// 사용 예시
ServiceLocator.Register<IObjectPool<Enemy>>(enemyPool);
var pool = ServiceLocator.Get<IObjectPool<Enemy>>();
```

### 5.2 생성자 주입 (권장)

```csharp
public class AutoBattleController
{
    private readonly ITargetFinder _targetFinder;
    private readonly IDamageCalculator _damageCalculator;

    public AutoBattleController(
        ITargetFinder targetFinder,
        IDamageCalculator damageCalculator)
    {
        _targetFinder = targetFinder;
        _damageCalculator = damageCalculator;
    }
}
```

---

## 6. 에러 처리 및 안전장치

### 6.1 Null 체크 패턴

```csharp
// 이벤트 발행 시 안전한 호출
public static void RaisePlayerHPChanged(float current, float max)
{
    OnPlayerHPChanged?.Invoke(current, max);
}
```

### 6.2 구독 해제 보장

```csharp
public class HPBar : MonoBehaviour
{
    private void OnEnable()
    {
        GameEventBus.OnPlayerHPChanged += UpdateHP;
    }

    private void OnDisable()
    {
        GameEventBus.OnPlayerHPChanged -= UpdateHP;
    }
}
```

---

## 7. 성능 고려사항

### 7.1 이벤트 최적화
- 매 프레임 발생하는 이벤트는 피하기 (예: 위치 업데이트)
- 배치 처리가 가능한 경우 모아서 발행
- 필요한 구독자만 이벤트 수신

### 7.2 메모리 관리
- 이벤트 구독 해제 필수 (메모리 누수 방지)
- 람다 대신 메서드 참조 사용 권장
- WeakReference 고려 (장기 실행 시스템)