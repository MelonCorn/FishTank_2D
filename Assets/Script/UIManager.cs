using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] TextMeshProUGUI fishCountText;     // 물고기 수 텍스트
    [SerializeField] TextMeshProUGUI moneyText;         // 재화 텍스트


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
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

    // 물고기 수 갱신 FishTank
    public void UpdateFishCount(int count)
    {
        if (fishCountText != null)
            fishCountText.text = $"Fish Count : {count}";
    }

    // ---------------------------------------------------------

    // 재화 갱신 자동 호출
    private void UpdateMoneyUI(int money)
    {
        if (moneyText != null)
            moneyText.text = $"Money : {money:N0}";
    }

}
