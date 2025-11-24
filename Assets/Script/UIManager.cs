using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] TextMeshProUGUI currentStateText;  // 현재 인풋 상태
    [SerializeField] TextMeshProUGUI foodTypeText;      // 현재 먹이 텍스트
    [SerializeField] TextMeshProUGUI fishCountText;     // 물고기 수 텍스트
    [SerializeField] TextMeshProUGUI fishTypeText;      // 현재 물고기 텍스트


    void Awake()
    {
        Instance = this;
    }
    
    
    // ---------------------------------------------------------

    //  현재 상태 텍스트 갱신 InputManager
    public void UpdateStateUI(string fullText)
    {
        if (currentStateText != null)
            currentStateText.text = fullText;
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

}
