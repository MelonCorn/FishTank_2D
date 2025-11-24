using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum InputState
{
    SpawnFood,
    Erase,
}


public class InputManager : MonoBehaviour
{
    [SerializeField] FishTank fishTank;
    [SerializeField] FoodManager foodManager;

    private InputState currentState = InputState.SpawnFood;   // 현재 버튼
    public InputState CurrentState => currentState;


    // 먹이 소환 버튼
    public void OnSpawnFood(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            currentState = InputState.SpawnFood;
        }
    }
    // 상점 패널 버튼
    public void OnOpenShop(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {

        }
    }
    // 먹이 패널 버튼
    public void OnOpenFood(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {

        }
    }
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

            // 값에 따라 다음, 이전 선택
            int scrollDir = (scrollValue > 0) ? 1 : -1;

            // 타입 변경
            foodManager.ChangeFood(scrollDir);
        }
    }

    public void OnInteraction(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            // UI 위에 마우스가 있다면 클릭 무시
            if (EventSystem.current.IsPointerOverGameObject()) return;

            // 마우스 위치
            Vector2 screenPosition = Mouse.current.position.ReadValue();

            // 월드 위치
            Vector3 wolrdPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            // z는 사용하지 않음
            wolrdPosition.z = 0f;


            switch (currentState)
            {
                case InputState.SpawnFood:
                    OnSpawnFood(wolrdPosition);
                    break;
            }

        }
    }

    // 먹이
    void OnSpawnFood(Vector3 worldPosition)
    {
        foodManager.AddFood(worldPosition);
    }
}
