# Stats 시스템 수정 지침서 (Claude Code용)

> **프로젝트 경로**: `C:\Harbor\DungeonRush`
> **수정 대상**: `Assets/02.Scripts/Stats/` 전체
> **명세서**: `STAT_SYSTEM_DEV_SPEC.md` 참조
> **기획 CSV (컬럼 정의서)**: `파라미터_컬럼_1차_-_Player관련.csv`, `파라미터_컬럼_1차_-_스킬.csv`

---

## 배경

현재 구현된 Stats 시스템의 아키텍처(폴더 구조, Repository 패턴, Dirty Flag, StatSheet 합산 등)는 유지한다.
문제는 **데이터 모델이 실제 CSV 스키마를 반영하지 않고 별도로 설계**되어 있어서, 실제 데이터를 넣으면 파싱이 깨지거나 필드가 누락된다.

기획자가 제공한 CSV 2종은 **컬럼 정의서**(스키마 문서)이지 실제 데이터가 아니다.
따라서 이번 작업에서는:
1. 컬럼 정의서를 기반으로 **테스트용 데이터 CSV**를 생성
2. 데이터 모델을 **실제 CSV 스키마에 맞게 수정**
3. Repository 파싱 로직을 **실제 CSV 포맷에 맞게 수정**
4. Runtime 로직의 **누락/오류 수정**
5. **end-to-end 검증** (CSV 로드 → GameData 조회 → RunState 동작)

### 중요: CSV 컬럼명 통일 방침

기획자 원본 Player CSV는 `BaseATK`, `BaseHP` 등 `Base` 접두어를 사용한다.
패시브 CSV의 `TargetStat`은 `ATK`, `HP` 등 접두어 없이 사용한다.
**향후 기획자에게 Player CSV 컬럼명에서 `Base` 접두어를 제거해달라고 요청할 예정.**

따라서 이번 작업에서는:
- **테스트 CSV는 `Base` 접두어 없는 통일된 컬럼명 사용** (`ATK`, `HP`, `MoveSpeed` 등)
- **코드에 `StripBasePrefix` 같은 접두어 변환 로직을 넣지 않는다. 기존 것도 제거.**
- CSV 헤더 = BaseStats 필드명 = StatSheet 키 = 패시브 TargetStat → **모두 동일한 문자열**

---

## Phase 0: 테스트용 데이터 CSV 생성

컬럼 정의서의 컬럼명과 예시값을 기반으로, `Assets/Resources/Data/`에 테스트 CSV 3종을 생성한다.

### 0-1. Characters.csv

기획안의 캐릭터 4종(마법사/궁수/암살자/검사) 데이터. **`Base` 접두어 없이 통일된 컬럼명 사용.**

```csv
PlayerID,PlayerName,Grade,PlayerModelPrefab,ATK,CDR,CritRate,CritDamage,HP,HPRegen,Armor,Evasion,MoveSpeed,PickupRange,ExpGain,GoldGain,Luck,Lifesteal,Thorns,StartActiveSkillID,StartPassiveSkillID,WeaponTypeTag,CharacterTraitID
Player_001,마법사,0,Wizard_001,12,0,0.01,2.0,100,0,0,0,1.0,1.0,1.0,1.0,0.05,0,0,SK_A_001,,Staff,TR_001
Player_002,궁수,0,Archer_001,10,0,0.01,2.0,80,0,0,0.03,1.1,1.0,1.0,1.0,0,0,0,SK_A_002,,Bow,TR_002
Player_003,암살자,1,Assassin_001,8,0,0.05,2.0,70,0,0,0.06,1.2,1.0,1.0,1.0,0,0,0,SK_A_003,,Dagger,TR_003
Player_004,검사,1,Warrior_001,15,0,0.01,2.0,150,0,0.1,0,0.9,1.0,1.0,1.0,0,0,0,SK_A_004,,Sword,TR_004
```

### 0-2. Skills.csv

스킬 메인 테이블. 공격 스킬 4종 + 패시브 스킬 2종. 공격 스킬과 패시브 스킬이 같은 테이블에 공존 — 해당 타입이 아닌 컬럼은 빈값.

```csv
SkillID,SkillName,SkillType,SkillSubType,MaxLevel,SkillGroup,Grade,Rate,LinkedEffectGroupID,DescKey,IconPath,PrefabPath,CastVFXPath,HitVFXPath,SFXPath,WeaponTagReq,SkillCoef,BaseCoolTime,CritEnabled,LifestealEnabled,BaseRange,BaseRadius,BaseKnockback,PierceCount,ChainCount,BounceCount,BaseProj,BaseProjSpeed,FirePattern,BaseDuration,TickInterval,TickCoef,MaxStack,StackRule,PassiveEffectType,TargetStat,ModifyType,ModifyValue,ApplyScope,ApplySkillTag,ApplySkillID,TriggerType,TriggerChance,TriggerCoolTime,BuffDuration,StackLimit,PassivePrefabPath
SK_A_001,파이어볼,0,Projectile,5,0,Common,1.0,,SKILL_DESC_FIREBALL,Icons/Skill/fireball,Prefabs/Skill/Fireball,VFX/Cast_Fire,VFX/Hit_Fire,SFX/fireball,,1.2,1.5,TRUE,TRUE,5.0,1.0,1.0,0,0,0,1,10.0,Single,0,0,0,1,0,,,,,,,,,,,,
SK_A_002,장궁,0,Projectile,5,1,Common,1.0,,SKILL_DESC_BOW,Icons/Skill/bow,Prefabs/Skill/Arrow,VFX/Cast_Arrow,VFX/Hit_Arrow,SFX/bow,,1.0,0.8,TRUE,TRUE,7.0,0,0.5,0,0,1,1,12.0,Single,0,0,0,1,0,,,,,,,,,,,,
SK_A_003,독단검,0,Projectile,5,2,Common,1.0,,SKILL_DESC_DAGGER,Icons/Skill/dagger,Prefabs/Skill/Dagger,,VFX/Hit_Dagger,SFX/dagger,,0.8,0.6,TRUE,FALSE,3.0,0,0.3,0,0,0,1,8.0,Single,3.0,1.0,0.3,5,1,,,,,,,,,,,,
SK_A_004,대검,0,Melee,5,3,Common,1.0,,SKILL_DESC_SWORD,Icons/Skill/sword,Prefabs/Skill/Sword,VFX/Cast_Sword,VFX/Hit_Sword,SFX/sword,,1.5,1.2,TRUE,TRUE,2.0,1.5,2.0,3,0,0,1,0,Single,0,0,0,1,0,,,,,,,,,,,,
SK_P_001,힘의 룬,1,StatPassive,5,10,Common,1.0,,SKILL_DESC_RUNE_POWER,Icons/Rune/power,,,,,,,,,,,,,,,,,,,,,StatModify,ATK,Mult,0.08,PlayerGlobal,,,None,0,0,0,0,
SK_P_002,신속의 룬,1,StatPassive,5,11,Common,1.0,,SKILL_DESC_RUNE_SPEED,Icons/Rune/speed,,,,,,,,,,,,,,,,,,,,,StatModify,CDR,Add,0.08,PlayerGlobal,,,None,0,0,0,0,
```

### 0-3. SkillLevelUps.csv

레벨업 테이블. 1행=1수정 구조.

```csv
SkillId,Level,ParamName,ValueType,Value
SK_A_001,2,SkillCoef,Flat,0.15
SK_A_001,2,BaseCoolTime,Flat,-0.1
SK_A_001,3,SkillCoef,Flat,0.2
SK_A_001,3,BaseProj,Flat,1
SK_A_001,4,SkillCoef,Percent,0.1
SK_A_001,5,SkillCoef,Flat,0.5
SK_A_001,5,BaseCoolTime,Flat,-0.2
SK_A_002,2,BaseCoolTime,Flat,-0.05
SK_A_002,3,BounceCount,Flat,1
SK_P_001,2,ModifyValue,Flat,0.04
SK_P_001,3,ModifyValue,Flat,0.04
```

---

## Phase 1: 데이터 모델 수정 — CSV 스키마에 맞추기

### 1-1. BaseStats.cs 수정

**파일**: `Stats/Data/BaseStats.cs`

필드를 실제 Player CSV의 스탯 컬럼 15개에 맞춘다 (접두어 없는 통일된 이름).

**제거할 필드** (CSV에 없고 무기별 속성인 것): `AttackSpeed`, `AttackRange`, `AreaSize`, `ProjectileCount`, `ProjectileSpeed`, `DetectRange`.

**추가할 필드**: `Evasion`, `PickupRange`, `ExpGain`, `GoldGain`, `Lifesteal`, `Thorns`.

**이름 변경**: `MaxHp` → `HP`, `CritChance` → `CritRate`.

최종 필드 목록 (**CSV 헤더와 정확히 동일**):
```
ATK, CDR, CritRate, CritDamage,
HP, HPRegen, Armor, Evasion,
MoveSpeed, PickupRange, ExpGain, GoldGain, Luck,
Lifesteal, Thorns
```

`AllKeys`, `GetValue()`, `SetValue()` 모두 동기화.

### 1-2. CharacterSpec.cs 수정

**파일**: `Stats/Data/CharacterSpec.cs`

CSV 전체 컬럼을 반영하여 필드 추가:

```csharp
public class CharacterSpec
{
    public string Id;                    // PlayerID
    public string Name;                  // PlayerName
    public int Grade;                    // Grade
    public string ModelPrefab;           // PlayerModelPrefab
    public BaseStats Stats;              // 스탯 15개
    public string StartActiveSkillId;    // StartActiveSkillID ← FK
    public string StartPassiveSkillId;   // StartPassiveSkillID (빈값 가능)
    public string WeaponTypeTag;         // WeaponTypeTag
    public string CharacterTraitId;      // CharacterTraitID
}
```

### 1-3. SkillSpec.cs 수정

**파일**: `Stats/Data/SkillSpec.cs`

누락된 공통 필드 추가:

```csharp
public string SubType;               // SkillSubType ("Projectile", "Melee", "StatPassive" 등)
public int SkillGroup;               // SkillGroup
public string Grade;                 // Grade ("Common", "Rare" 등)
public float Rate;                   // Rate (등장 확률/가중치)
public string[] LinkedEffectGroupIds; // LinkedEffectGroupID (세미콜론 구분 → 배열)
public string DescKey;               // DescKey
public string IconPath;              // IconPath
public string PrefabPath;            // PrefabPath
public string CastVFXPath;           // CastVFXPath
public string HitVFXPath;            // HitVFXPath
public string SFXPath;               // SFXPath
public string WeaponTagReq;          // WeaponTagReq
```

`SkillCategory` enum 값: 기존 `Attack`/`Passive` 유지 (CSV의 0/1에 대응).

### 1-4. SkillAttackData.cs 수정

**파일**: `Stats/Data/SkillAttackData.cs`

CSV 공격용 컬럼에 맞게 필드 교체:

```csharp
public float SkillCoef;          // SkillCoef (기존 Damage/DamageCoeff 대체)
public float BaseCoolTime;       // BaseCoolTime (기존 Cooldown)
public bool CritEnabled;         // CritEnabled ← 추가
public bool LifestealEnabled;    // LifestealEnabled ← 추가
public float BaseRange;          // BaseRange (기존 Range)
public float BaseRadius;         // BaseRadius ← 추가
public float BaseKnockback;      // BaseKnockback (기존 KnockbackForce)
public int PierceCount;          // PierceCount (기존 Pierce)
public int ChainCount;           // ChainCount
public int BounceCount;          // BounceCount
public int BaseProj;             // BaseProj (기존 ProjectileCount)
public float BaseProjSpeed;      // BaseProjSpeed (기존 ProjectileSpeed)
public string FirePattern;       // FirePattern ← 추가 (string)
public float BaseDuration;       // BaseDuration (기존 Duration)
public float TickInterval;       // TickInterval
public float TickCoef;           // TickCoef (기존 TickDamageCoeff)
public int MaxStack;             // MaxStack
public int StackRule;            // StackRule (0=Refresh/1=Stack/2=Extend)
```

**제거**: `Damage`, `DamageCoeff`, `CastDelay`, `ChainDamageDecay`, `CritChanceBonus`, `CritDamageBonus` (CSV에 없음).

`AllParams`, `GetParam()`, `SetParam()` 동기화. bool 필드(`CritEnabled`, `LifestealEnabled`)는 GetParam/SetParam에서 제외하고 별도 처리.

### 1-5. SkillPassiveData.cs 수정

**파일**: `Stats/Data/SkillPassiveData.cs`

CSV 패시브 컬럼에 맞게 **전면 교체**:

```csharp
public string EffectType;        // PassiveEffectType ("StatModify", "OnHit" 등)
public string TargetStat;        // TargetStat ("ATK", "HP" 등)
public string ModifyType;        // ModifyType ("Add", "Mult", "Override")
public float ModifyValue;        // ModifyValue
public string ApplyScope;        // ApplyScope ("PlayerGlobal", "SkillTag", "SpecificSkill")
public string ApplySkillTag;     // ApplySkillTag
public string ApplySkillId;      // ApplySkillID
public string TriggerType;       // TriggerType ("None", "OnHit" 등)
public float TriggerChance;      // TriggerChance
public float TriggerCoolTime;    // TriggerCoolTime
public float BuffDuration;       // BuffDuration
public int StackLimit;           // StackLimit
public string PassivePrefabPath; // PassivePrefabPath
```

`AllParams`, `GetParam()`, `SetParam()`에서 float 필드만 처리. string 필드는 별도 접근.

### 1-6. ParamModification.cs — ValueType에 Set 추가

**파일**: `Stats/Data/ParamModification.cs`

```csharp
public enum ValueType
{
    Set,      // 값 교체
    Flat,     // 가산
    Percent   // 곱연산
}
```

### 1-7. StatConfig.cs — 캡 키 이름 동기화

**파일**: `Stats/Config/StatConfig.cs`

`s_statCaps`의 키를 수정된 BaseStats 필드명과 맞춘다. 예: `"MaxHp"` → `"HP"`, `"CritChance"` → `"CritRate"` 등. CSV에 없는 키(`AttackSpeed`, `DetectRange` 등) 제거.

---

## Phase 2: Repository 파싱 수정

### 2-1. CsvCharacterRepository.cs 재작성

**파일**: `Stats/Repository/Csv/CsvCharacterRepository.cs`

테스트 CSV(`Characters.csv`) 기준으로 재작성:
- `rows[0]` = 헤더, `rows[1]~` = 데이터
- 헤더명으로 컬럼 인덱스 동적 탐색 (`FindColumn` 방식)
- **`StripBasePrefix` 로직 완전 제거** — CSV 헤더를 그대로 `BaseStats.SetValue()` 키로 사용
- Grade, ModelPrefab, StartActiveSkillID 등 CharacterSpec 새 필드 파싱 추가

### 2-2. CsvSkillRepository.cs 수정

**파일**: `Stats/Repository/Csv/CsvSkillRepository.cs`

- 스킬 CSV(`Skills.csv`): 공격/패시브가 한 테이블에 공존. `SkillType` 컬럼(0/1)으로 분기
- 레벨업 CSV(`SkillLevelUps.csv`): 현재 구조(별도 파일) 유지
- 헤더명을 실제 CSV 컬럼명과 맞출 것 (`"Id"` → `"SkillID"`, `"Category"` → `"SkillType"` 등)
- `LinkedEffectGroupID`는 세미콜론 구분 → string[] 파싱
- `CritEnabled`/`LifestealEnabled`는 `"TRUE"`/`"FALSE"` → bool 파싱
- SkillSpec의 새 필드(SubType, Grade, Rate, IconPath 등) 파싱 추가
- SkillPassiveData 파싱을 새 필드 구조에 맞게 수정

### 2-3. GameData.cs — ApplyMod에 Set 케이스 추가

**파일**: `Stats/Data/GameData.cs`

```csharp
float newValue = mod.ValueType switch
{
    ValueType.Set => mod.Value,
    ValueType.Flat => current + mod.Value,
    ValueType.Percent => current * (1f + mod.Value),
    _ => current
};
```

---

## Phase 3: Runtime 수정

### 3-1. StatSheet.cs — RecalculateAll 키 범위 확장

**파일**: `Stats/Runtime/StatSheet.cs`

`RecalculateAll()`에서 `BaseStats.AllKeys`만 순회하는 문제 수정. modifier에 등록된 모든 고유 StatKey를 수집하여 계산:

```csharp
private void RecalculateAll()
{
    _cachedFinals.Clear();

    var allKeys = new HashSet<string>(BaseStats.AllKeys);
    foreach (var mod in _modifiers)
    {
        allKeys.Add(mod.StatKey);
    }

    foreach (string key in allKeys)
    {
        float baseValue = GetBase(key);
        float flatSum = 0f;
        float percentSum = 0f;

        foreach (var mod in _modifiers)
        {
            if (mod.StatKey != key) continue;
            if (mod.ModifyType == ModifyType.Flat) flatSum += mod.Value;
            else percentSum += mod.Value;
        }

        float finalValue = (baseValue + flatSum) * (1f + percentSum);
        finalValue = StatConfig.ClampStat(key, finalValue);
        _cachedFinals[key] = finalValue;
    }

    _isDirty = false;
}
```

### 3-2. WeaponResolvedStats.cs — 전역 보너스 추가 + 필드명 동기화

**파일**: `Stats/Runtime/WeaponResolvedStats.cs`

`Resolve()`에 누락된 전역 보너스 적용 추가:
- 투사체 수: `StatSheet.GetFinal("BaseProj")`
- 관통: `"PierceCount"`
- 넉백: `"BaseKnockback"`
- 바운스: `"BounceCount"`

**주의**: 이 키들은 BaseStats에 없으므로, 룬이 해당 키로 modifier를 등록해야만 값이 생김. Phase 3-1의 수정이 전제.

기존 하드코딩된 보너스 적용 부분의 **필드명을 수정된 SkillAttackData 필드명과 맞출 것** (예: `Cooldown` → `BaseCoolTime`, `ProjectileSpeed` → `BaseProjSpeed`).

---

## Phase 4: end-to-end 검증

### 검증 시나리오 (유닛 테스트 또는 MonoBehaviour 테스트 스크립트)

```csharp
// 1. CSV 로드 → GameData 생성
var charRepo = new CsvCharacterRepository(Resources.Load<TextAsset>("Data/Characters").text);
var skillRepo = new CsvSkillRepository(
    Resources.Load<TextAsset>("Data/Skills").text,
    Resources.Load<TextAsset>("Data/SkillLevelUps").text);
var gameData = new GameData(charRepo, skillRepo);

// 2. 캐릭터 조회
var wizard = gameData.GetCharacter("Player_001");
Assert(wizard != null);
Assert(wizard.Stats.ATK == 12f);
Assert(wizard.Stats.HP == 100f);
Assert(wizard.Stats.Evasion == 0f);      // 새 필드
Assert(wizard.Stats.Lifesteal == 0f);    // 새 필드
Assert(wizard.StartActiveSkillId == "SK_A_001");  // FK
Assert(wizard.Grade == 0);

// 3. 스킬 조회
var fireball = gameData.GetSkill("SK_A_001");
Assert(fireball != null);
Assert(fireball.IsAttack);
Assert(fireball.AttackData.SkillCoef == 1.2f);
Assert(fireball.AttackData.BaseCoolTime == 1.5f);
Assert(fireball.AttackData.BaseProj == 1);
Assert(fireball.IconPath == "Icons/Skill/fireball");  // 새 필드

// 4. 패시브 조회
var powerRune = gameData.GetSkill("SK_P_001");
Assert(powerRune.IsPassive);
Assert(powerRune.PassiveData.TargetStat == "ATK");   // 키 통일 확인
Assert(powerRune.PassiveData.ModifyType == "Mult");
Assert(powerRune.PassiveData.ModifyValue == 0.08f);

// 5. 레벨업 누적
float coefLv3 = gameData.GetSkillParam("SK_A_001", "SkillCoef", 3);
// Lv1: 1.2 + Lv2: +0.15 + Lv3: +0.2 = 1.55
Assert(Mathf.Approximately(coefLv3, 1.55f));

// 6. RunState 동작
var runState = new RunState(gameData);
runState.StartRun("Player_001");
Assert(runState.GetStat("ATK") == 12f);   // CSV 헤더 = StatSheet 키 = 동일
Assert(runState.GetStat("HP") == 100f);

// 7. 무기 장착 + 스펙 조회
runState.EquipWeapon(0, "SK_A_001");
var wpnStats = runState.GetWeaponStats(0);
Assert(wpnStats != null);
Assert(wpnStats.SkillCoef == 1.2f);

// 8. 무기 레벨업
runState.LevelUpWeapon(0);  // → Lv.2
wpnStats = runState.GetWeaponStats(0);
Assert(Mathf.Approximately(wpnStats.SkillCoef, 1.35f));  // 1.2 + 0.15

// 9. 룬 modifier 등록 + 스탯 합산
runState.AddModifier(new StatModifier("Rune:힘의룬_Lv1", "ATK", ModifyType.Percent, 0.08f));
Assert(Mathf.Approximately(runState.GetStat("ATK"), 12f * 1.08f));  // 12.96

// 10. 분해 조회
var breakdown = runState.GetBreakdown("ATK");
Assert(breakdown.BaseValue == 12f);
Assert(breakdown.Entries.Count >= 1);

// 11. 무기 속성 전역 보너스 (투사체의 룬)
runState.AddModifier(new StatModifier("Rune:투사체의룬", "BaseProj", ModifyType.Flat, 1f));
wpnStats = runState.GetWeaponStats(0);
// BaseProj(Lv.2: 1) + 전역보너스(1) = 2 확인

// 12. 런 종료
runState.EndRun();
Assert(!runState.IsRunning);
```

---

## 수정 순서 요약

```
Phase 0: 테스트 CSV 3종 생성 (Characters, Skills, SkillLevelUps)
         → Base 접두어 없는 통일된 컬럼명

Phase 1: 데이터 모델 수정
  1-1. BaseStats.cs (필드 교체 — CSV 헤더와 동일한 이름)
  1-2. CharacterSpec.cs (필드 추가)
  1-3. SkillSpec.cs (필드 추가)
  1-4. SkillAttackData.cs (필드 교체)
  1-5. SkillPassiveData.cs (전면 교체)
  1-6. ParamModification.cs (Set 추가)
  1-7. StatConfig.cs (캡 키 동기화)

Phase 2: Repository 파싱 수정
  2-1. CsvCharacterRepository.cs (StripBasePrefix 제거, 새 필드 파싱)
  2-2. CsvSkillRepository.cs (헤더명 맞춤 + 새 필드 파싱 + PassiveData 수정)
  2-3. GameData.cs (Set 케이스 추가)

Phase 3: Runtime 수정
  3-1. StatSheet.cs (RecalculateAll 키 범위 확장)
  3-2. WeaponResolvedStats.cs (전역 보너스 + 필드명 동기화)

Phase 4: end-to-end 검증 스크립트 작성 + 실행
```

**원칙**:
- 아키텍처/패턴/폴더 구조는 유지
- 데이터 모델과 파싱만 실제 CSV에 맞추는 작업
- CSV 헤더 = 코드 필드명 = StatSheet 키 → 변환 로직 없이 직접 매칭
