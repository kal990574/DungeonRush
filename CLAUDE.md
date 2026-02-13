# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 프로젝트 개요
- **엔진**: Unity 6000.3.2f1 (LTS)
- **장르**: Idle + 로그라이크 (탑다운 2D 자동 전투, 메가봉크 스타일)
- **시점**: 탑다운 2D (위에서 아래로 내려다보는 시점)
- **맵**: 고정 아레나 (일정 크기의 전투 영역, 적은 아레나 가장자리에서 사방으로 스폰)
- **이동**: 완전 자동 (플레이어가 가장 가까운 적에게 자동 접근 후 공격)
- **플랫폼**: Android (가로)
- **언어**: C#
- **렌더링**: URP (Universal Render Pipeline)

---

## 0. Claude Code Assistant 지침

* **언어:** 모든 코드 리뷰 요약 및 코멘트를 **한국어(Korean)**로 작성합니다. 명확하고 자연스러운 한글 설명을 사용합니다.
* **디자인 원칙 검토:** 모든 PR에 대해 **SOLID 원칙**과 **디미터의 법칙(Law of Demeter)** 위반 여부를 중점적으로 확인합니다.
* **함수:** 함수는 한 가지 일만 합니다.
* **가이드 기반 코딩:** 사용자가 직접 코드를 작성하는 방식을 선호합니다. 코드 템플릿 제공 시 다음 형식을 따릅니다:
  1. **파일 경로**: 생성할 파일의 전체 경로
  2. **코드 템플릿**: 작성할 전체 코드
  3. **코드 설명**: 주요 요소별 설명 (표 형식)
  4. **활용처**: 이 코드가 프로젝트에서 어디에, 어떻게 사용되는지 구체적으로 명시

### 0.1. 핵심 디자인 원칙 (SOLID & LoD)

#### SOLID 원칙

| 약어 | 원칙 | 핵심 요약 |
| :---: | :--- | :--- |
| **S** | **단일 책임 원칙 (SRP)** | 클래스는 **단 하나의 변경 이유**만 가져야 합니다. |
| **O** | **개방-폐쇄 원칙 (OCP)** | **확장에는 열려 있고, 수정에는 닫혀 있어야** 합니다. |
| **L** | **리스코프 치환 원칙 (LSP)** | 상위 타입을 하위 타입으로 **치환해도 문제없이 작동**해야 합니다. |
| **I** | **인터페이스 분리 원칙 (ISP)** | **단일 목적의 작은 인터페이스**를 선호합니다. |
| **D** | **의존성 역전 원칙 (DIP)** | **고수준/저수준 모듈 모두 추상화에 의존**해야 합니다. |

#### 디미터의 법칙 (LoD)

* **핵심:** "오직 가장 가까운 친구와만 이야기하라"
* **위반 방지:** `a.getB().getC().doSomething()` 같은 **"기차 참사(Train Wreck)"** 패턴 금지

### 0.2. 보이스카우트 원칙

**"코드를 발견했을 때보다 더 깨끗하게 만들어 놓고 떠나라"**

- 작은 개선이라도 누적하면 큰 품질 향상
- 중복 제거, 네이밍 개선, 불필요한 복잡성 제거
- 미사용 변수, 죽은 코드(dead code) 정리
- 테스트 가능성, 확장 가능성 향상

### 0.3. YAGNI 원칙 (You Aren't Gonna Need It)

**"지금 당장 필요하지 않은 것은 구현하지 마라"**

- 현재 사용되지 않는 이벤트, 함수, 콜백은 **작성하지 않음**
- "나중에 쓸 것 같다"는 추측으로 미리 구현 금지
- 빈 메서드, 미사용 인터페이스 멤버, 호출처 없는 유틸리티 함수 금지
- 필요한 시점에 구현해도 충분하며, 미리 만든 코드는 유지보수 부담만 증가
- **예외**: 프레임워크/엔진이 강제하는 생명주기 메서드(예: `Start()`, `Update()`)는 제외

```csharp
// Good - 현재 필요한 것만 구현.
public class EnemyController : MonoBehaviour
{
    public void TakeDamage(float amount)
    {
        _currentHp -= amount;
    }
}

// Bad - 호출처가 없는데 "나중에 쓸 것 같아서" 미리 구현.
public class EnemyController : MonoBehaviour
{
    public event Action OnDamaged;      // 아무도 구독하지 않음.
    public event Action OnHealed;       // 힐 시스템이 아직 없음.
    public event Action OnStatusEffect; // 상태이상 시스템 미구현.

    public void TakeDamage(float amount)
    {
        _currentHp -= amount;
        OnDamaged?.Invoke();
    }

    public void Heal(float amount)      // 호출처 없음.
    {
        _currentHp += amount;
        OnHealed?.Invoke();
    }
}
```

### 0.4. 인터페이스 우선 설계 (Interface-First Design)

#### 핵심 원칙
- **구현보다 추상화 우선**: 클래스 작성 전 인터페이스부터 정의
- **구체 타입 의존 금지**: 필드/매개변수는 인터페이스 타입 사용
- **작은 인터페이스**: ISP 원칙에 따라 단일 책임의 작은 인터페이스 선호

#### 인터페이스 설계 규칙

| 규칙 | 설명 | 예시 |
|------|------|------|
| **I 접두사** | 모든 인터페이스는 `I`로 시작 | `IDamageable`, `IAttacker` |
| **행위 기반 명명** | 할 수 있는 것(-able) 또는 역할(-er) | `IPoolable`, `ITargetFinder` |
| **메서드 최소화** | 인터페이스당 1~3개 메서드 권장 | 많으면 분리 |

#### 의존성 주입 패턴

```csharp
// Good - 인터페이스에 의존
public class AutoBattleController
{
    private readonly ITargetFinder _targetFinder;
    private readonly IDamageCalculator _damageCalculator;

    public AutoBattleController(ITargetFinder finder, IDamageCalculator calc)
    {
        _targetFinder = finder;
        _damageCalculator = calc;
    }
}

// Bad - 구체 클래스에 의존
public class AutoBattleController
{
    private readonly BattleMediator _mediator;  // 구체 클래스 직접 참조
}
```

#### 프로젝트 핵심 인터페이스

| 인터페이스 | 용도 |
|------------|------|
| `IDamageable` | 데미지를 받을 수 있는 객체 |
| `IAttacker` | 공격을 수행할 수 있는 객체 |
| `IPoolable` | 오브젝트 풀링 대상 |
| `ITargetable` | 타겟팅 가능한 객체 |
| `ISkillExecutor` | 스킬 실행 가능한 객체 |

### 0.5. 클린 아키텍처 (Clean Architecture)

#### 핵심 원칙
- **의존성 규칙**: 의존성은 항상 **외부 → 내부** 방향으로만 흐른다
- **내부 레이어 독립성**: Domain 레이어는 외부 레이어(Unity, UI)를 알지 못한다
- **경계 명확화**: 레이어 간 통신은 **인터페이스**를 통해서만 수행

```
┌─────────────────────────────────────────────────────┐
│                 Presentation Layer                   │
│              (MonoBehaviour, UI, View)               │
├─────────────────────────────────────────────────────┤
│                 Application Layer                    │
│              (UseCase, Service, Manager)             │
├─────────────────────────────────────────────────────┤
│                   Domain Layer                       │
│            (Entity, Model, Interface)                │
├─────────────────────────────────────────────────────┤
│                Infrastructure Layer                  │
│          (Repository, External, Platform)            │
└─────────────────────────────────────────────────────┘
          ↑ 의존성 방향 (외부 → 내부)
```

#### 레이어별 책임

| 레이어 | 책임 | 포함 요소 | 의존 대상 |
|--------|------|-----------|-----------|
| **Presentation** | 화면 표시, 사용자 입력 처리 | MonoBehaviour, UI, View, Presenter | Application |
| **Application** | 비즈니스 로직 조율, 유스케이스 실행 | UseCase, Service, Manager | Domain |
| **Domain** | 핵심 비즈니스 규칙, 게임 로직 | Entity, Model, Interface, VO | 없음 (독립) |
| **Infrastructure** | 외부 시스템 연동, 데이터 영속성 | Repository, API, SaveSystem | Domain (인터페이스) |

#### 인게임 / 아웃게임 분리

게임 시스템을 **InGame**(실제 게임플레이)과 **OutGame**(메타 시스템)으로 명확히 분리합니다.

| 구분 | InGame | OutGame |
|------|--------|---------|
| **정의** | 실제 게임플레이가 진행되는 영역 | 게임플레이 외부의 메타 시스템 |
| **씬** | `GameScene` | `LobbyScene`, `ResultScene` |
| **시스템** | 전투, 캐릭터, 스킬, 아레나, 웨이브 | 로비, 상점, 인벤토리, 설정, 랭킹 |
| **상태** | 런타임 상태 (HP, 위치, 쿨다운) | 영속 상태 (재화, 해금, 설정) |
| **Time.timeScale** | 영향 받음 (일시정지 가능) | 영향 받지 않음 |

```csharp
// Good - InGame/OutGame 네임스페이스 분리
namespace DungeonRush.InGame.Combat { }
namespace DungeonRush.InGame.Character { }
namespace DungeonRush.OutGame.Lobby { }
namespace DungeonRush.OutGame.Shop { }

// Bad - 분리 없이 혼재
namespace DungeonRush.Systems { }  // InGame? OutGame? 불명확
```

#### 레이어 간 통신 규칙

```csharp
// Good - 인터페이스를 통한 의존성 역전
// Domain Layer (인터페이스 정의)
public interface IScoreRepository
{
    void Save(ScoreData data);
    ScoreData Load();
}

// Infrastructure Layer (구현)
public class LocalScoreRepository : IScoreRepository
{
    public void Save(ScoreData data) { /* PlayerPrefs 사용 */ }
    public ScoreData Load() { /* PlayerPrefs 사용 */ }
}

// Application Layer (인터페이스에 의존)
public class ScoreService
{
    private readonly IScoreRepository _repository;

    public ScoreService(IScoreRepository repository)
    {
        _repository = repository;
    }
}

// Bad - 구체 클래스 직접 참조
public class ScoreService
{
    private readonly LocalScoreRepository _repository;  // 구체 타입 의존
}
```

#### 순환 의존성 금지

```csharp
// Bad - 순환 의존성 (A → B → A)
public class PlayerController
{
    private EnemyController _enemy;  // Player → Enemy
}

public class EnemyController
{
    private PlayerController _player;  // Enemy → Player (순환!)
}

// Good - 인터페이스로 순환 해소
public interface ITargetable
{
    Vector3 Position { get; }
    bool IsAlive { get; }
}

public class PlayerController : ITargetable { }
public class EnemyController : ITargetable
{
    private ITargetable _target;  // 인터페이스에 의존
}
```

#### 금지 사항

| 금지 | 이유 | 대안 |
|------|------|------|
| Domain → Presentation 참조 | 내부 레이어가 외부를 알면 안 됨 | 이벤트/콜백 사용 |
| Domain에서 MonoBehaviour 상속 | Unity 의존성 제거 | 순수 C# 클래스 사용 |
| Infrastructure 직접 참조 | 구체 구현에 의존 | 인터페이스 통한 DI |
| InGame ↔ OutGame 직접 참조 | 결합도 증가 | 이벤트 버스, 공유 서비스 |
| 씬 간 싱글톤 남용 | 테스트 어려움, 숨겨진 의존성 | 명시적 의존성 주입 |

#### 데이터 흐름

```
[User Input] → Presentation → Application → Domain
                                              ↓
[Screen Update] ← Presentation ← Application ← Domain
                                              ↓
                              Infrastructure (저장/로드)
```

### 0.6. 게임 디자인 패턴

#### 패턴 선택 가이드

| 패턴 | 사용 시점 | 프로젝트 적용 대상 |
|------|----------|-------------------|
| **State Machine** | 3~7개의 명확한 상태 전환 | 플레이어/적 상태 (Idle, Move, Attack, Dead) |
| **Object Pooling** | 빈번한 생성/파괴 | 투사체, 이펙트, 적 |
| **Observer/Event** | 1:N 통신, 느슨한 결합 | HP 변경→UI, 적 사망→XP/점수 |
| **Command** | 실행 취소, 큐잉, 로깅 | 스킬 실행, 데미지 기록, 리플레이 |
| **Strategy** | 동일 인터페이스, 다른 알고리즘 | 적 AI, 데미지 계산, 타겟 선택 |
| **Factory** | 객체 생성 로직 캡슐화 | 적 스폰, 카드 생성, 스킬 생성 |
| **Flyweight** | 공유 가능한 불변 데이터 | ScriptableObject (스탯, 스킬 데이터) |
| **Service Locator** | 전역 서비스 접근 (DI 대안) | Audio, Save, Analytics |

#### Observer/Event 패턴 (레이어 간 통신)

```csharp
// Domain Layer - 이벤트 정의
public readonly struct EnemyDefeatedEvent
{
    public readonly int EnemyId;
    public readonly int XpReward;
    public readonly Vector3 Position;

    public EnemyDefeatedEvent(int enemyId, int xpReward, Vector3 position)
    {
        EnemyId = enemyId;
        XpReward = xpReward;
        Position = position;
    }
}

// Shared - 이벤트 버스 인터페이스
public interface IEventBus
{
    void Publish<T>(T eventData) where T : struct;
    void Subscribe<T>(Action<T> handler) where T : struct;
    void Unsubscribe<T>(Action<T> handler) where T : struct;
}

// Presentation Layer - 구독
public class XPBarUI : MonoBehaviour
{
    private IEventBus _eventBus;

    private void OnEnable()
    {
        _eventBus.Subscribe<EnemyDefeatedEvent>(OnEnemyDefeated);
    }

    private void OnDisable()
    {
        _eventBus.Unsubscribe<EnemyDefeatedEvent>(OnEnemyDefeated);
    }

    private void OnEnemyDefeated(EnemyDefeatedEvent e)
    {
        AddXP(e.XpReward);
    }
}
```

#### Command 패턴 (스킬 실행)

```csharp
// Domain Layer - 커맨드 인터페이스
public interface ISkillCommand
{
    void Execute();
    bool CanExecute();
}

// Application Layer - 구체 커맨드
public class ProjectileSkillCommand : ISkillCommand
{
    private readonly ITargetable _target;
    private readonly SkillData _data;
    private readonly IProjectileSpawner _spawner;

    public ProjectileSkillCommand(ITargetable target, SkillData data, IProjectileSpawner spawner)
    {
        _target = target;
        _data = data;
        _spawner = spawner;
    }

    public bool CanExecute() => _target != null && _target.IsAlive;

    public void Execute()
    {
        _spawner.Spawn(_data.ProjectilePrefab, _target);
    }
}

// 활용: 스킬 큐, 실행 로그, 리플레이 시스템
```

#### Strategy 패턴 (적 AI)

```csharp
// Domain Layer - 전략 인터페이스
public interface IEnemyBehavior
{
    void UpdateBehavior(EnemyContext context);
}

// Application Layer - 구체 전략
public class MeleeChaseStrategy : IEnemyBehavior
{
    public void UpdateBehavior(EnemyContext context)
    {
        // 플레이어에게 접근 후 근접 공격
        context.MoveToward(context.Target.Position);
        if (context.IsInAttackRange)
        {
            context.Attack();
        }
    }
}

public class RangedKiteStrategy : IEnemyBehavior
{
    public void UpdateBehavior(EnemyContext context)
    {
        // 거리 유지하며 원거리 공격
        if (context.IsTooClose)
        {
            context.MoveAwayFrom(context.Target.Position);
        }
        context.Attack();
    }
}

// Presentation Layer - 사용
public class EnemyController : MonoBehaviour
{
    private IEnemyBehavior _behavior;

    public void SetBehavior(IEnemyBehavior behavior)
    {
        _behavior = behavior;
    }
}
```

#### Factory 패턴 (적 스폰)

```csharp
// Domain Layer - 팩토리 인터페이스
public interface IEnemyFactory
{
    IEnemy Create(EnemyType type, Vector3 position);
}

// Infrastructure Layer - 구현
public class EnemyFactory : IEnemyFactory
{
    private readonly IObjectPool<Enemy> _pool;
    private readonly Dictionary<EnemyType, EnemyStatData> _statDatabase;

    public IEnemy Create(EnemyType type, Vector3 position)
    {
        var enemy = _pool.Get();
        var stats = _statDatabase[type];
        enemy.Initialize(stats, position);
        return enemy;
    }
}

// 장점: 생성 로직 캡슐화, 풀링 통합, 타입별 설정 관리
```

#### Service Locator 패턴 (전역 서비스)

```csharp
// Core - 서비스 로케이터
public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> s_services = new();

    public static void Register<T>(T service) where T : class
    {
        s_services[typeof(T)] = service;
    }

    public static T Get<T>() where T : class
    {
        return s_services.TryGetValue(typeof(T), out var service)
            ? (T)service
            : throw new InvalidOperationException($"Service {typeof(T)} not registered");
    }

    public static void Clear()
    {
        s_services.Clear();
    }
}

// Bootstrap에서 등록
public class GameBootstrap : MonoBehaviour
{
    private void Awake()
    {
        ServiceLocator.Register<IEventBus>(new EventBus());
        ServiceLocator.Register<IAudioService>(new AudioService());
        ServiceLocator.Register<ISaveService>(new SaveService());
    }
}

// 사용
var eventBus = ServiceLocator.Get<IEventBus>();
```

#### 패턴 조합 예시: 웨이브 시스템

```csharp
// WaveService가 여러 패턴을 조합
public class WaveService
{
    private readonly IEnemyFactory _factory;      // Factory
    private readonly IEventBus _eventBus;         // Observer
    private readonly IObjectPool<Enemy> _pool;    // Object Pool

    public void SpawnWave(WaveData data)
    {
        foreach (var spawn in data.Spawns)
        {
            var enemy = _factory.Create(spawn.Type, spawn.Position);
            // 적 사망 시 이벤트 발행 (Observer)
        }

        _eventBus.Publish(new WaveStartedEvent(data.WaveNumber));
    }
}
```

#### 패턴 사용 금지 사항

| 금지 | 이유 | 대안 |
|------|------|------|
| Singleton 남용 | 숨겨진 의존성, 테스트 어려움 | Service Locator + Interface |
| God Object | SRP 위반, 유지보수 어려움 | 책임 분리 |
| Deep Inheritance | 유연성 저하, 결합도 증가 | Composition over Inheritance |
| Premature Pattern | 불필요한 복잡성 | YAGNI - 필요할 때 적용 |

---

## 1. 프로젝트 구조

```
Assets/
├── 01.Scenes/
│   ├── InGame/              # 인게임 씬
│   │   └── GameScene.unity
│   └── OutGame/             # 아웃게임 씬
│       ├── LobbyScene.unity
│       └── ResultScene.unity
│
├── 02.Scripts/
│   ├── InGame/              # ===== 인게임 시스템 =====
│   │   ├── Presentation/    # [Presentation Layer]
│   │   │   ├── Character/   #   플레이어/적 MonoBehaviour
│   │   │   ├── Combat/      #   전투 관련 컴포넌트
│   │   │   ├── Skill/       #   스킬 실행/이펙트
│   │   │   ├── Arena/       #   아레나 경계/스폰
│   │   │   └── UI/          #   인게임 HUD (HP바, 스테이지 등)
│   │   │
│   │   ├── Application/     # [Application Layer]
│   │   │   ├── Service/     #   BattleService, WaveService
│   │   │   └── UseCase/     #   AttackUseCase, SpawnUseCase
│   │   │
│   │   └── Domain/          # [Domain Layer]
│   │       ├── Entity/      #   CharacterEntity, SkillEntity
│   │       ├── Model/       #   DamageModel, StatModel
│   │       └── Interface/   #   IDamageable, ITargetable
│   │
│   ├── OutGame/             # ===== 아웃게임 시스템 =====
│   │   ├── Presentation/    # [Presentation Layer]
│   │   │   ├── Lobby/       #   로비 UI
│   │   │   ├── Result/      #   결과 화면
│   │   │   ├── Shop/        #   상점 UI
│   │   │   └── Settings/    #   설정 UI
│   │   │
│   │   ├── Application/     # [Application Layer]
│   │   │   └── Service/     #   ShopService, SettingsService
│   │   │
│   │   └── Domain/          # [Domain Layer]
│   │       ├── Entity/      #   PlayerProgressEntity
│   │       └── Model/       #   CurrencyModel, UnlockModel
│   │
│   ├── Shared/              # ===== 공유 시스템 =====
│   │   ├── Domain/          # [Domain Layer - 공유]
│   │   │   ├── Interface/   #   IRepository, IEventBus
│   │   │   └── Event/       #   GameEvent 정의
│   │   │
│   │   ├── Infrastructure/  # [Infrastructure Layer]
│   │   │   ├── Repository/  #   SaveRepository, ScoreRepository
│   │   │   ├── Audio/       #   AudioManager
│   │   │   └── Platform/    #   광고, IAP, Analytics
│   │   │
│   │   └── Utility/         # 순수 유틸리티 (확장 메서드 등)
│   │
│   └── Core/                # ===== 코어 시스템 =====
│       ├── Bootstrap/       # 게임 초기화, DI 설정
│       ├── StateMachine/    # 범용 상태 머신
│       └── Pool/            # 오브젝트 풀 시스템
│
├── 03.Prefabs/
│   ├── InGame/              # 인게임 프리팹
│   │   ├── Character/
│   │   ├── Projectile/
│   │   └── Effect/
│   └── OutGame/             # 아웃게임 프리팹
│       └── UI/
│
├── 04.Images/               # 이미지 에셋
├── 05.Sprites/              # 2D 스프라이트 (SPUM 등)
├── 06.Sounds/               # 사운드 에셋
├── 07.Animations/           # 애니메이션
├── 08.Fonts/                # 폰트
├── 09.Materials/            # 머티리얼
├── 10.ScriptableObjects/    # 데이터 에셋
│   ├── InGame/              # 스킬, 적, 웨이브 데이터
│   └── OutGame/             # 상점, 업그레이드 데이터
└── Plugins/                 # 외부 플러그인 (DOTween 등)
```

### 네임스페이스 규칙

```csharp
// InGame 시스템
namespace DungeonRush.InGame.Presentation.Character { }
namespace DungeonRush.InGame.Application.Service { }
namespace DungeonRush.InGame.Domain.Entity { }

// OutGame 시스템
namespace DungeonRush.OutGame.Presentation.Lobby { }
namespace DungeonRush.OutGame.Application.Service { }
namespace DungeonRush.OutGame.Domain.Entity { }

// 공유 시스템
namespace DungeonRush.Shared.Domain.Interface { }
namespace DungeonRush.Shared.Infrastructure.Repository { }

// 코어 시스템
namespace DungeonRush.Core.Bootstrap { }
namespace DungeonRush.Core.Pool { }
```

### 레이어별 참조 규칙

| From \ To | Presentation | Application | Domain | Infrastructure | Core |
|-----------|:------------:|:-----------:|:------:|:--------------:|:----:|
| **Presentation** | ✓ | ✓ | ✓ | ✗ | ✓ |
| **Application** | ✗ | ✓ | ✓ | ✗ | ✓ |
| **Domain** | ✗ | ✗ | ✓ | ✗ | ✗ |
| **Infrastructure** | ✗ | ✗ | ✓ (Interface) | ✓ | ✓ |
| **Core** | ✗ | ✗ | ✗ | ✗ | ✓ |

- ✓: 참조 가능
- ✗: 참조 금지
- Domain은 어떤 외부 레이어도 참조하지 않음 (완전 독립)

---

## 2. 게임 시스템 개요

### 코어 루프
1. 웨이브 시작 → 아레나 가장자리에서 적 사방 스폰
2. 플레이어 자동 이동 (가장 가까운 적 추적) → 자동 공격/스킬 사용
3. 처치 시 XP/드랍 획득 → 임계치 도달 시 레벨업 카드 선택
4. 빌드업(스킬/스탯) → 보스 처치 → 다음 챕터
5. HP 0 → 점수 화면(랭킹 연동) → 재도전

### 아레나 구조
- **고정 아레나**: 일정 크기의 직사각형 전투 영역
- **경계 처리**: 플레이어/적 모두 아레나 밖으로 이동 불가
- **스폰 위치**: 아레나 가장자리 360° 전 방향에서 스폰

### 플레이어 자동 이동
- 가장 가까운 적을 자동 탐지하여 이동
- 공격 사거리 내 진입 시 이동 정지 후 자동 공격
- 타겟 처치 시 다음 가까운 적으로 자동 전환
- 적이 없으면 아레나 중앙으로 복귀

### 스테이지 구조
- **챕터**: 무한 상승 (1, 2, 3...)
- **웨이브**: 챕터당 10개 (1-1 ~ 1-10)
- **일반 웨이브** (1~9): 몬스터 처치 목표 달성 시 다음 웨이브
- **보스 웨이브** (10): 보스 1체 처치

### 적 스폰 규칙
- 아레나 가장자리 랜덤 위치에서 스폰 (360° 전 방향)
- 웨이브별 스폰 수/간격/적 종류 ScriptableObject로 관리
- 적은 플레이어를 향해 이동 (근접 적) 또는 제자리 공격 (원거리 적)

### 템포 설정
| 구분 | 지연 시간 |
|------|----------|
| 웨이브 시작 | 0.5초 |
| 일반 몬스터 스폰 간격 | 0.3~0.5초 |
| 보스 웨이브 진입 | 1.0초 |

---

## 3. 구현 시스템

### 캐릭터 시스템 (`02.Scripts/Character/`)
| 스크립트 | 설명 |
|----------|------|
| `PlayerController.cs` | 플레이어 자동 이동/상태 관리 |
| `PlayerAutoMove.cs` | 가장 가까운 적 탐지 및 자동 접근 |
| `EnemyController.cs` | 적 행동 패턴 (플레이어 추적/공격) |
| `BossController.cs` | 보스 전용 패턴 |
| `CharacterStats.cs` | HP, ATK 등 스탯 데이터 |

### 전투 시스템 (`02.Scripts/Combat/`)
| 스크립트 | 설명 |
|----------|------|
| `AutoBattleController.cs` | 자동 전투 루프 관리 (타겟 → 접근 → 공격) |
| `TargetFinder.cs` | 가장 가까운 적 탐색 |
| `DamageCalculator.cs` | 데미지 계산 (기본/크리티컬/속성) |
| `ProjectileManager.cs` | 투사체 생성 및 관리 |

### 아레나 시스템 (`02.Scripts/Arena/`)
| 스크립트 | 설명 |
|----------|------|
| `ArenaManager.cs` | 아레나 영역/경계 관리 |
| `StageManager.cs` | 챕터/웨이브 진행 관리 |
| `WaveController.cs` | 웨이브별 적 스폰 로직 |
| `SpawnManager.cs` | 아레나 가장자리 360° 스폰 처리 |

### 스킬 시스템 (`02.Scripts/Skill/`)
| 스크립트 | 설명 |
|----------|------|
| `SkillBase.cs` | 스킬 기본 클래스 (추상) |
| `ActiveSkill.cs` | 액티브 스킬 (게일 슬래시 등) |
| `SkillSlotManager.cs` | 스킬 슬롯 관리 (최대 3개) |

### 강화 시스템 (`02.Scripts/Upgrade/`)
| 스크립트 | 설명 |
|----------|------|
| `LevelUpManager.cs` | 레벨업 처리 |
| `CardSelectUI.cs` | 강화 카드 선택 UI |
| `CardData.cs` | 카드 데이터 (ScriptableObject) |
| `RerollSystem.cs` | 리롤 비용 계산 및 처리 |

### UI 시스템 (`02.Scripts/UI/`)
| 스크립트 | 설명 |
|----------|------|
| `HUDManager.cs` | 상단 HUD 통합 관리 |
| `HPBar.cs` | 체력 바 |
| `XPBar.cs` | 경험치 바 |
| `StageDisplay.cs` | 스테이지 표시 (1-1, Boss 등) |
| `SkillButtonUI.cs` | 스킬 버튼 (쿨다운/잠금 상태) |
| `ScoreResultUI.cs` | 게임오버 결과 화면 |
| `RankingUI.cs` | 랭킹 표시 |

---

## 4. 데이터 설계

### 스킬 카드 예시
| 타입 | 이름 | 효과 | 쿨다운 |
|------|------|------|--------|
| 액티브 | 게일 슬래시 | 전방 파동 n회(60%×n), 관통 2 | 8초 |

### 리롤 비용 공식
```
Cost_reroll = ⌈ 40 × (1 + 0.15 × R) × (1 + 0.10 × (Stage-1)) ⌉
- R: 리롤 횟수 (최대 3회)
- Stage: 현재 스테이지
- Base_reroll: 40
```

### 랭킹 정렬 기준
| 우선순위 | 기준 | 정렬 |
|----------|------|------|
| 1 | 최고 도달 스테이지 | 내림차순 |
| 2 | 총 점수 | 내림차순 |
| 3 | 클리어 시간 | 오름차순 |
| 4 | 처치 수 | 내림차순 |

---

## 5. 명명 규칙 (Naming Conventions)

### PascalCase 사용 대상
- 클래스, 구조체, 레코드, 대리자
- 인터페이스: **`I`** 접두사 (`IDamageable`, `ISkillExecutor`)
- 공용 멤버: 속성, 메서드, 이벤트, 공용 필드
- 상수

### camelCase 사용 대상
- 메서드 매개변수, 지역 변수

### 필드 명명 및 접두사

| 대상 | 접두사 | 예시 |
| :--- | :--- | :--- |
| Private 인스턴스 필드 | `_` | `_currentWave` |
| 정적 필드 | `s_` | `s_instance` |
| 스레드 정적 필드 | `t_` | `t_timeSpan` |

### 일반 원칙
- **간결성보다 명확성** 우선
- 연속 밑줄(`__`) 사용 금지
- 단일 문자 이름은 루프 카운터 외 금지

---

## 6. C# 언어 사용 규칙

- **데이터 형식:** 런타임 형식(`System.Int32`) 대신 **언어 키워드**(`int`, `string`) 사용
- **`var` 사용:** 형식을 **명확히 유추할 수 있는 경우에만** 사용
- **문자열 처리:**
  - 짧은 연결: **문자열 보간**(`$"{}"`)
  - 루프 내 대용량: **`StringBuilder`**
- **대리자:** `Func<>` 또는 `Action<>` 사용
- **예외 처리:** 처리할 수 있는 **특정 예외만 catch**, 일반 `Exception` 포괄 금지

---

## 7. 레이아웃 및 주석 규칙

- **들여쓰기:** **4개의 공백** (탭 금지)
- **중괄호:** **Allman 스타일** (여는/닫는 중괄호 별도 줄)
- **코드 밀도:** 한 줄에 하나의 문장/선언
- **주석:** `//` 사용, **별도 줄**에 배치, **대문자로 시작**, **마침표**로 종료
- **XML 주석:** 작성하지 않음 (메서드명/매개변수명이 충분히 설명적이어야 함)

---

## 8. Unity 특화 규칙

### 컴포넌트 캐싱
```csharp
// Good - Start()에서 캐싱
private SpriteRenderer _spriteRenderer;

private void Start()
{
    _spriteRenderer = GetComponent<SpriteRenderer>();
}

// Bad - Update()에서 매 프레임 호출
private void Update()
{
    GetComponent<SpriteRenderer>().color = Color.red; // 금지
}
```

### 오브젝트 풀링 (Object Pooling)
```csharp
// 투사체, 이펙트 등 빈번히 생성/파괴되는 오브젝트는 풀링 필수
public class ProjectilePool : MonoBehaviour
{
    private Queue<Projectile> _pool = new Queue<Projectile>();

    public Projectile Get()
    {
        return _pool.Count > 0 ? _pool.Dequeue() : CreateNew();
    }

    public void Return(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
        _pool.Enqueue(projectile);
    }
}
```

### ScriptableObject 활용
```csharp
// 스킬, 적, 카드 데이터는 ScriptableObject로 관리
[CreateAssetMenu(fileName = "SkillData", menuName = "DungeonRush/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("Basic Info")]
    public string skillName;
    public SkillType skillType;

    [Header("Stats")]
    public float damage;
    public float cooldown;
}
```

### 스크립트 구조
```csharp
public class ExampleScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _exampleValue = 10f;

    private ComponentType _cachedComponent;

    private void Start()
    {
        _cachedComponent = GetComponent<ComponentType>();
    }

    private void Update()
    {
        // 프레임 로직
    }
}
```

### 주의사항
- `.meta` 파일 수정 금지
- MonoBehaviour 클래스는 파일명과 클래스명 일치 필수
- `SerializeField`, `Header` 어트리뷰트 활용
- 에디터 전용 코드는 `#if UNITY_EDITOR` 사용
- 모바일 최적화: GC Alloc 최소화, 풀링 적극 활용

---

## 9. 외부 플러그인

### DOTween
- 애니메이션/트위닝 라이브러리
- UI 전환, 이펙트 연출에 사용
- `DOMove()`, `DOFade()`, `SetEase(Ease.OutCubic)` 등

### SPUM (2D Pixel Unit Maker)
- 2D 픽셀 캐릭터 생성 에셋
- 플레이어/적 스프라이트 제작

---

## 10. Git 브랜치 전략

- `main`: 안정 버전
- `develop`: 개발 통합 브랜치
- `feature/*`: 기능 개발
- `test/*`: 테스트/실험

---

## 11. 인게임 개발 체크리스트

### Phase 1: 코어 전투 (기반) ✅
- [x] 플레이어 FSM (Idle/Move/Combat/Dead)
- [x] 적 FSM (Chase/Attack/Dead)
- [x] 플레이어 자동 이동 (가까운 적 추적, AutoMoveController)
- [x] 타겟 탐색 (TargetFinder, Physics2D.OverlapCircleNonAlloc)
- [x] 체력/데미지 시스템 (HealthComponent, IDamageable)
- [x] 아레나 영역/경계 시스템 (ArenaBoundary, 4벽 콜라이더)
- [x] 기본 적 스폰 (EnemySpawner, 아레나 가장자리 360°)
- [x] 플레이어 스킬 슬롯 시스템 (SkillSlotManager, 최대 3슬롯, 독립 쿨다운)
- [x] 스킬 투사체 시스템 (Projectile, ProjectilePool)
- [x] 스킬 이펙트 풀링 (EffectPool, EffectAutoReturn)

### Phase 2: 적 시스템 강화 ← 현재
- [ ] 적 오브젝트 풀링 (Destroy → EnemyPool 전환)
- [ ] 적 외형 다양화 (SPUM 기반 다종 프리팹)
- [ ] 플레이어 캐릭터 외형 구현 (SPUM)

### Phase 3: 스킬 다양화 (빌드 깊이)
- [ ] 스킬 발사 패턴 다양화 (오라형/궤도형/장판형/전방위형)
- [ ] 스킬 슬롯 확장 설계 (3→4~6슬롯)

### Phase 4: 인-런 성장 (핵심 재미)
- [ ] XP/경험치 드롭 시스템 (적 처치 시 XP 보석 드롭)
- [ ] 레벨업 카드 선택 시스템 (3~4개 선택지 중 택 1, 게임 일시정지)
- [ ] 스킬 레벨업 시스템 (Lv.1→5+ 단계별 스탯 강화)
- [ ] 리롤 시스템 (골드 소모, 최대 3회)
- [ ] 패시브 아이템 시스템 (별도 슬롯, 스탯 보정/간접 효과)
- [ ] 스킬 진화/조합 시스템 (액티브 + 패시브 = 진화 스킬)

### Phase 5: 웨이브/스테이지 (진행 구조)
- [ ] 웨이브 시스템 (챕터당 10웨이브, 1-9 일반 + 10 보스)
- [ ] 적 난이도 스케일링 (웨이브별 체력/속도/밀도 증가)
- [ ] 적 종류 다양화 (근접/원거리/엘리트)
- [ ] 보스 전투 (고유 공격 패턴)

### Phase 6: 메타 진행 (런 간 영구 성장)
- [ ] 영구 화폐 시스템 (골드/보석, 런 종료 후 유지)
- [ ] 영구 스탯 강화 (기본 HP/ATK/속도 영구 업그레이드)
- [ ] 캐릭터 해금 시스템 (캐릭터별 고유 시작 무기/패시브)

### Phase 7: 최적화
- [ ] GC Alloc 최소화 (프로파일링 기반)
- [ ] 모바일 성능 프로파일링 (Android 실기기)
- [ ] 밸런스 튜닝 (파워 곡선/난이도 곡선)