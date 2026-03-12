# DungeonRush — 스탯 데이터 시스템 개발 명세서 v3

> CSV 기반 데이터로 기존 SO 참조를 대체하고, 런타임 스탯 변동을 관리하는 시스템.

---

## 1. 현재 코드 분석 — SO 의존 지점

### 1-1. 현재 SO 3종과 사용처

```
PlayerStatData SO (3개 필드)
├── maxHp: 200        → PlayerController.InitializeStats() → _health.Initialize(maxHp)
├── moveSpeed: 4      → PlayerController.InitializeStats() → _autoMove.Initialize(moveSpeed)
└── detectRange: 15   → PlayerController.InitializeStats() → _targetFinder.Initialize(detectRange)

EnemyStatData SO (5개 필드)
├── maxHp: 50         → EnemyController.InitializeStats() → _health.Initialize(maxHp)
├── moveSpeed: 2      → EnemyController.InitializeStats() → _autoMove.Initialize(moveSpeed)
├── attackDamage: 5   → EnemyController.InitializeStats() → _attack.Initialize(damage, ...)
├── attackRange: 1.2  → EnemyController.InitializeStats() → _attack.Initialize(..., range, ...)
└── attackCooldown: 1.5→ EnemyController.InitializeStats() → _attack.Initialize(..., cooldown)

SkillData SO (8개 필드)
├── skillName          → SkillExecutor 내부 풀 이름
├── skillType          → SkillExecutor.Execute() 분기 (Melee/Projectile)
├── damage: 10         → SkillExecutor.ExecuteMelee() → damageable.TakeDamage(damage)
│                        SkillExecutor.ExecuteProjectile() → projectile.Initialize(damage, ...)
├── range: 1.5         → SkillSlot.TryExecute() 사거리 판정
│                        SkillExecutor.ExecuteProjectile() → projectile.Initialize(..., range, ...)
├── cooldown: 1        → SkillSlot.TryExecute() 쿨다운 리셋
├── projectilePrefab   → SkillExecutor.InitializePools() 풀 생성
├── projectileSpeed: 8 → SkillExecutor.ExecuteProjectile() → projectile.Initialize(..., speed, ...)
└── hitEffectPrefab    → SkillExecutor.InitializePools() 이펙트 풀 생성
```

### 1-2. 중요 발견: 하류 컴포넌트는 이미 SO에서 디커플링됨

아래 컴포넌트들은 SO를 직접 참조하지 않고, float 값만 받는다:

| 컴포넌트 | 초기화 방식 | SO 의존 |
|---------|-----------|---------|
| HealthComponent | `Initialize(float maxHp)` | ❌ 없음 |
| AutoMoveController | `Initialize(float moveSpeed)` | ❌ 없음 |
| TargetFinder | `Initialize(float detectRange)` | ❌ 없음 |
| AttackController | `Initialize(float damage, float range, float cooldown)` | ❌ 없음 |
| Projectile | `Initialize(float damage, float speed, float maxRange, ...)` | ❌ 없음 |

**SO 의존은 Controller 레벨에 집중**:
- `PlayerController._statData` (PlayerStatData SO) — `[SerializeField]`로 Inspector 주입
- `EnemyController._statData` (EnemyStatData SO) — `[SerializeField]`로 Inspector 주입
- `SkillSlotManager._initialSkills` (SkillData[] SO) — `[SerializeField]`로 Inspector 주입
- `SkillSlot._data` / `SkillExecutor._data` — SkillData SO 직접 참조 (damage, range, cooldown 등)

### 1-3. 교체 전략

Controller의 `InitializeStats()`에서 SO 대신 새 시스템에서 값을 가져오게 바꾸면 된다. 하류 컴포넌트는 수정 불필요.

```csharp
// Before (현재):
private void InitializeStats()
{
    _health.Initialize(_statData.maxHp);          // SO에서 직접
    _targetFinder.Initialize(_statData.detectRange);
    _autoMove.Initialize(_statData.moveSpeed);
}

// After (교체 후):
private void InitializeStats()
{
    var spec = gameData.GetCharacter("Player_001");
    _health.Initialize(spec.Stats.HP);                 // GameData에서
    _targetFinder.Initialize(spec.Stats.PickupRange);
    _autoMove.Initialize(spec.Stats.MoveSpeed);
}
```

SkillSlot/SkillExecutor는 SkillData SO를 내부적으로 참조하는 구조라, 이 부분은 새 SkillSpec으로 교체하거나 어댑터로 감싸야 한다. 단, **이번 작업에서는 데이터 시스템만 만들고, 기존 코드 교체는 다음 단계**에서 진행.

---

## 2. 작업 범위

### 한다
- CSV 2종(캐릭터, 스킬) 파싱 → 정적 데이터 로드 (`GameData`)
- 런타임 스탯 상태 관리 (`RunState`) — 소스별 분해 조회 포함
- 기존 SO가 제공하던 모든 데이터를 새 시스템에서도 제공할 수 있는 구조
- 인터페이스 기반 데이터 소스 추상화 (향후 DB 교체 대비)

### 안 한다
- 기존 Controller/SkillSlot 코드의 SO 참조를 새 시스템으로 교체하는 작업 (이번 범위 밖)
- UI 구현, 전투 로직, 씬 전환
- 룬/아이템 Definition CSV (아직 기획자 미제공)

---

## 3. 두 계층 구조

```
┌──────────────────────────────────────────────────────────────┐
│                         소비자                                │
│                                                                │
│  PlayerController    EnemyController    SkillSlotManager       │
│  (InitializeStats)   (InitializeStats)  (SkillSlot/Executor)  │
│  HealthComponent     AttackController   Projectile    UI ...   │
│       │                    │                │          │        │
│  "캐릭터 스펙?"      "적 스펙?"       "스킬 스펙?"   "최종 ATK?" │
└───────┼────────────────────┼────────────────┼──────────┼───────┘
        │                    │                │          │
        ▼                    ▼                ▼          ▼
   ┌─────────────┐                     ┌──────────────────┐
   │  GameData    │                     │    RunState       │
   │  (정적 스펙)  │◄────────────────────│  (런타임 상태)     │
   │              │   Spec 참조하여       │                    │
   │  CSV→불변    │   수치 계산          │  런 동안 가변       │
   └──────┬──────┘                     └──────────────────┘
          │
   ┌──────▼──────┐
   │  Repository  │
   │ (CSV 파싱)   │
   └─────────────┘
```

---

## 4. GameData — 정적 데이터 (기존 SO 대체)

### 4-1. 데이터 모델

```csharp
// ── 캐릭터 기본 스탯 (CSV의 Base~ 컬럼 15개를 명시적 필드로 정의) ──
public class BaseStats
{
    // 공격
    public float ATK;              // BaseATK        — 기본 공격력
    public float CDR;              // BaseCDR        — 쿨타임 감소율
    public float CritRate;         // BaseCritRate   — 크리티컬 확률
    public float CritDamage;       // BaseCritDamage — 크리티컬 배율

    // 방어
    public float HP;               // BaseHP         — 최대 체력
    public float HPRegen;          // BaseHPRegen    — 초당 HP 회복
    public float Armor;            // BaseArmor      — 피해 감소율
    public float Evasion;          // BaseEvasion    — 회피율

    // 유틸/파밍
    public float MoveSpeed;        // BaseMoveSpeed    — 이동속도 배율
    public float PickupRange;      // BasePickupRange  — 습득 범위
    public float ExpGain;          // BaseExpGain      — XP 배율
    public float GoldGain;         // BaseGoldGain     — 골드 배율
    public float Luck;             // BaseLuck         — 행운

    // 특수
    public float Lifesteal;        // BaseLifesteal  — 흡혈
    public float Thorns;           // BaseThorns     — 반사 데미지
}
// CSV 파싱 시 "Base" 접두어를 제거하고 필드명에 직접 매핑:
//   BaseATK → ATK, BaseCritRate → CritRate, BaseHP → HP ...

// ── 캐릭터 스펙 (기존 PlayerStatData SO 대체) ──
public class CharacterSpec
{
    public string Id;                    // "Player_001"
    public string Name;
    public int Grade;
    public string ModelPrefab;
    public BaseStats Stats;              // 15개 기본 스탯
    public string StartActiveSkillId;    // "SK_A_001"
    public string StartPassiveSkillId;   // nullable
    public string WeaponTypeTag;
    public string CharacterTraitId;
}
// 기존 PlayerStatData와 대응:
//   PlayerStatData.maxHp       → CharacterSpec.Stats.HP
//   PlayerStatData.moveSpeed   → CharacterSpec.Stats.MoveSpeed
//   PlayerStatData.detectRange → CharacterSpec.Stats.PickupRange
//   (+ ATK, CritRate, Armor 등 12개 추가)


// ── 스킬 스펙 (기존 SkillData SO 대체 + 기획안 확장) ──
public class SkillSpec
{
    // 공통 (기존 SkillData의 skillName, skillType에 대응)
    public string Id;                    // "SK_A_001"
    public string Name;
    public int SkillType;                // 0=Active, 1=Passive
    public string SubType;               // "Projectile", "Aura", ...
    public int MaxLevel;
    public int SkillGroup;
    public string Grade;
    public float Rate;
    public string[] LinkedEffectGroupIds;
    public string DescKey, IconPath, PrefabPath;
    public string CastVFXPath, HitVFXPath, SFXPath;
    public string WeaponTagReq;

    // 공격용 (기존 SkillData의 damage/range/cooldown/projectileSpeed에 대응 + 확장)
    public SkillAttackData Attack;       // nullable

    // 패시브용
    public SkillPassiveData Passive;     // nullable
}

public class SkillAttackData
{
    // 기존 SkillData 대응:
    //   SkillData.damage → SkillCoef (스킬 계수로 변환, 실 데미지 = 캐릭터ATK × SkillCoef)
    //   SkillData.cooldown → BaseCoolTime
    //   SkillData.range → BaseRange
    //   SkillData.projectileSpeed → BaseProjSpeed
    public float SkillCoef;
    public float BaseCoolTime;
    public bool CritEnabled, LifestealEnabled;
    public float BaseRange, BaseRadius, BaseKnockback;
    public int PierceCount, ChainCount, BounceCount;
    public int BaseProj;
    public float BaseProjSpeed;
    public string FirePattern;
    public float BaseDuration, TickInterval, TickCoef;
    public int MaxStack, StackRule;
}

public class SkillPassiveData
{
    public string EffectType;
    public string TargetStat;            // "ATK", "HP" 등 — BaseStats 필드명과 동일
    public string ModifyType;            // "Add", "Mult", "Override"
    public float ModifyValue;
    public string ApplyScope;
    public string ApplySkillTag, ApplySkillId;
    public string TriggerType;
    public float TriggerChance, TriggerCoolTime, BuffDuration;
    public int StackLimit;
    public string PassivePrefabPath;
}

// ── 스킬 레벨업 데이터 ──
public class SkillLevelUpData
{
    public string SkillId;
    public int Level;
    public List<ParamModification> Modifications;
    public string UITextKey;
}

public class ParamModification
{
    public string ParamName;     // "SkillCoef", "BaseCoolTime" 등
    public int ValueType;        // 0=Set, 1=Add, 2=Mult
    public float Value;
}
```

### 4-2. GameData API

```csharp
public class GameData
{
    void Load(ICharacterRepository charRepo, ISkillRepository skillRepo);

    // 캐릭터
    CharacterSpec GetCharacter(string id);
    IReadOnlyList<CharacterSpec> GetAllCharacters();

    // 스킬
    SkillSpec GetSkill(string id);
    IReadOnlyList<SkillSpec> GetSkillsByGroup(int groupId);

    // 레벨업 테이블
    IReadOnlyList<SkillLevelUpData> GetSkillLevelUps(string skillId);

    // 편의: 특정 레벨에서 스킬 파라미터 최종값
    // (Lv.1 기본값에 Lv.2~targetLevel 엔트리를 순차 적용)
    float GetSkillParam(string skillId, string paramName, int level);
}
```

### 4-3. Repository — CSV 파싱 추상화

```csharp
public interface ICharacterRepository { List<CharacterSpec> LoadAll(); }
public interface ISkillRepository
{
    List<SkillSpec> LoadAllSkills();
    List<SkillLevelUpData> LoadAllLevelUps();
}

// 현재: CSV 구현
public class CsvCharacterRepository : ICharacterRepository { ... }
public class CsvSkillRepository : ISkillRepository { ... }

// 향후: DB 서버로 교체 시 인터페이스만 새로 구현
```

### 4-4. 기존 SO → GameData 대응 관계

| 기존 SO | 기존 필드 | GameData 대응 |
|---------|----------|--------------|
| `PlayerStatData` | `.maxHp` | `GetCharacter(id).Stats.HP` |
| | `.moveSpeed` | `GetCharacter(id).Stats.MoveSpeed` |
| | `.detectRange` | `GetCharacter(id).Stats.PickupRange` |
| `SkillData` | `.damage` | `GetSkill(id).Attack.SkillCoef` (× 캐릭터ATK) |
| | `.range` | `GetSkill(id).Attack.BaseRange` |
| | `.cooldown` | `GetSkill(id).Attack.BaseCoolTime` |
| | `.projectileSpeed` | `GetSkill(id).Attack.BaseProjSpeed` |
| | `.skillType` | `GetSkill(id).SkillType` |
| | `.projectilePrefab` | `GetSkill(id).PrefabPath` (경로 기반 로드) |
| | `.hitEffectPrefab` | `GetSkill(id).HitVFXPath` (경로 기반 로드) |

---

## 5. RunState — 런타임 스탯 관리

런 시작~종료 동안 존재. 장착 상태를 추적하고, GameData를 참조해서 최종 스탯을 계산.

### 5-1. 데이터 모델

```csharp
public class RunState
{
    public string CharacterId;
    public int Level;

    public List<EquippedWeapon> Weapons;          // 무기 슬롯 (최대 4)
    public List<EquippedRune> Runes;              // 룬 슬롯 (최대 4)
    public Dictionary<string, int> Items;         // 아이템ID → 중첩수

    internal StatSheet StatSheet;                 // 캐릭터 전역 스탯 합산기
    internal Dictionary<int, WeaponResolvedStats> WeaponStats;
}

public class EquippedWeapon { public int SlotIndex; public string SkillId; public int CurrentLevel; }
public class EquippedRune   { public int SlotIndex; public string RuneId;  public int CurrentLevel; }
```

### 5-2. StatSheet — 소스별 합산 + 분해 조회

```csharp
public class StatSheet
{
    // 소스별 레이어: "CharacterBase", "Rune:힘의룬_Lv1", "Item:자석_x3" 등
    Dictionary<string, List<StatModifier>> layers;

    // 런 시작 시 — BaseStats의 각 필드를 리플렉션 또는 수동 매핑으로 Flat modifier로 등록
    // 예: baseStats.ATK=25 → StatModifier { SourceKey="CharacterBase", StatKey="ATK", Flat, 25 }
    void SetBase(BaseStats baseStats);
    void AddModifier(StatModifier mod);
    void RemoveModifiers(string sourceKey);

    float GetFinal(string statKey);                        // "ATK" → 28.0
    StatBreakdown GetBreakdown(string statKey);            // 소스별 분해
}

// StatModifier의 StatKey는 BaseStats의 필드명과 동일한 문자열을 사용.
// 정적 데이터 쪽은 명시적 필드(BaseStats.ATK), 런타임 합산 쪽은 string key("ATK")
// → SetBase()에서 변환, 이후 룬/아이템 등 동적 소스도 같은 키로 합산.
public class StatModifier
{
    public string SourceKey;     // "CharacterBase", "Rune:힘의룬_Lv2"
    public string SourceLabel;   // UI용: "기본", "힘의 룬 Lv.2"
    public string StatKey;       // "ATK", "HP" 등 — BaseStats 필드명과 동일
    public string ModifyType;    // "Flat", "Percent"
    public float Value;
}

// 계산: flatSum × (1 + percentSum), 캡 적용

public class StatBreakdown
{
    public string StatKey;
    public float BaseValue, FinalValue;
    public List<StatBreakdownEntry> Entries;
}

public class StatBreakdownEntry
{
    public string SourceLabel;
    public string ModifyType;
    public float RawValue, Contribution;
}
```

### 5-3. WeaponResolvedStats — 무기 최종 스펙

```csharp
public class WeaponResolvedStats
{
    public int SlotIndex;
    public string SkillId;
    public int Level;
    public float FinalSkillCoef, FinalCoolTime;
    public int FinalProjCount;
    public float FinalProjSpeed, FinalRange, FinalKnockback;
    public int FinalPierceCount, FinalChainCount, FinalBounceCount;
    public bool IsDirty;
}
// 계산: 스킬 기본값 → 레벨업 누적(Set/Add/Mult) → StatSheet 전역 보너스 ->캐릭터 CDR 적용
```

### 5-4. RunState API

```csharp
public class RunState
{
    RunState(GameData gameData);

    // 런 생명주기
    void StartRun(string characterId);  // StatSheet 기본값 세팅 + 시작 무기 장착
    void EndRun();                      // 전부 초기화

    // 장착/강화
    void EquipWeapon(int slot, string skillId);
    void LevelUpWeapon(int slot);
    void EquipRune(int slot, string runeId);
    void LevelUpRune(int slot);
    void AddItem(string itemId, int count = 1);

    // 스탯 조회
    float GetStat(string statKey);
    StatBreakdown GetStatBreakdown(string statKey);
    WeaponResolvedStats GetWeaponStats(int slot);

    // 커스텀 Modifier
    void AddModifier(StatModifier mod);
    void RemoveModifiers(string sourceKey);

    // 이벤트
    event Action<string> OnStatChanged;
    event Action<int> OnWeaponChanged;
}
```

---

## 6. 사용 시나리오

```csharp
// ─── 로비: 캐릭터 선택 (GameData만) ───
var wizard = gameData.GetCharacter("Player_001");
// wizard.Stats.ATK == 25
// wizard.Stats.HP == 120  (← 기존 PlayerStatData.maxHp)

var fireball = gameData.GetSkill("SK_A_001");
// fireball.Attack.BaseCoolTime == 1.5  (← 기존 SkillData.cooldown)

// ─── 런 시작 ───
runState.StartRun("Player_001");

// ─── 전투: 기존 코드가 필요한 값을 얻는 방식 ───
// Before: _statData.maxHp         → After: runState.GetStat("HP")       — string key로 조회
// Before: _statData.moveSpeed     → After: runState.GetStat("MoveSpeed")
// Before: _data.damage (SkillData)→ After: runState.GetWeaponStats(0).FinalSkillCoef × runState.GetStat("ATK")

// ─── 레벨업 → 무기 강화 ───
runState.LevelUpWeapon(0);  // 파이어볼 Lv.1→2
// 내부: GameData에서 레벨업 테이블 조회 → WeaponResolvedStats 재계산

// ─── 룬 장착 ───
runState.EquipRune(0, "Rune_Power");
// 내부: StatSheet에 Modifier 추가 → OnStatChanged("ATK") 발행

// ─── UI: 스탯 분해 ───
var bd = runState.GetStatBreakdown("ATK");
// "기본 25 + 힘의 룬 +8% = 27.0"

// ─── 사망 ───
runState.EndRun();
```

---

## 7. 폴더 구조

```
Assets/02.Scripts/Stats/
├── Data/                          ← 정적 데이터 (기존 SO 대체)
│   ├── BaseStats.cs               ← 캐릭터 기본 스탯 15개 명시적 필드
│   ├── CharacterSpec.cs
│   ├── SkillSpec.cs               (SkillAttackData, SkillPassiveData 포함)
│   ├── SkillLevelUpData.cs        (ParamModification 포함)
│   └── GameData.cs
│
├── Runtime/                       ← 런타임 상태 (런 중 가변)
│   ├── RunState.cs
│   ├── EquippedWeapon.cs
│   ├── EquippedRune.cs
│   ├── StatSheet.cs               (StatModifier 포함)
│   ├── StatBreakdown.cs           (StatBreakdownEntry 포함)
│   └── WeaponResolvedStats.cs
│
├── Repository/                    ← 데이터 소스 추상화
│   ├── ICharacterRepository.cs
│   ├── ISkillRepository.cs
│   └── Csv/
│       ├── CsvCharacterRepository.cs
│       ├── CsvSkillRepository.cs
│       └── CsvParser.cs
│
└── Config/
    └── StatConfig.cs              ← 캡 값, CSV 경로
```

---

## 8. CSV 파싱 주의사항

**Player CSV**: 1행 빈 행, 2행 헤더, 컬럼A 비어있음(B부터 데이터). `Base` 접두어 strip → stat key. Grade에 줄바꿈 포함 가능.

**스킬 CSV**: 좌우 2개 테이블 공존 → 컬럼 인덱스로 분리. 컬럼A=카테고리 구분자. 배열: LinkedEffectGroupID는 세미콜론, 레벨업 배열은 콤마+공백. bool은 `TRUE`/`FALSE` 문자열. SkillID 빈 행 스킵.

**파서**: RFC 4180 (quoted fields, 줄바꿈), BOM 처리.

---

## 9. 구현 순서 (Claude Code용)

### Phase 1: 정적 데이터
1. `BaseStats`, `CharacterSpec`, `SkillSpec`(AttackData, PassiveData), `SkillLevelUpData`(ParamModification)
2. `ICharacterRepository`, `ISkillRepository` 인터페이스
3. `CsvParser` → `CsvCharacterRepository`, `CsvSkillRepository`
4. `GameData` (Repository 주입, Dictionary 캐싱, `GetSkillParam()` 레벨 누적)

### Phase 2: 런타임 상태
5. `StatModifier`, `StatSheet`, `StatBreakdown`
6. `EquippedWeapon`, `EquippedRune`, `WeaponResolvedStats`
7. `RunState` (StartRun/EndRun, 장착/레벨업, GetStat/GetBreakdown, GetWeaponStats, Dirty Flag, 이벤트)

### Phase 3: 검증
8. CSV 파싱 → GameData 조회, 스킬 레벨 누적, StatSheet 합산+분해, 전체 시나리오

---

## 10. 체크리스트

- [ ] GameData가 기존 SO의 모든 데이터를 제공할 수 있음 (섹션 4-4 대응표 기준)
- [ ] CSV는 StreamingAssets 또는 Resources에서 읽기 (Editor 우선)
- [ ] 데이터 모델은 순수 C# (MonoBehaviour X)
- [ ] Repository 인터페이스 의존 (DB 교체 가능)
- [ ] 정적 데이터: `BaseStats` 클래스에 15개 스탯을 명시적 필드로 정의
- [ ] 런타임 데이터: StatSheet/StatModifier는 string key("ATK", "HP" 등) — 동적 소스 대응
- [ ] StatSheet 소스별 분해 조회 정상 동작
- [ ] WeaponResolvedStats 레벨업 누적 + CDR 적용 정상 동작
- [ ] Dirty Flag: 무의미한 재계산 방지

---

## 부록: 기존 코드 SO 참조 위치 (향후 교체 시 참고)

| 파일 | SO 필드 | 사용 위치 |
|------|---------|----------|
| `PlayerController.cs:69` | `_statData.maxHp` | `_health.Initialize()` |
| `PlayerController.cs:70` | `_statData.detectRange` | `_targetFinder.Initialize()` |
| `PlayerController.cs:71` | `_statData.moveSpeed` | `_autoMove.Initialize()` |
| `EnemyController.cs:83` | `_statData.maxHp` | `_health.Initialize()` |
| `EnemyController.cs:84` | `_statData.moveSpeed` | `_autoMove.Initialize()` |
| `EnemyController.cs:85` | `_statData.attackDamage/Range/Cooldown` | `_attack.Initialize()` |
| `SkillSlotManager.cs:13` | `_initialSkills[]` (SkillData SO 배열) | `AddSkill()` → SkillSlot/Executor 생성 |
| `SkillSlot.cs:34` | `_data.range` | 사거리 판정 |
| `SkillSlot.cs:37` | `_data.cooldown` | 쿨다운 리셋 |
| `SkillExecutor.cs:37` | `_data.damage` | Melee 데미지 |
| `SkillExecutor.cs:44` | `_data.damage/projectileSpeed/range` | 투사체 초기화 |
| `SkillExecutor.cs:65` | `_data.projectilePrefab` | 투사체 풀 생성 |
| `SkillExecutor.cs:75` | `_data.hitEffectPrefab` | 이펙트 풀 생성 |

> 하류 컴포넌트(HealthComponent, AttackController, Projectile 등)는 이미 float 파라미터만 받으므로 수정 불필요.
