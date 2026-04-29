# 🐟 FishTank

> **2D 힐링 경영 시뮬레이션**

자신만의 작은 바다를 가꾸는 게임입니다.

물고기들에게 먹이를 주고 배설물을 치우며 수조를 관리하세요.

다양한 물고기와 장식을 수집해 나만의 수조를 완성할 수 있습니다.

<br>

| 기간 | 인원 | 엔진 |
|------|------|------|
| 2025.11.21 ~ 11.27 (7일) | 1인 | Unity 2022.3.61f1 · URP |

<br>

## 시연 영상

[![YouTube](https://img.shields.io/badge/YouTube-%23FF0000.svg?style=for-the-badge&logo=YouTube&logoColor=white)](https://youtu.be/pmbKJmH2m2I)

<p align="center">
  <img src="https://github.com/user-attachments/assets/5e7106fd-ce99-4dec-b7e8-39380d3a147b" width="32%" alt="Image 1" />
  <img src="https://github.com/user-attachments/assets/72baaf0f-759b-45d3-8a80-6113f12b2065" width="32%" alt="Image 2" />
  <img src="https://github.com/user-attachments/assets/cbdfb36b-ba57-42ed-b8ec-7e46c61f75e1" width="32%" alt="Image 3" />
</p>

<br>

## 📌 목차

- [게임 소개](#-게임-소개)
- [조작](#-조작)
- [게임 플로우](#-게임-플로우)
- [핵심 구현](#-핵심-구현)
  - [물고기 FSM](#1-물고기-fsm)
  - [ScriptableObject 기반 아이템 데이터](#2-scriptableobject-기반-아이템-데이터)
  - [오브젝트 풀링](#3-오브젝트-풀링)
  - [통합 입력 관리자](#4-통합-입력-관리자)
  - [ISaveable 저장 시스템](#5-isaveable-저장-시스템)
  - [GridUI 버튼 생성 시스템](#6-gridui-버튼-생성-시스템)
- [Prefab vs ScriptableObject](#prefab-vs-scriptableobject)
- [트러블슈팅](#-트러블슈팅)
- [회고](#-회고)

<br>

---

## 🎮 게임 소개

| 핵심 요소 | 설명 |
|----------|------|
| **물고기 수집 & 성장** | 다양한 종류의 물고기를 구매하고 경험치를 쌓아 성장시킴 |
| **배고픔 & 먹이** | 물고기는 시간이 지나면 허기를 느끼고 먹이를 주지 않으면 사망 |
| **배설물 & 수조 청소** | 물고기가 배설물을 남기며 청소 도구로 수조를 청결하게 유지 |
| **재화 & 상점** | 인게임 재화로 상점에서 물고기 · 먹이 · 장식을 구매 |

<br>

---

## 🕹️ 조작

| 키 | 동작 |
|---|---|
| 마우스 좌클릭 | 상호작용 (먹이 투척 / 배설물 청소) |
| Space / 숫자 1 | 상점 탭 열기/닫기 |
| 마우스 우클릭 / 숫자 2 | 도구 탭 열기/닫기 |
| 휠 스크롤 | 도구 변경 |
| Esc | 설정 창 열기/닫기 |

<br>

---

## ⚙️ 핵심 구현

### 1. 물고기 FSM

**문제** 물고기가 Idle·ChaseFood·Dead 등 다양한 상태에 따른 행동을 표현해야 하는데,  
상태마다 `Update`에 조건문을 두면 코드가 폭발적으로 복잡해짐

**해결** `FishState` Enum + `ChangeState()` 함수로 유한 상태 머신 구현

| 상태 | 동작 |
|------|------|
| `Idle` | `WaitUntil`로 배고픔 체크 후 `Physics2D.OverlapCircle`로 먹이 감지 |
| `ChaseFood` | 먹이 `Transform`을 목표로 추적 이동 |
| `Dead` | 사망 연출(`Sink`) 후 오브젝트 풀 반납 |

→ 상태 전환 로직 통일 / 코루틴 1개 재사용으로 GC Alloc 최소화

<br>

### 2. ScriptableObject 기반 아이템 데이터

**문제** 물고기·먹이·장식·도구가 늘어날수록 프리팹이 기하급수적으로 늘어나 관리 불가

**해결** `ItemData(SO)` 상속 계층으로 인스펙터에서 직관적으로 데이터를 추가·수정

```csharp
public class ItemData : ScriptableObject { ... }
public class FishData : ItemData { ... }
public class FoodData : ItemData { ... }
public class ToolData : ItemData { ... }
```

상점 시스템은 `ItemData[]` 목록만 참조하므로 새 아이템 추가 시 자동 연동

→ 새 아이템 추가 = SO 에셋 생성만으로 완료 / 상점·버튼·저장 모두 단일 인터페이스로 자동 연동

<br>

### 3. 오브젝트 풀링

**문제** 물고기·먹이·배설물의 빈번한 `Instantiate/Destroy` → 프레임마다 GC 스파이크 발생 위험

**해결** `Queue<GameObject>` 기반 풀 구현, 배설물 생성 코루틴의 `WaitForSeconds`도 캐싱

→ `Instantiate/Destroy` 호출 제거 / 풀 크기 = 최대 동시 활성 수로 메모리 예측 가능

<br>

### 5. ISaveable 저장 시스템

**문제** 저장 대상(물고기·장식·배설물·재화)이 늘어날수록 `SaveManager`가 모든 대상을  
구체적으로 알게 되어 코드 복잡도·결합도 증가

**해결** `ISaveable` 인터페이스 + JSON 직렬화

물고기·배설물처럼 활성화된 오브젝트 수집은 제네릭 함수로 일반화

→ 새 저장 대상 추가 = `ISaveable` 구현만으로 완료 / `SaveManager` 코드 무수정, 결합도 감소

<br>

### 6. GridUI 버튼 생성 시스템

**문제** 상점 탭·도구 탭이 각각 그리드 버튼을 생성하는데, 탭마다 따로 구현 시 중복 코드 폭발

**해결** 부모 클래스 `GridUI` + 인터페이스 `ICreateButton` 이중 구조

→ 새 탭 추가 = `GridUI` 상속 + `ICreateButton` 구현만으로 완료 / 버튼 생성 코드 중복 0

<br>

---

### Prefab vs ScriptableObject

| | Prefab | **ScriptableObject ✅** |
|---|---|---|
| 아이템 N종 | 프리팹 N개 생성 | SO 에셋 N개 |
| 런타임 비용 | `Instantiate` 비용 발생 | 없음 |
| 인스펙터 수정 | 가능 | 가능 |
| 상속·다형성 | 어려움 | 계층 구조로 자유롭게 |
| 상점·저장 연동 | 수동 참조 필요 | `ItemData` 단일 인터페이스로 자동 |

**결론** 아이템 종류가 계속 늘어나는 구조에서 Prefab 방식은 관리 비용이 선형 증가.  
SO 상속 계층으로 `ItemData` 단일 인터페이스를 확보하면 상점·풀링·저장 모두 확장 없이 자동 연동됨.

<br>

---

## 🐛 트러블슈팅

### #1 — UI 클릭 시 의도치 않은 수조 상호작용

| | |
|--|--|
| **원인** | UI 버튼 클릭 시 마우스가 수조 위에 겹쳐 있어 버튼 클릭과 동시에 먹이 투척·청소 동작 발생 |
| **해결** | `EventSystem.RaycastAll`로 `IsPointerOverUI()` 구현, 상호작용 시작 전 early return 처리 |
| **결과** | UI 버튼 클릭 시 수조 상호작용 0건 / 이벤트 구조와 분리되어 모든 상호작용에 재사용 가능 |

<br>

### #2 — BGM 메모리 최적화

| | |
|--|--|
| **원인** | `loadType: DecompressOnLoad`로 게임 시작 시 BGM 전체를 메모리에 올려 불필요한 점유 |
| **해결** | `loadType: Streaming`으로 변경하여 재생 중에만 디스크에서 청크 단위로 읽기 |
| **결과** | BGM 전체 메모리 할당 제거 / 재생 중에만 스트리밍으로 메모리 대폭 감소 |

<br>
