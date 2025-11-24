using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    public event Action<int> OnHappinessChanged;// 행복도 변경 알림
    public event Action<int> OnMoneyChanged;    // 재화 변경 알림

    int happiness;        // 어항 행복도
    int money = 100;      // 재화
    public int Happiness
    {
        get => happiness;
        private set
        {
            happiness = value;
            // 행복도 변경 알림 방송
            OnHappinessChanged?.Invoke(happiness);
        }
    }
    public int Money
    {
        get => money;
        private set
        {
            money = value;
            // 재화 변경 알림 방송
            OnMoneyChanged?.Invoke(money);
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 시작 하면 한 번 알림
        OnMoneyChanged?.Invoke(money);
    }

    // 재화 추가
    public void AddMoney(int amount)
    {
        Money += amount;
    }

    // 결제 시도
    public bool TryPurchase(int cost)
    {
        bool isSuccess = false;

        // 요구보다 많으면
        if (Money >= cost)
        {
            // 성공
            isSuccess = true;
            // 감소
            Money -= cost;
        }

        return isSuccess;
    }
}
