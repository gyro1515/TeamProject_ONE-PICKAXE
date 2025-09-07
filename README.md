# ONE PICKAXE

## 🎮 게임 소개
'ONE PICKAXE'는 위험한 광산에 도전하는 한 광부의 모험을 그린 2D 액션 어드벤처 게임입니다. 플레이어는 단 하나의 곡괭이만을 사용하여 광산의 모든 위험을 헤쳐나가야 합니다. 곡괭이를 휘둘러 적을 물리치고, 던져서 원거리의 적을 공격하거나 벽에 박아 발판으로 삼는 등 다양한 액션을 즐길 수 있습니다.

---

## 👨‍💻 팀원 소개
| 이름 | 담당 파트 |
| :--- | :--- |
| **김희진** | 애니메이션, 레벨 오브젝트, 아이템 상호작용 |
| **이원진** | UI, 적 시스템, 가변 점프, 씬/사운드 매니저, 오브젝트 풀링 |
| **차주원** | 곡괭이 시스템, 플레이어 시스템, XlsxToJson 데이터 변환 툴 |

---

## ✨ 핵심 기능 및 시스템 상세

### 1. 곡괭이 시스템 (Pickaxe System)
*   **구현 핵심**: 상태 머신(FSM) 패턴, 제네릭, 상속
*   **설명**: 곡괭이의 복잡한 상태를 체계적으로 관리하기 위해 FSM 패턴을 도입했습니다. '장착(Equipped)'과 '투척(Thrown)' 두 개의 큰 상태 머신으로 나누고, 그 아래 'Idle', 'Swing', 'Fly', 'Stuck', 'Recall' 등의 세부 상태(`PickaxeBaseState`)를 구현하여 각 상태의 역할을 명확히 분리했습니다. 이 구조는 새로운 기능이 추가되더라도 유연하게 확장할 수 있도록 설계되었습니다.
*   **주요 스크립트**:
    *   `Pickaxe/EquippedPickaxeController.cs`: 플레이어가 곡괭이를 들고 있을 때의 상태 관리.
    *   `Pickaxe/ThrownPickaxeController.cs`: 곡괭이가 던져졌을 때의 상태 관리.
    *   `Pickaxe/PickaxeState/`: `Swing`, `Fly`, `Stuck` 등 곡괭이의 개별 상태 로직.

### 2. 데이터 관리 시스템 (Data Management)
*   **구현 핵심**: Unity Editor 확장, 리플렉션(Reflection), 직렬화(Serialization)
*   **설명**: 기획자가 Excel(Xlsx)로 작성한 게임 데이터를 Unity 에디터에서 버튼 클릭 한 번으로 C# 클래스(`GeneratedData/`)와 Json 파일(`Resources/GeneratedJson/`)로 자동 변환하는 커스텀 툴을 개발했습니다. 이 툴은 리플렉션을 사용해 Excel 시트의 헤더를 분석하고, 그에 맞는 C# 클래스 파일을 동적으로 생성합니다. 이를 통해 데이터 관리 프로세스를 자동화하고 휴먼 에러를 최소화하여 개발자와 기획자 간의 협업 효율을 극대화했습니다.
*   **주요 스크립트**:
    *   `Editor/XlsxToJsonConverter.cs`: Excel 파일을 읽어 C#과 Json으로 변환하는 에디터 툴.
    *   `Manager/DataTableManager.cs`: 변환된 Json 데이터를 파싱하여 게임 내에 제공하는 매니저.
    *   `GeneratedData/`: 변환 과정에서 자동 생성된 데이터 클래스들.

### 3. 적 AI 시스템 (Enemy AI)
*   **구현 핵심**: 비헤이비어 트리 (Behavior Tree)
*   **설명**: 근접 및 원거리 유닛의 다양한 AI 패턴을 구현하기 위해 비헤이비어 트리를 직접 설계했습니다. `Sequence`, `Selector`, `Action` 등 기본 노드를 조합하여 '탐색', '추적', '공격', '대기' 등 복잡한 의사결정 구조를 만들었습니다. 이를 통해 각 적 유닛의 특성에 맞는 고유한 행동 패턴을 부여했습니다.
*   **주요 스크립트**:
    *   `BehaviorTree/`: `SequenceNode`, `SelectorNode`, `ActionNode` 등 BT를 구성하는 기본 노드 클래스.
    *   `Enemy/EnemyController.cs`: 적의 기본 행동 로직과 BT 실행을 관리.
    *   `Enemy/Melee/`, `Enemy/Ranged/`: 근접, 원거리 몬스터의 특화된 행동 노드 및 로직.
    *   `Enemy/DetectionArea.cs`: 플레이어를 감지하는 트리거.

### 4. 게임 관리 시스템 (Managers)
*   **구현 핵심**: 싱글톤 (Singleton) 패턴
*   **설명**: 게임의 핵심 기능들을 모듈화하여 관리하는 다양한 매니저 클래스를 구현했습니다. `SingletonMono<T>` 베이스 클래스를 상속받아 모든 매니저가 싱글톤으로 동작하며, 프로젝트 어디에서든 쉽게 접근하여 사용할 수 있습니다.
*   **주요 스크립트**:
    *   `Manager/GameManager.cs`: 게임의 전반적인 상태(시작, 종료, 점수)를 관리.
    *   `Manager/UIManager.cs`: UI의 생성, 제거, 업데이트를 총괄.
    *   `Manager/SoundManager.cs`: 배경음악, 효과음 재생 및 오브젝트 풀링을 통한 사운드 소스 관리.
    *   `Manager/ProjectileManager.cs`: 원거리 공격 투사체에 대한 오브젝트 풀링 관리.
    *   `GlobalScripts/SceneLoader.cs`: 페이드 효과를 포함한 씬 전환 관리.

### 5. 플레이어 시스템 (Player System)
*   **구현 핵심**: Unity Input System, 컴포지션(Composition)
*   **설명**: Unity의 새로운 Input System을 사용하여 사용자의 입력을 처리하고, `PlayerController`가 이를 받아 캐릭터의 움직임, 점프, 공격 등 다양한 행동을 제어합니다. `Player` 클래스는 데이터, `PlayerController`는 행동, `PlayerAnimation`은 시각적 표현을 담당하도록 역할을 분리하여 코드의 유지보수성을 높였습니다.
*   **주요 스크립트**:
    *   `Player/PlayerController.cs`: 입력 값을 받아 플레이어의 모든 행동을 제어.
    *   `Player/Player.cs`: 플레이어의 체력, 스탯 등 핵심 데이터를 관리.
    *   `Player/PlayerAnimation.cs`: 플레이어의 상태에 따라 적절한 애니메이션을 재생.
    *   `PlayerInput/PlayerInput.inputactions`: 모든 조작 키와 액션을 정의한 파일.

---

## 📂 상세 프로젝트 구조

프로젝트의 주요 에셋은 `Assets` 폴더 내에 있으며, 특히 `Scripts` 폴더는 다음과 같은 체계적인 구조로 설계되었습니다.

```
Assets/
┣ 📂 Animations/      # 캐릭터, 오브젝트 애니메이션 클립 및 컨트롤러
┣ 📂 Data/
┃ ┗ 📂 Xlsx/          # 기획 데이터 (Excel 파일)
┣ 📂 Editor/          # Unity 에디터 확장 스크립트
┃ ┗ 📜 XlsxToJsonConverter.cs
┣ 📂 Externals/       # 외부 아트, 사운드 에셋
┣ 📂 GlobalScripts/   # 씬 로딩, 페이드 효과 등 전역 스크립트
┣ 📂 Prefabs/         # 재사용 가능한 게임 오브젝트 프리팹
┣ 📂 Resources/
┃ ┗ 📂 GeneratedJson/ # Xlsx에서 변환된 데이터 Json 파일
┣ 📂 Scenes/          # 게임 씬 (시작, 스테이지 등)
┣ 📂 Scripts/         # 📜 핵심 게임 로직 스크립트
┃ ┣ 📂 Base/              # 공통 기반 클래스 (SingletonMono, BaseCharacter 등)
┃ ┣ 📂 BehaviorTree/      # 적 AI를 위한 행동 트리 클래스 (Sequence, Selector 등)
┃ ┣ 📂 Enemy/             # 적 캐릭터 관련 (Enemy, EnemyController, Melee, Ranged)
┃ ┣ 📂 GeneratedData/     # Xlsx에서 자동 생성된 데이터 클래스
┃ ┣ 📂 Interfaces/        # 각종 인터페이스 (IDamageable, IItem 등)
┃ ┣ 📂 Item/              # 아이템 관련 (Ore, Potion)
┃ ┣ 📂 LevelObject/       # 상호작용 오브젝트 (DestructibleWall, Door, SpikeTrap)
┃ ┣ 📂 Manager/           # 싱글톤 매니저 (GameManager, UIManager, SoundManager 등)
┃ ┣ 📂 Pickaxe/           # 곡괭이 시스템 (Equipped/Thrown Controller, States)
┃ ┣ 📂 Player/            # 플레이어 관련 (Player, PlayerController, PlayerAnimation)
┃ ┗ 📂 UI/                # UI 패널 스크립트 (UIHUD, UIPause, UIStartMenu 등)
┗ ...
```

---

## 🔧 트러블 슈팅

### 1. 기획-개발 간의 용어 불일치
*   **문제**: '점프 강화'라는 동일한 표현을 두고 기획자는 '버튼을 누르는 시간에 따라 점프 높이가 변하는 가변 점프'를, 개발자는 '힘을 모아 더 높이 뛰는 차징 점프'로 다르게 해석하여 초기 구현이 의도와 다르게 진행되었습니다.
*   **해결**: 기획자의 시연을 통해 의도를 명확히 파악하고, 이후 '가변 점프', '차징 점프'와 같은 용어를 문서화하여 팀 내 용어를 통일했습니다. 이를 통해 명확한 소통의 중요성을 깨달았습니다.

### 2. 트리거 이벤트와 물리 충돌의 불일치
*   **문제**: 근접 공격 시, 이미 공격 범위(Trigger) 안에 들어와 있던 적이 피격되지 않는 현상이 발생했습니다.
*   **원인**: Unity의 `OnTriggerEnter` 이벤트는 콜라이더가 '처음 진입하는 순간'에만 호출되기 때문에, 공격 모션이 시작될 때 이미 범위 안에 있던 적은 감지되지 않았습니다.
*   **해결**: 공격이 시작되는 시점에 `OverlapCollider` 계열의 함수를 사용하여 공격 범위 내에 있는 모든 적을 직접 검출하고 데미지를 주는 방식으로 로직을 변경하여 문제를 해결했습니다.
