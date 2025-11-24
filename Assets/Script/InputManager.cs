using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum InputState
{
    None,
    SpawnFood,
    SpawnFish,
}


public class InputManager : MonoBehaviour
{
    [SerializeField] FishTank fishTank;
    [SerializeField] FoodManager foodManager;

    private InputState _currentState = InputState.None;   // 현재 버튼
    public InputState CurrentState => _currentState;


    public void OnSpawnFood(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            _currentState = InputState.SpawnFood;
            SetCurrentStateText();
        }
    }
    public void OnSpawnFish(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            _currentState = InputState.SpawnFish;
            SetCurrentStateText();
        }
    }
    public void OnCancel(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            _currentState = InputState.None;
            SetCurrentStateText();
        }
    }
    public void OnChangeType(InputAction.CallbackContext ctx)
    {
        // 기본 상태라면 무시
        if (_currentState == InputState.None) return;

        // 휠이 굴릴 때만 실행
        if (ctx.performed)
        {
            // 휠 값  위+ / 아래-
            float scrollValue = ctx.ReadValue<float>();

            // 혹시 모를 비정상적 입력 방지
            if (scrollValue == 0) return;

            // 값에 따라 다음, 이전 선택
            int scrollDir = (scrollValue > 0) ? 1 : -1;

            // 타입 변경
            switch (_currentState)
            {
                case InputState.SpawnFood:
                    foodManager.ChangeFood(scrollDir);
                    break;
                case InputState.SpawnFish:
                    fishTank.ChangeFish(scrollDir);
                    break;
            }
        }
    }

    public void OnInteraction(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            // UI 위에 마우스가 있다면 클릭 무시
            //if (EventSystem.current.IsPointerOverGameObject()) return;

            // 마우스 위치
            Vector2 screenPosition = Mouse.current.position.ReadValue();

            // 월드 위치
            Vector3 wolrdPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            // z는 사용하지 않음
            wolrdPosition.z = 0f;


            switch (_currentState)
            {
                case InputState.SpawnFood:
                    OnSpawnFood(wolrdPosition);
                    break;
                case InputState.SpawnFish:
                    OnSpawnFish(wolrdPosition);
                    break;
            }

        }
    }

    // 먹이
    void OnSpawnFood(Vector3 worldPosition)
    {
        foodManager.AddFood(worldPosition);
    }
    // 물고기
    void OnSpawnFish(Vector3 worldPosition)
    {
        fishTank.AddFish(worldPosition);
    }

    // 상태 텍스트 갱신
    void SetCurrentStateText()
    {
        UIManager.Instance.UpdateStateUI(_currentState.ToString());
    }

}
