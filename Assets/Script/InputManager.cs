using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum InputState
{
    SpawnFood,
    CleanTool,
}


public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private InputState currentState = InputState.SpawnFood;   // 현재 버튼
    public InputState CurrentState => currentState;

    public event Action OnShopKey;                   // 상점 키 눌렀을 때
    public event Action OnFoodKey;                   // 먹이 키 눌렀을 때
    public event Action<int> OnScroll;               // 휠 굴릴 때 (도구용,값 전달)
    public event Action<Vector3> OnInteractionFood;  // 클릭했을 때 (먹이용,좌표 전달)
    public event Action<bool> OnInteractionClean;    // 누를 때, 뗄 때 (청소 도구용, 온오프 전달)

    [SerializeField] LayerMask waterLayer;      // 먹이 클릭가능 레이어
    [SerializeField] CleanTool cleanTool;       // 배설물 청소 도구

    [SerializeField] SoundPanel soundPanel;     // 소리 제어판

    PlayerInput playerInput;                    // 입력 시스템

    private Vector3 MouseWorldPos
    {
        get
        {
            // 스크린 좌표 월드 좌표로 변환
            Vector2 screenPos = Mouse.current.position.ReadValue();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            worldPos.z = 0f;
            return worldPos;
        }
    }            // 마우스 월드 좌표

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        // 청소 모드일 때
        // 사운드 패널 꺼져있을 때
        // 마우스 따라다니게
        if (currentState == InputState.CleanTool && soundPanel.isActive == false)
        {
            // 청소 도구 이동 명령
            cleanTool.transform.position = MouseWorldPos;
        }
    }

    // -------------------------------------------

    #region 패널

    // 상점 패널 키
    public void OnOpenShop(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            OnShopKey?.Invoke();
        }
    }

    // 먹이 패널 버튼
    public void OnOpenTool(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            OnFoodKey?.Invoke();
        }
    }

    // 일시 정지
    public void OnPause(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            // 일시 정지
            // 사운드 패널 상태 설정
            SetPause();
        }
    }


    // 사운드 패널 상태 설정
    public void SetPause()
    {
        // 사운드 패널 상태 변경
        soundPanel.SetActivePanel();

        // 활성화
        // Pause 맵으로 변경
        if (soundPanel.isActive == true)
            playerInput.SwitchCurrentActionMap("Pause");
        // 비활성화
        // Player 맵으로 변경
        else
            playerInput.SwitchCurrentActionMap("Player");
    }

    #endregion

    // -------------------------------------------

    #region 상태 변경


    // 상태 변경
    public void ChangeState(InputState newState)
    {
        // 같은 상태면 방지
        if (currentState == newState) return;

        // 상태 적용
        currentState = newState;

        // 청소 도구 활성화
        cleanTool.gameObject.SetActive(currentState == InputState.CleanTool);
    }


    #endregion

    // -------------------------------------------

    #region 휠 스크롤

    // 먹이 타입 변경
    public void OnChangeType(InputAction.CallbackContext ctx)
    {
        // 먹이, 청소 상태 아니면 무시
        if (currentState != InputState.SpawnFood && currentState != InputState.CleanTool) return;

        // 휠이 굴릴 때 실행
        if (ctx.performed)
        {
            // 휠 값  위+ / 아래-
            float scrollValue = ctx.ReadValue<float>();

            // 혹시 모를 비정상적 입력 방지
            if (scrollValue == 0) return;

            // 값에 따라 이전, 다음
            int scrollDir = (scrollValue > 0) ? -1 : 1;

            // 타입 변경
            OnScroll?.Invoke(scrollDir);
        }
    }

    #endregion

    // -------------------------------------------

    #region 마우스 좌클릭

    // 상호작용
    public void OnInteraction(InputAction.CallbackContext ctx)
    {
        switch (currentState)
        {
            case InputState.SpawnFood:
                SpawnFoodState(ctx);
                break;
            case InputState.CleanTool:
                CleanToolState(ctx);
                break;
        }
    }

    // 먹이 상호작용
    void SpawnFoodState(InputAction.CallbackContext ctx)
    {
        // UI 방지
        if (IsPointerOverUI() == true) return;

        if (ctx.started)
        {
            // 마우스 월드 좌표에 먹이 활성화
            OnInteractionFood?.Invoke(MouseWorldPos);
        }
    }


    // 청소 도구 상호작용
    void CleanToolState(InputAction.CallbackContext ctx)
    {
        // 누르기 시작할 떄
        if (ctx.started)
        {
            // UI 방지
            if (IsPointerOverUI() == true) return;
                OnInteractionClean?.Invoke(true);
        }
        // 뗄 때
        else if (ctx.canceled)
            OnInteractionClean?.Invoke(false);

    }

    #endregion

    // -------------------------------------------

    // 마우스 UI 감지
    private bool IsPointerOverUI()
    {
        // 포인터 데이터 정보 생성
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Mouse.current.position.ReadValue();

        // 결과 담을 리스트 생성
        List<RaycastResult> results = new List<RaycastResult>();

        // 위치에 관통해서 검사
        EventSystem.current.RaycastAll(eventData, results);

        // 리스트 하나라도 있으면 UI 감지
        return results.Count > 0;
    }
}