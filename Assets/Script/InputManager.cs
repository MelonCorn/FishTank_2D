using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum InputState
{
    SpawnFood,
    Erase,
    MiniGame,
}


public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private InputState currentState = InputState.SpawnFood;   // 현재 버튼
    public InputState CurrentState => currentState;

    public event Action OnShopKey;             // 상점 키 눌렀을 때
    public event Action OnFoodKey;             // 먹이 키 눌렀을 때
    public event Action<int> OnScroll;         // 휠 굴릴 때 (값 전달)
    public event Action<Vector3> OnClick;      // 클릭했을 때 (좌표 전달)

    private void Awake()
    {
        Instance = this;
    }


    // 상점 패널 키
    public void OnOpenShop(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            OnShopKey?.Invoke();
        }
    }
    // 먹이 패널 버튼
    public void OnOpenFood(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            OnFoodKey?.Invoke();
        }
    }

    // 먹이 타입 변경
    public void OnChangeType(InputAction.CallbackContext ctx)
    {
        // 먹이 상태 아니면 무시
        if (currentState != InputState.SpawnFood) return;

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

    // 상호작용
    public void OnInteraction(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            // UI 위에 마우스가 있다면 클릭 무시
            if (EventSystem.current.IsPointerOverGameObject()) return;

            // 마우스 위치
            Vector2 screenPosition = Mouse.current.position.ReadValue();

            // 월드 위치
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            // z는 사용하지 않음
            worldPosition.z = 0f;

            // 먹이 활성화
            OnClick?.Invoke(worldPosition);
        }
    }
}
