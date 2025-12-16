# DungeonRush 데이터 구조 설계

## 1. ScriptableObject 구조

### 1.1 폴더 구조

```
Assets/10.ScriptableObjects/
├── Skills/
│   ├── Active/
│   │   └── SK_GaleSlash.asset
│   ├── Buff/
│   │   ├── SK_FireBuff.asset
│   │   ├── SK_PoisonBuff.asset
│   │   └── SK_LifestealBuff.asset
│   └── Ultimate/
│       └── SK_ValorStrike.asset
│
├── Enemies/
│   ├── Normal/
│   │   ├── EN_Slime.asset
│   │   ├── EN_Goblin.asset
│   │   └── EN_Skeleton.asset
│   ├── Elite/
│   │   └── EN_OrcWarrior.asset
│   └── Boss/
│       ├── EN_Boss_Golem.asset
│       └── EN_Boss_Dragon.asset
│
├── Cards/
│   ├── Stat/
│   │   ├── CD_ATKUp.asset
│   │   ├── CD_HPUp.asset
│   │   └── CD_CritUp.asset
│   └── Skill/
│       ├── CD_GaleSlash.asset
│       └── CD_FireBuff.asset
│
├── Stages/
│   ├── ST_Chapter1.asset
│   ├── ST_Chapter2.asset
│   └── ST_Chapter3.asset
│
└── Config/
    ├── GameConfig.asset
    ├── BalanceConfig.asset
    └── SpawnConfig.asset
```

---

## 2. Enum 정의

### 2.1 Core Enums

```csharp
// Assets/02.Scripts/Core/GameState.cs
public enum GameState
{
    None,
    Menu,
    Playing,
    Paused,
    LevelUp,
    GameOver
}

// Assets/02.Scripts/Core/DamageType.cs
public enum DamageType
{
    Physical,
    Fire,
    Poison,
    True  // 방어력 무시
}
```

### 2.2 Skill Enums

```csharp
// Assets/02.Scripts/Skill/SkillType.cs
public enum SkillType
{
    Active,     // 액티브 스킬
    Buff,       // 버프 스킬
    Ultimate    // 궁극기
}

// Assets/02.Scripts/Skill/TargetType.cs
public enum TargetType
{
    Single,     // 단일 대상
    Area,       // 범위 공격
    Self,       // 자기 자신
    AllEnemies  // 모든 적
}

// Assets/02.Scripts/Skill/BuffType.cs
public enum BuffType
{
    None,
    Burn,       // 화상 (DoT)
    Poison,     // 독 (DoT, 중첩)
    Lifesteal,  // 흡혈
    AttackUp,   // 공격력 증가
    SpeedUp     // 이동속도 증가
}
```

### 2.3 Character Enums

```csharp
// Assets/02.Scripts/Character/EnemyType.cs
public enum EnemyType
{
    Normal,
    Elite,
    Boss
}

// Assets/02.Scripts/Character/CharacterState.cs
public enum CharacterState
{
    Idle,
    Moving,
    Attacking,
    Stunned,
    Dead
}
```

### 2.4 Upgrade Enums

```csharp
// Assets/02.Scripts/Upgrade/CardRarity.cs
public enum CardRarity
{
    Common,     // 일반 (60%)
    Rare,       // 희귀 (25%)
    Epic,       // 영웅 (12%)
    Legendary   // 전설 (3%)
}

// Assets/02.Scripts/Upgrade/CardEffectType.cs
public enum CardEffectType
{
    StatBoost,      // 스탯 증가
    SkillUnlock,    // 스킬 해금
    SkillUpgrade,   // 스킬 강화
    Special         // 특수 효과
}

// Assets/02.Scripts/Upgrade/StatType.cs
public enum StatType
{
    MaxHP,
    ATK,
    DEF,
    CritRate,
    CritDamage,
    AttackSpeed,
    MoveSpeed
}
```

---

## 3. ScriptableObject 상세 정의

### 3.1 SkillData

```csharp
// Assets/02.Scripts/Skill/SkillData.cs
[CreateAssetMenu(fileName = "SK_NewSkill", menuName = "DungeonRush/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("기본 정보")]
    [Tooltip("고유 식별자")]
    public string skillId;

    [Tooltip("스킬 이름")]
    public string skillName;

    [TextArea(2, 4)]
    [Tooltip("스킬 설명")]
    public string description;

    [Tooltip("스킬 타입")]
    public SkillType skillType;

    [Tooltip("스킬 아이콘")]
    public Sprite icon;

    [Header("타겟팅")]
    [Tooltip("타겟 타입")]
    public TargetType targetType;

    [Tooltip("공격 범위")]
    public float range = 5f;

    [Tooltip("효과 반경 (Area 타입용)")]
    public float effectRadius = 2f;

    [Header("데미지")]
    [Tooltip("ATK 대비 데미지 배율 (1.0 = 100%)")]
    [Range(0f, 10f)]
    public float damageMultiplier = 1f;

    [Tooltip("데미지 타입")]
    public DamageType damageType = DamageType.Physical;

    [Tooltip("타격 횟수")]
    [Range(1, 10)]
    public int hitCount = 1;

    [Header("투사체 (Active 전용)")]
    [Tooltip("투사체 프리팹")]
    public GameObject projectilePrefab;

    [Tooltip("투사체 수")]
    [Range(1, 10)]
    public int projectileCount = 1;

    [Tooltip("관통 횟수 (0 = 관통 없음)")]
    [Range(0, 10)]
    public int penetration = 0;

    [Tooltip("투사체 속도")]
    public float projectileSpeed = 10f;

    [Header("버프 (Buff 전용)")]
    [Tooltip("버프 타입")]
    public BuffType buffType;

    [Tooltip("버프 지속 시간")]
    public float buffDuration = 3f;

    [Tooltip("틱 데미지 (DoT용, ATK 대비 비율)")]
    [Range(0f, 1f)]
    public float tickDamageRatio = 0.1f;

    [Tooltip("틱 간격")]
    public float tickInterval = 1f;

    [Tooltip("최대 중첩 횟수")]
    [Range(1, 10)]
    public int maxStacks = 1;

    [Header("궁극기 (Ultimate 전용)")]
    [Tooltip("차지 시간")]
    public float chargeTime = 2f;

    [Tooltip("차지 중 무적 여부")]
    public bool invincibleDuringCharge = false;

    [Header("쿨다운")]
    [Tooltip("쿨다운 시간")]
    public float cooldown = 8f;

    [Header("이펙트")]
    [Tooltip("시전 이펙트")]
    public GameObject castEffect;

    [Tooltip("적중 이펙트")]
    public GameObject hitEffect;

    [Tooltip("시전 사운드")]
    public AudioClip castSound;

    [Tooltip("적중 사운드")]
    public AudioClip hitSound;

    [Header("레벨업 스케일링")]
    [Tooltip("레벨당 데미지 증가율")]
    [Range(0f, 0.5f)]
    public float damagePerLevel = 0.1f;

    [Tooltip("최대 스킬 레벨")]
    public int maxLevel = 5;
}
```

### 3.2 EnemyData

```csharp
// Assets/02.Scripts/Character/EnemyData.cs
[CreateAssetMenu(fileName = "EN_NewEnemy", menuName = "DungeonRush/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("기본 정보")]
    [Tooltip("고유 식별자")]
    public string enemyId;

    [Tooltip("적 이름")]
    public string enemyName;

    [Tooltip("적 타입")]
    public EnemyType enemyType;

    [Tooltip("적 프리팹")]
    public GameObject prefab;

    [Header("기본 스탯")]
    [Tooltip("기본 최대 체력")]
    public float baseHP = 100f;

    [Tooltip("기본 공격력")]
    public float baseATK = 10f;

    [Tooltip("기본 방어력")]
    public float baseDEF = 5f;

    [Tooltip("공격 속도 (초당 공격 횟수)")]
    public float attackSpeed = 1f;

    [Tooltip("공격 범위")]
    public float attackRange = 1.5f;

    [Tooltip("이동 속도")]
    public float moveSpeed = 3f;

    [Header("보상")]
    [Tooltip("처치 시 획득 경험치")]
    public int xpReward = 10;

    [Tooltip("처치 시 획득 골드")]
    public int goldReward = 5;

    [Header("챕터별 스케일링")]
    [Tooltip("챕터당 HP 증가율")]
    [Range(0f, 1f)]
    public float hpPerChapter = 0.2f;

    [Tooltip("챕터당 ATK 증가율")]
    [Range(0f, 1f)]
    public float atkPerChapter = 0.15f;

    [Tooltip("챕터당 DEF 증가율")]
    [Range(0f, 1f)]
    public float defPerChapter = 0.1f;

    [Header("스폰 설정")]
    [Tooltip("스폰 가중치 (높을수록 자주 등장)")]
    [Range(1, 100)]
    public int spawnWeight = 50;

    [Tooltip("최소 등장 챕터")]
    public int minChapter = 1;

    [Header("보스 전용")]
    [Tooltip("보스 패턴 스크립트")]
    public BossPatternData[] bossPatterns;

    [Tooltip("페이즈 전환 HP 비율")]
    [Range(0f, 1f)]
    public float[] phaseThresholds;

    [Header("이펙트")]
    [Tooltip("스폰 이펙트")]
    public GameObject spawnEffect;

    [Tooltip("사망 이펙트")]
    public GameObject deathEffect;

    // 계산된 스탯 반환 메서드
    public float GetHP(int chapter)
    {
        return baseHP * (1 + hpPerChapter * (chapter - 1));
    }

    public float GetATK(int chapter)
    {
        return baseATK * (1 + atkPerChapter * (chapter - 1));
    }

    public float GetDEF(int chapter)
    {
        return baseDEF * (1 + defPerChapter * (chapter - 1));
    }
}
```

### 3.3 CardData

```csharp
// Assets/02.Scripts/Upgrade/CardData.cs
[CreateAssetMenu(fileName = "CD_NewCard", menuName = "DungeonRush/Card Data")]
public class CardData : ScriptableObject
{
    [Header("기본 정보")]
    [Tooltip("고유 식별자")]
    public string cardId;

    [Tooltip("카드 이름")]
    public string cardName;

    [TextArea(2, 4)]
    [Tooltip("카드 설명")]
    public string description;

    [Tooltip("카드 희귀도")]
    public CardRarity rarity;

    [Tooltip("카드 아이콘")]
    public Sprite icon;

    [Header("효과")]
    [Tooltip("효과 타입")]
    public CardEffectType effectType;

    [Header("스탯 증가 (StatBoost용)")]
    [Tooltip("증가시킬 스탯 타입")]
    public StatType statType;

    [Tooltip("증가 값 (고정값 또는 비율)")]
    public float statValue;

    [Tooltip("비율 증가 여부 (true = %, false = 고정값)")]
    public bool isPercentage;

    [Header("스킬 관련 (SkillUnlock/Upgrade용)")]
    [Tooltip("연결된 스킬")]
    public SkillData linkedSkill;

    [Tooltip("스킬 레벨 증가량 (Upgrade용)")]
    public int skillLevelIncrease = 1;

    [Header("출현 조건")]
    [Tooltip("최소 플레이어 레벨")]
    public int minPlayerLevel = 1;

    [Tooltip("해당 스킬 보유 시에만 등장 (Upgrade용)")]
    public bool requiresSkill;

    [Tooltip("한 런에서 최대 획득 횟수 (0 = 무제한)")]
    public int maxObtainCount = 0;

    [Header("시각 효과")]
    [Tooltip("희귀도별 테두리 색상")]
    public Color borderColor = Color.white;

    [Tooltip("획득 시 이펙트")]
    public GameObject obtainEffect;

    // 희귀도별 확률 반환
    public static float GetRarityWeight(CardRarity rarity)
    {
        return rarity switch
        {
            CardRarity.Common => 60f,
            CardRarity.Rare => 25f,
            CardRarity.Epic => 12f,
            CardRarity.Legendary => 3f,
            _ => 0f
        };
    }
}
```

### 3.4 StageData

```csharp
// Assets/02.Scripts/Stage/StageData.cs
[CreateAssetMenu(fileName = "ST_Chapter", menuName = "DungeonRush/Stage Data")]
public class StageData : ScriptableObject
{
    [Header("챕터 정보")]
    [Tooltip("챕터 번호")]
    public int chapterNumber;

    [Tooltip("챕터 이름")]
    public string chapterName;

    [Tooltip("배경 스프라이트")]
    public Sprite background;

    [Tooltip("배경 음악")]
    public AudioClip bgm;

    [Header("웨이브 설정")]
    [Tooltip("챕터당 웨이브 수")]
    public int wavesPerChapter = 10;

    [Tooltip("웨이브당 적 수")]
    public int enemiesPerWave = 10;

    [Tooltip("웨이브 시작 딜레이")]
    public float waveStartDelay = 0.5f;

    [Tooltip("적 스폰 간격")]
    public float spawnInterval = 0.5f;

    [Header("적 구성")]
    [Tooltip("일반 적 풀")]
    public EnemySpawnEntry[] normalEnemies;

    [Tooltip("엘리트 적 풀")]
    public EnemySpawnEntry[] eliteEnemies;

    [Tooltip("보스")]
    public EnemyData bossEnemy;

    [Header("엘리트 출현")]
    [Tooltip("엘리트 출현 확률 (0~1)")]
    [Range(0f, 1f)]
    public float eliteSpawnChance = 0.1f;

    [Tooltip("엘리트 최소 출현 웨이브")]
    public int eliteMinWave = 3;

    [Header("보스 웨이브")]
    [Tooltip("보스 웨이브 시작 딜레이")]
    public float bossWaveDelay = 1.0f;

    [Header("난이도")]
    [Tooltip("난이도 배율")]
    public float difficultyMultiplier = 1f;

    [Tooltip("경험치 배율")]
    public float xpMultiplier = 1f;

    [Tooltip("골드 배율")]
    public float goldMultiplier = 1f;
}

[System.Serializable]
public class EnemySpawnEntry
{
    public EnemyData enemyData;
    [Range(1, 100)]
    public int weight = 50;
}
```

### 3.5 GameConfig (전역 설정)

```csharp
// Assets/02.Scripts/Core/GameConfig.cs
[CreateAssetMenu(fileName = "GameConfig", menuName = "DungeonRush/Config/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("플레이어 초기 스탯")]
    public float initialHP = 100f;
    public float initialATK = 20f;
    public float initialDEF = 5f;
    public float initialCritRate = 0.05f;
    public float initialCritDamage = 1.5f;
    public float initialAttackSpeed = 1f;
    public float initialMoveSpeed = 5f;

    [Header("레벨업")]
    public int baseXPRequired = 100;
    public float xpRequiredMultiplier = 1.2f;
    public int cardsPerLevelUp = 3;

    [Header("리롤")]
    public int baseRerollCost = 40;
    public float rerollCostIncreasePerUse = 0.15f;
    public float rerollCostIncreasePerStage = 0.10f;
    public int maxRerollsPerLevelUp = 3;

    [Header("스킬")]
    public int maxSkillSlots = 3;
    public float autoSkillInterval = 0.5f;

    [Header("전투")]
    public float attackCooldownMin = 0.2f;
    public float criticalDamageMultiplier = 2f;
    public float minimumDamage = 1f;

    [Header("화면")]
    public float spawnAreaMargin = 1f;
    public float despawnDistance = 15f;

    // 레벨별 필요 경험치 계산
    public int GetXPRequired(int level)
    {
        return Mathf.CeilToInt(baseXPRequired * Mathf.Pow(xpRequiredMultiplier, level - 1));
    }

    // 리롤 비용 계산
    public int GetRerollCost(int rerollCount, int stage)
    {
        float cost = baseRerollCost
            * (1 + rerollCostIncreasePerUse * rerollCount)
            * (1 + rerollCostIncreasePerStage * (stage - 1));
        return Mathf.CeilToInt(cost);
    }
}
```

---

## 4. 런타임 데이터 클래스

### 4.1 CharacterStats (런타임)

```csharp
// Assets/02.Scripts/Character/CharacterStats.cs
[System.Serializable]
public class CharacterStats
{
    [Header("기본 스탯")]
    public float maxHP;
    public float currentHP;
    public float atk;
    public float def;
    public float critRate;
    public float critDamage;
    public float attackSpeed;
    public float moveSpeed;

    [Header("추가 스탯 (버프/장비)")]
    public float bonusATK;
    public float bonusDEF;
    public float bonusCritRate;
    public float bonusCritDamage;
    public float bonusAttackSpeed;
    public float bonusMoveSpeed;

    // 최종 스탯 계산
    public float TotalATK => atk + bonusATK;
    public float TotalDEF => def + bonusDEF;
    public float TotalCritRate => Mathf.Clamp01(critRate + bonusCritRate);
    public float TotalCritDamage => critDamage + bonusCritDamage;
    public float TotalAttackSpeed => attackSpeed + bonusAttackSpeed;
    public float TotalMoveSpeed => moveSpeed + bonusMoveSpeed;

    public bool IsDead => currentHP <= 0;
    public float HPRatio => currentHP / maxHP;

    // 초기화
    public void Initialize(GameConfig config)
    {
        maxHP = config.initialHP;
        currentHP = maxHP;
        atk = config.initialATK;
        def = config.initialDEF;
        critRate = config.initialCritRate;
        critDamage = config.initialCritDamage;
        attackSpeed = config.initialAttackSpeed;
        moveSpeed = config.initialMoveSpeed;

        ResetBonusStats();
    }

    public void Initialize(EnemyData data, int chapter)
    {
        maxHP = data.GetHP(chapter);
        currentHP = maxHP;
        atk = data.GetATK(chapter);
        def = data.GetDEF(chapter);
        critRate = 0f;
        critDamage = 1.5f;
        attackSpeed = data.attackSpeed;
        moveSpeed = data.moveSpeed;

        ResetBonusStats();
    }

    public void ResetBonusStats()
    {
        bonusATK = 0;
        bonusDEF = 0;
        bonusCritRate = 0;
        bonusCritDamage = 0;
        bonusAttackSpeed = 0;
        bonusMoveSpeed = 0;
    }

    // 스탯 수정
    public void ApplyStatBoost(StatType type, float value, bool isPercentage)
    {
        float actualValue = isPercentage ? GetBaseStat(type) * value : value;

        switch (type)
        {
            case StatType.MaxHP:
                maxHP += actualValue;
                currentHP += actualValue;
                break;
            case StatType.ATK:
                bonusATK += actualValue;
                break;
            case StatType.DEF:
                bonusDEF += actualValue;
                break;
            case StatType.CritRate:
                bonusCritRate += value; // 크리티컬은 항상 고정값
                break;
            case StatType.CritDamage:
                bonusCritDamage += value;
                break;
            case StatType.AttackSpeed:
                bonusAttackSpeed += actualValue;
                break;
            case StatType.MoveSpeed:
                bonusMoveSpeed += actualValue;
                break;
        }
    }

    private float GetBaseStat(StatType type)
    {
        return type switch
        {
            StatType.MaxHP => maxHP,
            StatType.ATK => atk,
            StatType.DEF => def,
            StatType.CritRate => critRate,
            StatType.CritDamage => critDamage,
            StatType.AttackSpeed => attackSpeed,
            StatType.MoveSpeed => moveSpeed,
            _ => 0f
        };
    }
}
```

### 4.2 BuffInstance (런타임 버프)

```csharp
// Assets/02.Scripts/Skill/BuffInstance.cs
public class BuffInstance
{
    public BuffType Type { get; private set; }
    public float Duration { get; private set; }
    public float RemainingTime { get; private set; }
    public float TickDamage { get; private set; }
    public float TickInterval { get; private set; }
    public int CurrentStacks { get; private set; }
    public int MaxStacks { get; private set; }

    private float _tickTimer;

    public BuffInstance(SkillData skillData, float attackPower)
    {
        Type = skillData.buffType;
        Duration = skillData.buffDuration;
        RemainingTime = Duration;
        TickDamage = attackPower * skillData.tickDamageRatio;
        TickInterval = skillData.tickInterval;
        MaxStacks = skillData.maxStacks;
        CurrentStacks = 1;
        _tickTimer = TickInterval;
    }

    public bool IsExpired => RemainingTime <= 0;

    public void AddStack()
    {
        if (CurrentStacks < MaxStacks)
        {
            CurrentStacks++;
        }
        RemainingTime = Duration; // 갱신
    }

    public float Update(float deltaTime)
    {
        RemainingTime -= deltaTime;
        _tickTimer -= deltaTime;

        float damage = 0f;
        if (_tickTimer <= 0 && !IsExpired)
        {
            damage = TickDamage * CurrentStacks;
            _tickTimer = TickInterval;
        }

        return damage;
    }
}
```

---

## 5. 저장 데이터 구조

### 5.1 SaveData

```csharp
// Assets/02.Scripts/Core/SaveData.cs
[System.Serializable]
public class SaveData
{
    // 영구 저장 데이터
    public int totalGold;
    public int highestChapter;
    public int highestWave;
    public int totalKills;
    public float totalPlayTime;

    // 랭킹 데이터
    public List<RunRecord> runRecords = new List<RunRecord>();

    // 설정
    public float bgmVolume = 1f;
    public float sfxVolume = 1f;
    public bool vibration = true;
}

[System.Serializable]
public class RunRecord
{
    public int chapter;
    public int wave;
    public int score;
    public int kills;
    public float clearTime;
    public string dateTime;
}
```

---

## 6. 네이밍 컨벤션 정리

| 타입 | 접두사 | 예시 |
|------|--------|------|
| SkillData | SK_ | SK_GaleSlash.asset |
| EnemyData | EN_ | EN_Slime.asset |
| CardData | CD_ | CD_ATKUp.asset |
| StageData | ST_ | ST_Chapter1.asset |
| Config | - | GameConfig.asset |

---

---

## 7. 빌드/시너지 시스템 데이터 (신규)

### 7.1 Build Enums

```csharp
// Assets/02.Scripts/Build/SynergyType.cs
public enum SynergyType
{
    FireMaster,         // 화염 마스터
    PoisonExpert,       // 맹독 전문가
    Unyielding,         // 불굴의 전사
    CriticalStrike,     // 치명적 일격
    SwiftBlade,         // 신속의 칼날
    VampireBless        // 흡혈귀의 축복
}

// Assets/02.Scripts/Build/SynergyTier.cs
public enum SynergyTier
{
    Basic,      // Tier 1: 스킬 1 + 스탯 1
    Advanced,   // Tier 2: 스킬 2 + 스탯 2
    Legendary   // Tier 3: 스킬 3 + 스탯 3
}

// Assets/02.Scripts/Build/SynergyEffectType.cs
public enum SynergyEffectType
{
    DotDamageBonus,         // DoT 데미지 증가
    PoisonStackBonus,       // 독 중첩 한도 증가
    DamageReduction,        // 받는 피해 감소
    CritExtraHit,           // 크리티컬 추가 타격
    CooldownReduction,      // 스킬 쿨다운 감소
    LifestealBonus          // 흡혈량 증가
}
```

### 7.2 SynergyData ScriptableObject

```csharp
// Assets/02.Scripts/Build/SynergyData.cs
[CreateAssetMenu(fileName = "SY_NewSynergy", menuName = "DungeonRush/Synergy Data")]
public class SynergyData : ScriptableObject
{
    [Header("기본 정보")]
    [Tooltip("고유 식별자")]
    public string synergyId;

    [Tooltip("시너지 이름")]
    public string synergyName;

    [TextArea(2, 4)]
    [Tooltip("시너지 설명")]
    public string description;

    [Tooltip("시너지 타입")]
    public SynergyType synergyType;

    [Tooltip("시너지 티어")]
    public SynergyTier tier;

    [Tooltip("시너지 아이콘")]
    public Sprite icon;

    [Header("스킬 조건")]
    [Tooltip("필요한 스킬 타입들")]
    public SkillRequirement[] requiredSkills;

    [Header("스탯 조건")]
    [Tooltip("필요한 스탯 조건들")]
    public StatRequirement[] requiredStats;

    [Header("효과")]
    [Tooltip("시너지 효과 타입")]
    public SynergyEffectType effectType;

    [Tooltip("효과 수치 (비율: 0.5 = 50%)")]
    public float effectValue;

    [Tooltip("추가 효과 설명")]
    public string effectDescription;

    [Header("UI")]
    [Tooltip("활성화 이펙트")]
    public GameObject activateEffect;

    [Tooltip("활성화 사운드")]
    public AudioClip activateSound;

    // 조건 충족 여부 체크
    public bool CheckConditions(PlayerBuildState buildState)
    {
        // 스킬 조건 체크
        foreach (var req in requiredSkills)
        {
            if (!buildState.HasSkillOfType(req.skillType, req.minLevel))
            {
                return false;
            }
        }

        // 스탯 조건 체크
        foreach (var req in requiredStats)
        {
            if (!buildState.MeetsStatThreshold(req.statType, req.thresholdRatio))
            {
                return false;
            }
        }

        return true;
    }
}

[System.Serializable]
public class SkillRequirement
{
    [Tooltip("필요 스킬 타입 (버프 타입 또는 스킬 타입)")]
    public BuffType skillType;

    [Tooltip("최소 스킬 레벨")]
    [Range(1, 5)]
    public int minLevel = 1;
}

[System.Serializable]
public class StatRequirement
{
    [Tooltip("필요 스탯 타입")]
    public StatType statType;

    [Tooltip("초기값 대비 필요 비율 (1.5 = 150%)")]
    [Range(1f, 3f)]
    public float thresholdRatio = 1.5f;
}
```

### 7.3 DifficultyConfig ScriptableObject

```csharp
// Assets/02.Scripts/Stage/DifficultyConfig.cs
[CreateAssetMenu(fileName = "DifficultyConfig", menuName = "DungeonRush/Config/Difficulty Config")]
public class DifficultyConfig : ScriptableObject
{
    [Header("챕터별 난이도 설정")]
    public ChapterDifficulty[] chapterDifficulties;

    // 챕터에 맞는 난이도 설정 반환
    public ChapterDifficulty GetDifficulty(int chapter)
    {
        foreach (var diff in chapterDifficulties)
        {
            if (chapter >= diff.chapterRangeMin && chapter <= diff.chapterRangeMax)
            {
                return diff;
            }
        }

        // 기본값: 마지막 설정 반환
        return chapterDifficulties[^1];
    }
}

[System.Serializable]
public class ChapterDifficulty
{
    [Header("챕터 범위")]
    public int chapterRangeMin;
    public int chapterRangeMax;

    [Header("적 스케일링")]
    [Tooltip("적 스탯 배율")]
    [Range(1f, 10f)]
    public float enemyStatMultiplier = 1f;

    [Header("DPS 체크")]
    [Tooltip("DPS 체크 활성화")]
    public bool hasDpsCheck;

    [Tooltip("DPS 체크 시간 (초)")]
    public float dpsCheckTime = 60f;

    [Header("생존 체크")]
    [Tooltip("생존 체크 활성화")]
    public bool hasSurvivalCheck;

    [Tooltip("최소 HP 비율")]
    [Range(1f, 3f)]
    public float minHpRatio = 1f;

    [Tooltip("최소 DEF 비율")]
    [Range(1f, 3f)]
    public float minDefRatio = 1f;

    [Header("빌드 요구사항")]
    [Tooltip("빌드 체크 활성화")]
    public bool hasBuildRequirement;

    [Tooltip("최소 스킬 개수")]
    public int minSkillCount = 0;

    [Tooltip("최소 시너지 개수")]
    public int minSynergyCount = 0;
}
```

### 7.4 런타임 데이터 클래스

```csharp
// Assets/02.Scripts/Build/PlayerBuildState.cs
[System.Serializable]
public class PlayerBuildState
{
    // 선택한 카드들
    public List<CardData> selectedCards = new List<CardData>();

    // 보유 스킬들
    public List<SkillBase> equippedSkills = new List<SkillBase>();

    // 활성화된 시너지들
    public List<SynergyType> activeSynergies = new List<SynergyType>();

    // 카드 추가
    public void AddCard(CardData card)
    {
        selectedCards.Add(card);
    }

    // 스킬 타입 보유 확인
    public bool HasSkillOfType(BuffType buffType, int minLevel)
    {
        foreach (var skill in equippedSkills)
        {
            if (skill.Data.buffType == buffType && skill.Level >= minLevel)
            {
                return true;
            }
        }
        return false;
    }

    // 스탯 임계치 충족 확인
    public bool MeetsStatThreshold(StatType statType, float thresholdRatio)
    {
        // CharacterStats와 연동하여 확인
        // 실제 구현 시 PlayerController에서 참조
        return false; // placeholder
    }

    // 상태 초기화 (새 런 시작 시)
    public void Reset()
    {
        selectedCards.Clear();
        equippedSkills.Clear();
        activeSynergies.Clear();
    }
}

// Assets/02.Scripts/Build/ActiveSynergy.cs
[System.Serializable]
public class ActiveSynergy
{
    public SynergyData data;
    public bool isActive;
    public float activatedTime;

    public ActiveSynergy(SynergyData synergyData)
    {
        data = synergyData;
        isActive = false;
        activatedTime = 0f;
    }

    public void Activate()
    {
        isActive = true;
        activatedTime = Time.time;
    }

    public void Deactivate()
    {
        isActive = false;
    }
}
```

### 7.5 ScriptableObject 폴더 구조 (추가)

```
Assets/10.ScriptableObjects/
├── ...
├── Synergies/                    # 신규
│   ├── Tier1/
│   │   ├── SY_FireMaster.asset
│   │   ├── SY_PoisonExpert.asset
│   │   └── SY_CriticalStrike.asset
│   ├── Tier2/
│   │   ├── SY_Unyielding.asset
│   │   └── SY_SwiftBlade.asset
│   └── Tier3/
│       └── SY_VampireBless.asset
│
└── Config/
    ├── GameConfig.asset
    ├── BalanceConfig.asset
    ├── SpawnConfig.asset
    └── DifficultyConfig.asset    # 신규
```

---

## 8. 데이터 검증 체크리스트

- [ ] 모든 ScriptableObject에 고유 ID 부여
- [ ] 스킬 쿨다운 0 이상 검증
- [ ] 적 스탯 양수 검증
- [ ] 카드 희귀도 확률 합계 100%
- [ ] 스테이지 데이터 순차적 챕터 번호
- [ ] 보스 데이터 필수 필드 검증
- [ ] 시너지 조건 논리적 검증 (달성 가능 여부)
- [ ] 챕터별 난이도 설정 연속성 검증