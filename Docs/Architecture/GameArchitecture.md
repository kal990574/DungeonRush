# DungeonRush 게임 아키텍처 설계서

## 1. 시스템 개요

### 1.1 게임 컨셉
- **장르**: Idle + 로그라이크 (자동 전투)
- **플랫폼**: Android (세로, 9:19 · 1080×2280)
- **코어 루프**: 스테이지 → 자동전투 → 레벨업 → 보스 → 반복/사망

### 1.2 아키텍처 원칙
- **SOLID 원칙** 준수
- **디미터의 법칙** 준수 (기차 참사 패턴 금지)
- **단일 책임**: 각 클래스는 하나의 역할만 수행
- **이벤트 기반 통신**: 시스템 간 느슨한 결합

---

## 2. 시스템 아키텍처 다이어그램

```
┌─────────────────────────────────────────────────────────────────┐
│                        PRESENTATION LAYER                        │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────────────┐   │
│  │ HUDManager│ │CardSelectUI│ │ScoreResultUI│ │SkillButtonUI │   │
│  └─────┬────┘ └─────┬────┘ └─────┬────┘ └────────┬─────────┘   │
│        │            │            │               │              │
│        └────────────┴────────────┴───────────────┘              │
│                              │                                   │
│                        UIEventBus                                │
└──────────────────────────────┼───────────────────────────────────┘
                               │
┌──────────────────────────────┼───────────────────────────────────┐
│                        GAME LOGIC LAYER                          │
│                              │                                   │
│  ┌───────────────────────────┴───────────────────────────────┐  │
│  │                      GameEventBus                          │  │
│  └───────────────────────────┬───────────────────────────────┘  │
│                              │                                   │
│  ┌──────────┐ ┌──────────┐ ┌┴─────────┐ ┌──────────┐           │
│  │StageManager│ │BattleManager│ │SkillManager│ │UpgradeManager│   │
│  └─────┬────┘ └─────┬────┘ └─────┬────┘ └─────┬────┘           │
│        │            │            │            │                  │
│  ┌─────┴────┐ ┌─────┴────┐ ┌─────┴────┐ ┌─────┴────┐           │
│  │WaveController│ │AutoBattle │ │SkillSlot │ │LevelUpManager│    │
│  │SpawnManager│ │DamageCalc │ │SkillBase │ │RerollSystem│       │
│  └──────────┘ └──────────┘ └──────────┘ └──────────┘           │
└──────────────────────────────┬───────────────────────────────────┘
                               │
┌──────────────────────────────┼───────────────────────────────────┐
│                        ENTITY LAYER                              │
│                              │                                   │
│  ┌──────────┐ ┌──────────┐ ┌┴─────────┐ ┌──────────┐           │
│  │  Player  │ │  Enemy   │ │   Boss   │ │Projectile│           │
│  │Controller│ │Controller│ │Controller│ │ Manager  │           │
│  └─────┬────┘ └─────┬────┘ └─────┬────┘ └─────┬────┘           │
│        │            │            │            │                  │
│        └────────────┴────────────┴────────────┘                  │
│                              │                                   │
│                    CharacterStats (공통)                         │
└──────────────────────────────┬───────────────────────────────────┘
                               │
┌──────────────────────────────┼───────────────────────────────────┐
│                         DATA LAYER                               │
│                              │                                   │
│  ┌──────────┐ ┌──────────┐ ┌┴─────────┐ ┌──────────┐           │
│  │SkillData │ │EnemyData │ │ CardData │ │StageData │           │
│  │   (SO)   │ │   (SO)   │ │   (SO)   │ │   (SO)   │           │
│  └──────────┘ └──────────┘ └──────────┘ └──────────┘           │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                      INFRASTRUCTURE LAYER                        │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐           │
│  │GameManager│ │ObjectPool│ │SaveSystem│ │AudioManager│          │
│  │(Singleton)│ │ (Generic)│ │          │ │          │           │
│  └──────────┘ └──────────┘ └──────────┘ └──────────┘           │
└─────────────────────────────────────────────────────────────────┘
```

---

## 3. 핵심 시스템 상세 설계

### 3.1 Core 시스템 (`02.Scripts/Core/`)

#### GameManager (싱글톤)
```
역할: 게임 전역 상태 관리
책임:
  - 게임 상태 전환 (Menu, Playing, Paused, GameOver)
  - 게임 초기화/재시작
  - 전역 이벤트 발행
의존성: 없음 (최상위)
```

#### GameEventBus (이벤트 버스)
```
역할: 시스템 간 느슨한 결합을 위한 이벤트 중개
이벤트 목록:
  - OnGameStateChanged(GameState)
  - OnPlayerDeath()
  - OnStageCleared()
  - OnLevelUp(int level)
  - OnEnemyKilled(EnemyData)
  - OnBossDefeated()
```

#### ObjectPool<T> (제네릭 풀)
```
역할: 오브젝트 재사용으로 GC 최소화
사용처:
  - 투사체 (Projectile)
  - 데미지 텍스트 (DamagePopup)
  - 이펙트 (VFX)
  - 적 (Enemy)
```

### 3.2 Stage 시스템 (`02.Scripts/Stage/`)

#### StageManager
```
역할: 챕터/웨이브 진행 총괄
책임:
  - 현재 챕터/웨이브 추적
  - 웨이브 완료 판정
  - 보스 웨이브 전환
상태:
  - currentChapter: int
  - currentWave: int (1~10)
  - killCount: int
  - isBoosWave: bool
```

#### WaveController
```
역할: 웨이브별 적 스폰 로직
책임:
  - 웨이브 시작/종료 처리
  - 스폰 타이밍 제어
  - 목표 처치 수 관리
```

#### SpawnManager
```
역할: 몬스터 실제 생성
책임:
  - 스폰 위치 결정
  - ObjectPool에서 적 가져오기
  - 스폰 이펙트 재생
```

### 3.3 Battle 시스템 (`02.Scripts/Battle/`)

#### AutoBattleController
```
역할: 자동 전투 로직
책임:
  - 타겟 탐색 (가장 가까운 적)
  - 공격 범위 판정
  - 공격 실행 명령
```

#### DamageCalculator (정적 클래스)
```
역할: 데미지 계산
공식:
  - 기본 데미지 = ATK × 스킬배율
  - 크리티컬 = 기본 × 크리티컬 배율
  - 최종 = 기본 - DEF (최소 1)
```

#### ProjectileManager
```
역할: 투사체 관리
책임:
  - 투사체 발사
  - 충돌 판정
  - 관통 처리
```

### 3.4 Character 시스템 (`02.Scripts/Character/`)

#### 인터페이스 정의
```csharp
interface IDamageable
{
    void TakeDamage(float damage, DamageType type);
    bool IsDead { get; }
}

interface IAttacker
{
    float AttackPower { get; }
    void Attack(IDamageable target);
}
```

#### CharacterStats (공통 스탯)
```
속성:
  - MaxHP / CurrentHP
  - ATK (공격력)
  - DEF (방어력)
  - CritRate (크리티컬 확률)
  - CritDamage (크리티컬 배율)
  - AttackSpeed (공격 속도)
  - MoveSpeed (이동 속도)
```

#### PlayerController
```
역할: 플레이어 상태 관리
책임:
  - 스탯 관리
  - 레벨/경험치 관리
  - 스킬 슬롯 연동
```

#### EnemyController
```
역할: 일반 적 행동
책임:
  - 플레이어 추적
  - 공격 패턴 실행
  - 사망 시 보상 드랍
```

#### BossController (EnemyController 상속)
```
역할: 보스 전용 행동
추가 책임:
  - 다단계 패턴
  - 체력 페이즈
  - 특수 스킬
```

### 3.5 Skill 시스템 (`02.Scripts/Skill/`)

#### SkillBase (추상 클래스)
```csharp
abstract class SkillBase
{
    SkillData Data { get; }
    float CooldownRemaining { get; }
    bool IsReady { get; }

    abstract void Execute(CharacterStats caster, IDamageable target);
    void StartCooldown();
}
```

#### 스킬 타입 계층
```
SkillBase (추상)
├── ActiveSkill (데미지/투사체)
│   ├── GaleSlash (게일 슬래시)
│   └── ... (추가 액티브)
├── BuffSkill (버프 부여)
│   ├── FireBuff (화염 부여)
│   ├── PoisonBuff (맹독 부여)
│   └── LifestealBuff (흡혈)
└── UltimateSkill (궁극기)
    └── ValorStrike (용맹의 일격)
```

#### SkillSlotManager
```
역할: 스킬 슬롯 관리
제한: 최대 3개 슬롯
책임:
  - 스킬 장착/해제
  - 자동 스킬 사용 순서
  - 쿨다운 UI 연동
```

### 3.6 Upgrade 시스템 (`02.Scripts/Upgrade/`)

#### LevelUpManager
```
역할: 레벨업 처리
책임:
  - 경험치 임계치 계산
  - 레벨업 이벤트 발행
  - 카드 선택 UI 호출
```

#### CardSelectSystem
```
역할: 강화 카드 시스템
책임:
  - 랜덤 카드 3장 선택
  - 카드 적용
  - 리롤 처리
```

#### RerollSystem
```
역할: 리롤 비용 계산
공식: Cost = ⌈40 × (1 + 0.15×R) × (1 + 0.10×(Stage-1))⌉
제한: 최대 3회
```

### 3.7 UI 시스템 (`02.Scripts/UI/`)

#### UI 계층 구조
```
Canvas (Screen Space - Overlay)
├── HUD Layer (상시 표시)
│   ├── HPBar
│   ├── XPBar
│   ├── StageDisplay
│   └── SkillButtons (x3)
├── Popup Layer (레벨업/결과)
│   ├── CardSelectPopup
│   └── ScoreResultPopup
└── Overlay Layer (페이드/로딩)
```

#### HUDManager
```
역할: HUD 요소 통합 관리
책임:
  - HP/XP 바 업데이트
  - 스테이지 표시 갱신
  - 스킬 버튼 상태 동기화
```

---

## 4. 데이터 구조 (ScriptableObject)

### 4.1 SkillData
```csharp
[CreateAssetMenu(menuName = "DungeonRush/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("기본 정보")]
    public string skillId;
    public string skillName;
    public SkillType skillType;  // Active, Buff, Ultimate
    public Sprite icon;

    [Header("스탯")]
    public float damageMultiplier;
    public float cooldown;
    public int projectileCount;
    public int penetration;

    [Header("버프 전용")]
    public float duration;
    public float tickDamage;
    public int maxStacks;
}
```

### 4.2 EnemyData
```csharp
[CreateAssetMenu(menuName = "DungeonRush/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("기본 정보")]
    public string enemyId;
    public string enemyName;
    public EnemyType enemyType;  // Normal, Elite, Boss
    public GameObject prefab;

    [Header("스탯")]
    public float baseHP;
    public float baseATK;
    public float baseDEF;
    public float moveSpeed;
    public float attackRange;

    [Header("보상")]
    public int xpReward;
    public int goldReward;

    [Header("스케일링")]
    public float hpPerChapter;
    public float atkPerChapter;
}
```

### 4.3 CardData
```csharp
[CreateAssetMenu(menuName = "DungeonRush/Card Data")]
public class CardData : ScriptableObject
{
    [Header("기본 정보")]
    public string cardId;
    public string cardName;
    public string description;
    public CardRarity rarity;  // Common, Rare, Epic, Legendary
    public Sprite icon;

    [Header("효과")]
    public CardEffectType effectType;
    public float effectValue;
    public SkillData linkedSkill;  // 스킬 카드인 경우
}
```

### 4.4 StageData
```csharp
[CreateAssetMenu(menuName = "DungeonRush/Stage Data")]
public class StageData : ScriptableObject
{
    [Header("챕터 설정")]
    public int chapterNumber;
    public string chapterName;
    public Sprite background;

    [Header("웨이브 설정")]
    public int wavesPerChapter = 10;
    public int enemiesPerWave = 10;

    [Header("적 풀")]
    public EnemyData[] normalEnemies;
    public EnemyData bossEnemy;

    [Header("난이도 스케일링")]
    public float difficultyMultiplier;
}
```

---

## 5. 게임 흐름 (Game Flow)

### 5.1 상태 머신
```
[Menu] ──시작버튼──▶ [Playing] ──HP=0──▶ [GameOver]
                        │                    │
                        │                    │
                    레벨업              재시작버튼
                        │                    │
                        ▼                    │
                   [LevelUp] ────카드선택────┘
                        │
                        │
                    카드선택완료
                        │
                        ▼
                   [Playing]
```

### 5.2 웨이브 진행 흐름
```
웨이브 시작 (0.5초 딜레이)
      │
      ▼
적 스폰 (0.5초 간격 × 10마리) ◀──────┐
      │                              │
      ▼                              │
자동 전투 진행                        │
      │                              │
      ▼                              │
적 처치 ──목표 미달성──────────────────┘
      │
      │ 목표 달성 (10마리)
      ▼
웨이브 완료
      │
      ├── 웨이브 1~9: 다음 웨이브
      │
      └── 웨이브 10: 보스 웨이브 (1.0초 딜레이)
                │
                ▼
          보스 스폰
                │
                ▼
          보스 전투
                │
                ▼
          보스 처치 ──▶ 다음 챕터
```

---

## 6. 파일 구조 최종안

```
Assets/02.Scripts/
├── Core/
│   ├── GameManager.cs
│   ├── GameEventBus.cs
│   ├── GameState.cs
│   ├── Singleton.cs
│   └── ObjectPool.cs
│
├── Battle/
│   ├── AutoBattleController.cs
│   ├── DamageCalculator.cs
│   ├── DamageType.cs
│   └── ProjectileManager.cs
│
├── Character/
│   ├── Interfaces/
│   │   ├── IDamageable.cs
│   │   └── IAttacker.cs
│   ├── CharacterStats.cs
│   ├── PlayerController.cs
│   ├── EnemyController.cs
│   └── BossController.cs
│
├── Stage/
│   ├── StageManager.cs
│   ├── WaveController.cs
│   └── SpawnManager.cs
│
├── Skill/
│   ├── SkillBase.cs
│   ├── SkillType.cs
│   ├── ActiveSkill.cs
│   ├── BuffSkill.cs
│   ├── UltimateSkill.cs
│   └── SkillSlotManager.cs
│
├── Upgrade/
│   ├── LevelUpManager.cs
│   ├── CardSelectSystem.cs
│   ├── RerollSystem.cs
│   └── CardRarity.cs
│
└── UI/
    ├── HUDManager.cs
    ├── HPBar.cs
    ├── XPBar.cs
    ├── StageDisplay.cs
    ├── SkillButtonUI.cs
    ├── CardSelectUI.cs
    └── ScoreResultUI.cs
```

---

## 7. 구현 우선순위

### Phase 1: Core Foundation
1. Singleton, ObjectPool
2. GameManager, GameEventBus
3. CharacterStats, IDamageable

### Phase 2: Basic Gameplay
4. PlayerController
5. EnemyController
6. AutoBattleController
7. DamageCalculator

### Phase 3: Stage System
8. StageManager
9. WaveController
10. SpawnManager

### Phase 4: Skill System
11. SkillBase, SkillSlotManager
12. ActiveSkill (기본 공격)
13. BuffSkill, UltimateSkill

### Phase 5: Progression
14. LevelUpManager
15. CardSelectSystem
16. RerollSystem

### Phase 6: UI & Polish
17. HUD 요소들
18. 팝업 UI
19. 이펙트, 사운드

---

## 8. 설계 검증 체크리스트

### SOLID 원칙 준수
- [x] **SRP**: 각 클래스 단일 책임 (Manager별 역할 분리)
- [x] **OCP**: 스킬 시스템 확장 가능 (SkillBase 상속)
- [x] **LSP**: EnemyController/BossController 치환 가능
- [x] **ISP**: IDamageable, IAttacker 분리
- [x] **DIP**: 이벤트 버스를 통한 느슨한 결합

### 디미터의 법칙 준수
- [x] Manager간 직접 참조 대신 이벤트 사용
- [x] UI는 EventBus 구독으로 데이터 수신
- [x] 체인 호출 패턴 회피