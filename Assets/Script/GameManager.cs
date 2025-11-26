using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    [SerializeField] TextMeshProUGUI fishCountText;     // 물고기 수 텍스트
    [SerializeField] TextMeshProUGUI moneyText;         // 재화 텍스트


    int happiness;               // 어항 행복도
    int money = 1000;            // 재화

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // 시작 하면 한 번 갱신
        UpdateMoneyUI(money);
    }

    // 재화 추가
    public void AddMoney(int amount)
    {
        // 추가
        money += amount;
        // 갱신
        UpdateMoneyUI(money);
    }

    // 결제 시도
    public bool TryPurchase(int cost)
    {
        bool isSuccess = false;

        // 요구보다 많으면
        if (money >= cost)
        {
            // 성공
            isSuccess = true;
            // 감소
            money -= cost;
            // 갱신
            UpdateMoneyUI(money);
        }

        return isSuccess;
    }


    // 물고기 수 갱신 FishTank
    public void UpdateFishCount(int count)
    {
        if (fishCountText != null)
            fishCountText.text = $" : {count}";
    }

    // 재화 갱신
    private void UpdateMoneyUI(int money)
    {
        if (moneyText != null)
            moneyText.text = $"돈 : {money:N0} 원";
    }
}
