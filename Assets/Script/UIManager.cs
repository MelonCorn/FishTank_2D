using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] TextMeshProUGUI currentStateText;  // 현재 인풋 상태
    [SerializeField] TextMeshProUGUI foodTypeText;      // 현재 먹이 텍스트
    [SerializeField] TextMeshProUGUI fishCountText;     // 물고기 수 텍스트
    [SerializeField] TextMeshProUGUI fishTypeText;      // 현재 물고기 텍스트
    [SerializeField] TextMeshProUGUI moneyText;         // 재화 텍스트


    void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        // 재화 변경 알림 구독
        GameManager.Instance.OnMoneyChanged += UpdateMoneyUI;
    }
    private void OnDisable()
    {
        // 재화 변경 알림 구독 해지
        if(GameManager.Instance != null)
            GameManager.Instance.OnMoneyChanged -= UpdateMoneyUI;
    }

    // ---------------------------------------------------------

    //  현재 상태 텍스트 갱신 InputManager
    public void UpdateStateUI(string state)
    {
        if (currentStateText != null)
            currentStateText.text = $"Current Action : {state}";
    }

    // ---------------------------------------------------------

    // 물고기 수 갱신 FishTank
    public void UpdateFishCount(int count)
    {
        if (fishCountText != null)
            fishCountText.text = $"Fish Count : {count}";
    }

    // 선택된 물고기 이름 갱신 FishTank
    public void UpdateFishType(string fishName)
    {
        if (fishTypeText != null)
            fishTypeText.text = $"Current Fish : {fishName}";
    }

    // ---------------------------------------------------------

    // 선택된 먹이 이름 갱신 FoodManager
    public void UpdateFoodType(string foodName)
    {
        if (foodTypeText != null)
            foodTypeText.text = $"Current Food : {foodName}";
    }


    // ---------------------------------------------------------

    // 재화 갱신 자동 호출
    private void UpdateMoneyUI(int money)
    {
        if (moneyText != null)
            moneyText.text = $"Money : {money:N0}";
    }

}
