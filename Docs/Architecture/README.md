# DungeonRush 아키텍처 설계 문서

## 문서 목록

| 문서 | 설명 |
|------|------|
| [GameArchitecture.md](./GameArchitecture.md) | 전체 게임 아키텍처, 시스템 구조, 구현 우선순위 |
| [SystemStructure.md](./SystemStructure.md) | 상세 시스템 구조, 디자인 패턴, 확장 포인트 |
| [SystemDependency.md](./SystemDependency.md) | 시스템 간 의존성, 이벤트 버스, 시퀀스 다이어그램 |
| [DataStructure.md](./DataStructure.md) | ScriptableObject 정의, Enum, 런타임 데이터 |

---

## 아키텍처 요약

### 레이어 구조

```
┌────────────────────────────────────────┐
│         Presentation Layer (UI)         │
├────────────────────────────────────────┤
│          Game Logic Layer              │
│  (Stage, Battle, Skill, Upgrade)       │
├────────────────────────────────────────┤
│           Entity Layer                 │
│   (Player, Enemy, Boss, Projectile)    │
├────────────────────────────────────────┤
│            Data Layer                  │
│        (ScriptableObjects)             │
├────────────────────────────────────────┤
│        Infrastructure Layer            │
│  (GameManager, ObjectPool, Save)       │
└────────────────────────────────────────┘
```

### 핵심 시스템 (7개)

| 시스템 | 위치 | 핵심 역할 |
|--------|------|----------|
| **Core** | `02.Scripts/Core/` | GameManager, EventBus, ObjectPool |
| **Stage** | `02.Scripts/Stage/` | 챕터/웨이브 진행, 스폰 |
| **Battle** | `02.Scripts/Battle/` | 자동 전투, 데미지 계산 |
| **Character** | `02.Scripts/Character/` | 플레이어, 적, 보스 |
| **Skill** | `02.Scripts/Skill/` | 스킬 시스템 (Active/Buff/Ultimate) |
| **Upgrade** | `02.Scripts/Upgrade/` | 레벨업, 카드 선택, 리롤 |
| **UI** | `02.Scripts/UI/` | HUD, 팝업, 결과 화면 |

### 통신 방식

- **이벤트 버스**: 시스템 간 느슨한 결합
- **직접 참조**: 동일 레이어 내 또는 하위 레이어로만
- **인터페이스**: `IDamageable`, `IAttacker`, `IPoolable`

---

## 구현 순서

### Phase 1: Core Foundation
```
1. Singleton.cs
2. ObjectPool.cs
3. GameManager.cs
4. GameEventBus.cs
```

### Phase 2: Basic Gameplay
```
5. CharacterStats.cs
6. IDamageable.cs / IAttacker.cs
7. PlayerController.cs
8. EnemyController.cs
9. AutoBattleController.cs
10. DamageCalculator.cs
```

### Phase 3: Stage System
```
11. StageManager.cs
12. WaveController.cs
13. SpawnManager.cs
```

### Phase 4: Skill System
```
14. SkillBase.cs
15. SkillSlotManager.cs
16. ActiveSkill.cs
17. BuffSkill.cs
18. UltimateSkill.cs
```

### Phase 5: Progression
```
19. LevelUpManager.cs
20. CardSelectSystem.cs
21. RerollSystem.cs
```

### Phase 6: UI
```
22. HUDManager.cs
23. HPBar.cs / XPBar.cs
24. SkillButtonUI.cs
25. CardSelectUI.cs
26. ScoreResultUI.cs
```

---

## 설계 원칙

### SOLID 준수
- **SRP**: 각 클래스 단일 책임
- **OCP**: 스킬 시스템 확장 (SkillBase 상속)
- **LSP**: Enemy/Boss 치환 가능
- **ISP**: 인터페이스 분리 (IDamageable, IAttacker)
- **DIP**: 이벤트 버스 통한 느슨한 결합

### 디미터의 법칙
- 기차 참사 패턴 금지
- Manager 간 직접 참조 대신 이벤트 사용

### Unity 최적화
- 컴포넌트 캐싱 (GetComponent는 Start에서만)
- 오브젝트 풀링 필수 (적, 투사체, 이펙트)
- GC Alloc 최소화

---

## 다이어그램 범례

```
── : 직접 참조/호출
═══ : 이벤트 발행/구독
──▶ : 단방향 흐름
◀──▶ : 양방향 통신
```

---

## 변경 이력

| 날짜 | 버전 | 설명 |
|------|------|------|
| 2024-XX-XX | 1.0 | 초기 설계 문서 작성 |