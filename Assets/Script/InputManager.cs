using TMPro;
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
    [SerializeField] GameObject foodPrefab;     // 먹이 프리팹
    [SerializeField] TextMeshProUGUI currentStateText;

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
    public void OnNone(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            _currentState = InputState.None;
            SetCurrentStateText();
        }
    }

    public void OnLMBClick(InputAction.CallbackContext ctx)
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


    void OnSpawnFood(Vector3 worldPosition)
    {
        // 생성
        Instantiate(foodPrefab, worldPosition, Quaternion.identity);
    }
    void OnSpawnFish(Vector3 worldPosition)
    {
        FishTank.Instance.AddFish(worldPosition);
    }

    
    void SetCurrentStateText()
    {
        currentStateText.text = "Current State : " + _currentState.ToString();
    }

}
