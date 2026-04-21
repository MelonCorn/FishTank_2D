# FishTank

> 2D 힐링 경영 시뮬레이션 게임

자신만의 작은 바다를 가꾸세요.

물고기들에게 먹이를 주고 배설물을 치우며 수조를 관리합니다.

다양한 물고기와 장식을 수집해 나만의 수조를 완성하세요.

<br>


## 시연 영상

[![YouTube](https://img.shields.io/badge/YouTube-%23FF0000.svg?style=for-the-badge&logo=YouTube&logoColor=white)](https://youtu.be/pmbKJmH2m2I)

<br>

---

## 개발 정보

| 항목 | 내용 |
|---|---|
| 개발 기간 | 2025.11.21 ~ 11.27 (7일) |
| 개발 인원 | 1인 |
| 개발 환경 | Unity 2022.3.61f1 · URP |
| 언어 | C# |

<br>

---

## 게임 소개

| 시스템 | 설명 |
|---|---|
| 🐟 물고기 수집 & 성장 | 다양한 종류의 물고기를 구매하고 경험치를 쌓아 성장시킴 |
| 🍖 배고픔 & 먹이 | 물고기는 시간이 지나면 허기를 느끼고 먹이를 주지 않으면 사망 |
| 🧹 배설물 & 수조 청소 | 물고기가 배설물을 남기며 청소 도구로 수조를 청결하게 유지 |
| 💰 재화 & 상점 | 인게임 재화로 상점에서 물고기 · 먹이 · 장식을 구매 |

<br>

---

## 조작

| 키 | 동작 |
|---|---|
| 마우스 좌클릭 | 상호작용 (먹이 투척 / 배설물 청소) |
| Space / 숫자 1 | 상점 탭 열기/닫기 |
| 마우스 우클릭 / 숫자 2 | 도구 탭 열기/닫기 |
| 휠 스크롤 | 도구 변경 |
| Esc | 설정 창 열기/닫기 |

<br>

---

## 핵심 기술

### 🐠 물고기 FSM
물고기는 `FishState` Enum과 `ChangeState()` 함수로 유한 상태 머신을 구현

- **Idle** : `WaitUntil`로 배고픔 체크 후 먹이 감지
- **ChaseFood** : 먹이 `Transform`을 목표로 추적 이동
- **Dead** : 사망 연출 후 오브젝트 풀 반납

<br>

---

### 📦 ScriptableObject 기반 아이템 데이터
`ItemData(SO)` 상속 계층으로 물고기·먹이·도구 데이터를 인스펙터에서 직관적으로 관리

```csharp
public class ItemData : ScriptableObject { ... }
public class FishData : ItemData { ... }
public class FoodData : ItemData { ... }
public class ToolData : ItemData { ... }
```

상점 시스템은 `ItemData[]` 목록만 참조하므로 새 아이템 추가 시 SO 에셋 생성만으로 자동 연동

<br>

---

### ♻️ 오브젝트 풀링
물고기·먹이·배설물의 빈번한 생성·파괴로 인한 GC 스파이크를 방지

> 배설물 생성 코루틴의 `WaitForSeconds`도 캐싱하여 Coroutine alloc 최소화

<br>

---

### 💾 ISaveable 저장 시스템
저장 대상이 늘어나도 `SaveManager`를 수정하지 않아도 되는 구조

<br>

---

### 🖱️ GridUI 버튼 생성 시스템
부모 클래스 `GridUI` + 인터페이스 `ICreateButton` 이중 구조로 버튼 생성 로직을 공통화

새 탭 추가 = `GridUI` 상속 + `ICreateButton` 구현만으로 완료

<br>

---

## Prefab vs ScriptableObject

| | Prefab | ScriptableObject ✅ |
|---|---|---|
| 아이템 N종 | 프리팹 N개 생성 | SO 에셋 N개 |
| 런타임 비용 | Instantiate 비용 발생 | 없음 |
| 인스펙터 수정 | 가능 | 가능 |
| 상속·다형성 | 어려움 | 계층 구조로 자유롭게 |
| 상점·저장 연동 | 수동 참조 필요 | ItemData 단일 인터페이스로 자동 |

> 아이템 종류가 계속 늘어나는 구조에서 SO 상속 계층으로 `ItemData` 단일 인터페이스를 확보하면  
> 상점·풀링·저장 모두 확장 없이 자동 연동

<br>

---

## 트러블슈팅

### 1. UI 클릭 시 의도치 않은 수조 상호작용
- **원인** : UI 버튼 클릭 시 마우스가 수조 위에 겹쳐 있어 동시에 상호작용 발생
- **해결** : `EventSystem.RaycastAll`로 `IsPointerOverUI()` 구현, 상호작용 시작 전 early return 처리

### 2. BGM 메모리 최적화
- **원인** : `loadType: DecompressOnLoad`로 게임 시작 시 BGM 전체를 메모리에 올려 불필요한 할당
- **해결** : `loadType: Streaming`으로 변경하여 재생 중에만 디스크에서 청크 단위로 읽기

<br>
