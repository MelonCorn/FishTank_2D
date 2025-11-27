using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    [SerializeField] TextMeshProUGUI fishCountText;     // 물고기 수 텍스트
    [SerializeField] TextMeshProUGUI moneyText;         // 재화 텍스트


    int money;                  // 재화

    public int CurrentMoney
    {
        get
        {
            return money;
        }
        set
        {
            money = value;

            if (moneyText != null)
                moneyText.text = $"돈 : {money:N0} 원";
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        CurrentMoney = 1000;
    }

    // 결제 시도
    public bool TryPurchase(int cost)
    {
        bool isSuccess = false;

        // 요구보다 많으면
        if (CurrentMoney >= cost)
        {
            // 성공
            isSuccess = true;
            // 감소
            CurrentMoney -= cost;
        }

        return isSuccess;
    }


    // 물고기 수 갱신 FishTank
    public void UpdateFishCount(int count)
    {
        if (fishCountText != null)
            fishCountText.text = $" : {count}";
    }
}
