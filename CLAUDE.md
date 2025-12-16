# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 프로젝트 개요
- **엔진**: Unity 6000.0.60f1 (LTS)
- **장르**: Idle + 로그라이크 (자동 전투)
- **플랫폼**: Android (세로, 9:19 · 1080×2280)
- **언어**: C#
- **렌더링**: URP (Universal Render Pipeline)

---

## 0. Claude Code Assistant 지침

* **리뷰 언어:** 모든 코드 리뷰 요약 및 코멘트를 **한국어(Korean)**로 작성합니다. 명확하고 자연스러운 한글 설명을 사용합니다.
* **디자인 원칙 검토:** 모든 PR에 대해 **SOLID 원칙**과 **디미터의 법칙(Law of Demeter)** 위반 여부를 중점적으로 확인합니다.
* **함수:** 함수는 한 가지 일만 합니다.

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

---

## 1. 프로젝트 구조

```
Assets/
├── 01.Scenes/          # 씬 파일
├── 02.Scripts/         # C# 스크립트
│   ├── Battle/         # 전투 시스템
│   ├── Character/      # 플레이어/적 캐릭터
│   ├── Skill/          # 스킬 시스템
│   ├── Stage/          # 스테이지/웨이브 관리
│   ├── UI/             # UI 시스템
│   ├── Upgrade/        # 레벨업/강화 시스템
│   └── Core/           # 게임 매니저, 유틸리티
├── 03.Prefabs/         # 프리팹
├── 04.Images/          # 이미지 에셋
├── 05.Sprites/         # 2D 스프라이트 (SPUM 등)
├── 06.Sounds/          # 사운드 에셋
├── 07.Animations/      # 애니메이션
├── 08.Fonts/           # 폰트
├── 09.Materials/       # 머티리얼
├── 10.ScriptableObjects/ # 데이터 에셋
└── Plugins/            # 외부 플러그인 (DOTween 등)
```

---

## 2. 게임 시스템 개요

### 코어 루프
1. 스테이지 시작 → 적 스폰 → 자동전투
2. 처치 시 XP/드랍 획득 → 임계치 도달 시 레벨업 카드 선택
3. 빌드업(스킬/스탯) → 보스 처치 → 다음 챕터
4. HP 0 → 점수 화면(랭킹 연동) → 재도전

### 스테이지 구조
- **챕터**: 무한 상승 (1, 2, 3...)
- **웨이브**: 챕터당 10개 (1-1 ~ 1-10)
- **일반 웨이브** (1~9): 몬스터 10마리 처치 목표
- **보스 웨이브** (10): 보스 1체 처치

### 템포 설정
| 구분 | 지연 시간 |
|------|----------|
| 웨이브 시작 | 0.5초 |
| 일반 몬스터 스폰 간격 | 0.5초 |
| 보스 웨이브 진입 | 1.0초 |

---

## 3. 구현 시스템

### 전투 시스템 (`02.Scripts/Battle/`)
| 스크립트 | 설명 |
|----------|------|
| `AutoBattleController.cs` | 자동 전투 로직 관리 |
| `DamageCalculator.cs` | 데미지 계산 (기본/크리티컬/속성) |
| `ProjectileManager.cs` | 투사체 생성 및 관리 |

### 캐릭터 시스템 (`02.Scripts/Character/`)
| 스크립트 | 설명 |
|----------|------|
| `PlayerController.cs` | 플레이어 상태 및 스탯 관리 |
| `EnemyController.cs` | 적 행동 패턴 |
| `BossController.cs` | 보스 전용 패턴 |
| `CharacterStats.cs` | HP, ATK 등 스탯 데이터 |

### 스테이지 시스템 (`02.Scripts/Stage/`)
| 스크립트 | 설명 |
|----------|------|
| `StageManager.cs` | 챕터/웨이브 진행 관리 |
| `WaveController.cs` | 웨이브별 적 스폰 로직 |
| `SpawnManager.cs` | 몬스터 스폰 처리 |

### 스킬 시스템 (`02.Scripts/Skill/`)
| 스크립트 | 설명 |
|----------|------|
| `SkillBase.cs` | 스킬 기본 클래스 (추상) |
| `ActiveSkill.cs` | 액티브 스킬 (게일 슬래시 등) |
| `BuffSkill.cs` | 버프 스킬 (화염/맹독 부여) |
| `UltimateSkill.cs` | 궁극기 (용맹의 일격) |
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
| 버프 | 화염 부여 | 3초 화상(초당 10%ATK), 중첩×2 | 10초 |
| 버프 | 맹독 부여 | 4초 독(초당 6%ATK, 최대5중첩) | 12초 |
| 버프 | 흡혈 | 5초간 피해의 12% 회복 | 14초 |
| 궁극 | 용맹의 일격 | 차지 2s, 광역 400% + ATK+25%(3s) | - |

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

## 11. 개발 체크리스트

### 핵심 시스템
- [ ] 자동 전투 시스템
- [ ] 스테이지/웨이브 진행
- [ ] 몬스터 스폰 시스템
- [ ] 레벨업/카드 선택 시스템
- [ ] 스킬 시스템 (액티브/버프/궁극)
- [ ] 보스 전투

### UI
- [ ] HUD (HP바, XP바, 스테이지 표시)
- [ ] 스킬 버튼 (쿨다운, 잠금 상태)
- [ ] 레벨업 카드 선택 팝업
- [ ] 게임오버 결과 화면
- [ ] 랭킹 시스템

### BM (수익화)
- [ ] 전면 광고 (빈도/길이/스킵 규칙)
- [ ] 광고 제거 (패스/영구/기간제)
- [ ] 배속 (×2/×3 판매)
- [ ] 물약/소모품 (번들/쿨다운)
- [ ] 골드 (가격 테이블/밸런스)

### 최적화
- [ ] 오브젝트 풀링
- [ ] GC Alloc 최소화
- [ ] 모바일 성능 프로파일링