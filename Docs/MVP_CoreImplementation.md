# DungeonRush MVP 코어 게임플레이 구현 계획

## 목표
플레이어 움직임, 적 스폰, 자동 전투가 동작하는 MVP 코어 구현

## 구현 범위
- **Core**: GameManager, GameEventBus, ObjectPool, Singleton
- **Character**: PlayerController, EnemyController, CharacterStats (SPUM 연동)
- **Battle**: AutoBattleController, DamageCalculator

---

## Phase 1: Core 시스템 (의존성 없음)

### 1.1 폴더 생성
```
Assets/02.Scripts/Core/
```

### 1.2 파일 생성 순서

| 순서 | 파일 | 설명 |
|------|------|------|
| 1 | `Singleton.cs` | 제네릭 싱글톤 기반 클래스 |
| 2 | `GameState.cs` | 게임 상태 Enum (None, Menu, Playing, Paused, LevelUp, GameOver) |
| 3 | `DamageType.cs` | 데미지 타입 Enum (Physical, Fire, Poison, True) |
| 4 | `GameEventBus.cs` | 이벤트 중개 시스템 (기존 문서 기반) |
| 5 | `ObjectPool.cs` | 제네릭 오브젝트 풀 + IPoolable 인터페이스 |
| 6 | `GameConfig.cs` | 게임 설정 ScriptableObject |
| 7 | `GameManager.cs` | 기존 파일 수정 - Singleton 상속, 상태 관리 |

### 1.3 핵심 구현 내용

**GameEventBus.cs** (SystemDependency.md 기반):
```csharp
// 필수 이벤트만 먼저 구현
- OnGameStateChanged
- OnPlayerHPChanged
- OnPlayerDeath
- OnEnemySpawned
- OnEnemyKilled
```

---

## Phase 2: Character 시스템 (Core 의존)

### 2.1 폴더 생성
```
Assets/02.Scripts/Character/
Assets/02.Scripts/Character/Interfaces/
```

### 2.2 파일 생성 순서

| 순서 | 파일 | 설명 |
|------|------|------|
| 1 | `IDamageable.cs` | 데미지 수신 인터페이스 |
| 2 | `IAttacker.cs` | 공격 수행 인터페이스 |
| 3 | `IPoolable.cs` | 풀링 대상 인터페이스 |
| 4 | `CharacterState.cs` | 캐릭터 상태 Enum (Idle, Moving, Attacking, Stunned, Dead) |
| 5 | `CharacterStats.cs` | 런타임 스탯 클래스 (DataStructure.md 기반) |
| 6 | `CharacterBase.cs` | 캐릭터 추상 기반 클래스 |
| 7 | `PlayerController.cs` | 플레이어 컨트롤러 (SPUM 연동) |
| 8 | `EnemyController.cs` | 적 컨트롤러 (SPUM 연동, IPoolable) |

### 2.3 SPUM 연동 핵심

**PlayerState 매핑** (SPUM_Prefabs.cs 참조):
```csharp
CharacterState.Idle     → PlayerState.IDLE
CharacterState.Moving   → PlayerState.MOVE
CharacterState.Attacking → PlayerState.ATTACK
CharacterState.Stunned  → PlayerState.DAMAGED
CharacterState.Dead     → PlayerState.DEATH
```

**SPUM_Prefabs 사용**:
```csharp
_spumPrefabs.OverrideControllerInit();
_spumPrefabs.PlayAnimation(PlayerState.IDLE, 0);
```

---

## Phase 3: Battle 시스템 (Character 의존)

### 3.1 폴더 생성
```
Assets/02.Scripts/Battle/
```

### 3.2 파일 생성 순서

| 순서 | 파일 | 설명 |
|------|------|------|
| 1 | `DamageCalculator.cs` | 정적 데미지 계산 클래스 |
| 2 | `AutoBattleController.cs` | 자동 전투 로직 (타겟 탐색, 공격 실행) |
| 3 | `BattleMediator.cs` | 활성 적 관리, 유틸리티 메서드 |

### 3.3 자동 전투 로직

```
1. 가장 가까운 적 탐색 (주기적, 매 프레임 X)
2. 공격 범위 판정
3. 공격 쿨다운 관리
4. 데미지 계산 및 적용
5. 적 사망 시 이벤트 발행
```

---

## Phase 4: ScriptableObject 데이터

### 4.1 폴더 생성
```
Assets/10.ScriptableObjects/
Assets/10.ScriptableObjects/Config/
Assets/10.ScriptableObjects/Enemies/
```

### 4.2 파일 생성

| 파일 | 설명 |
|------|------|
| `EnemyData.cs` | 적 데이터 ScriptableObject 정의 |
| `GameConfig.asset` | 플레이어 초기 스탯, 전투 설정 |
| `EN_TestSlime.asset` | 테스트용 적 데이터 |

---

## Phase 5: 씬 구성 및 테스트

### 5.1 GamePlay.unity 씬 수정

1. **GameManager** 오브젝트 생성 및 컴포넌트 추가
2. **Player** 프리팹에 `PlayerController` 컴포넌트 추가
3. **SpawnPoint** 오브젝트 생성 (적 스폰 위치)
4. **EnemyPool** 오브젝트 생성 (ObjectPool 컴포넌트)

### 5.2 테스트 체크리스트

- [ ] GameManager 싱글톤 동작 확인
- [ ] 플레이어 SPUM 애니메이션 재생
- [ ] 적 스폰 및 풀링 동작
- [ ] 자동 전투 타겟팅
- [ ] 데미지 계산 및 HP 감소
- [ ] 적 사망 및 풀 반환

---

## 수정할 파일 목록

### 신규 생성
```
Assets/02.Scripts/Core/Singleton.cs
Assets/02.Scripts/Core/GameState.cs
Assets/02.Scripts/Core/DamageType.cs
Assets/02.Scripts/Core/GameEventBus.cs
Assets/02.Scripts/Core/ObjectPool.cs
Assets/02.Scripts/Core/GameConfig.cs
Assets/02.Scripts/Character/Interfaces/IDamageable.cs
Assets/02.Scripts/Character/Interfaces/IAttacker.cs
Assets/02.Scripts/Character/Interfaces/IPoolable.cs
Assets/02.Scripts/Character/CharacterState.cs
Assets/02.Scripts/Character/CharacterStats.cs
Assets/02.Scripts/Character/CharacterBase.cs
Assets/02.Scripts/Character/PlayerController.cs
Assets/02.Scripts/Character/EnemyController.cs
Assets/02.Scripts/Character/EnemyData.cs
Assets/02.Scripts/Battle/DamageCalculator.cs
Assets/02.Scripts/Battle/AutoBattleController.cs
Assets/02.Scripts/Battle/BattleMediator.cs
```

### 기존 파일 수정
```
Assets/02.Scripts/GameManager.cs → Assets/02.Scripts/Core/GameManager.cs (이동 및 수정)
Assets/03.Prefabs/Player.prefab (PlayerController 컴포넌트 추가)
Assets/01.Scenes/GamePlay.unity (씬 구성)
```

---

## 참조 문서

- `Docs/Architecture/SystemDependency.md` - GameEventBus 이벤트 목록
- `Docs/Architecture/DataStructure.md` - ScriptableObject 및 Enum 정의
- `Assets/10.Resources/SPUM/Core/Script/Data/SPUM_Prefabs.cs` - PlayerState, PlayAnimation()

---

## 구현 순서 요약

```
1. Core 폴더 생성 + Enum 파일들
2. Singleton.cs → GameEventBus.cs → ObjectPool.cs
3. GameConfig.cs (ScriptableObject)
4. GameManager.cs 이동 및 수정
5. Character 폴더 + 인터페이스들
6. CharacterStats.cs → CharacterBase.cs
7. PlayerController.cs (SPUM 연동)
8. EnemyData.cs + EnemyController.cs
9. Battle 폴더 + DamageCalculator.cs
10. AutoBattleController.cs + BattleMediator.cs
11. 씬 구성 및 테스트
```