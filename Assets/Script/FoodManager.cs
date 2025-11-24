using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using System;

public class FoodManager : MonoBehaviour
{
    private ObjectPool<Food> foodPool;              // 먹이 풀

    public event Action<int> OnFoodChanged;         // 먹이 변경 시

    [SerializeField] Food foodPrefab;               // 먹이 프리팹
    [SerializeField] FoodData[] foodData;           // 먹이 데이터 목록
    [SerializeField] int defaultSize = 20;          // 초기화 수

    private int currentFood = 0;

    public FoodData[] FoodData => foodData;

    private int CurrentFood
    {
        set
        {
            currentFood = value;
            OnFoodChanged?.Invoke(currentFood);
        }
    }

    void Awake()
    {
        // 풀 초기화
        InitPool();
    }

    private void Start()
    {
        // 처음엔 0번 선택
        OnFoodChanged(0);
    }

    #region 풀링
    // 풀 초기화
    void InitPool()
    {
        // 풀 생성
        foodPool = new ObjectPool<Food>(
            createFunc: CreateNewFood,
            actionOnGet: (food) => food.gameObject.SetActive(true),
            actionOnRelease: (food) => food.gameObject.SetActive(false),
            actionOnDestroy: (food) => Destroy(food),
            defaultCapacity: defaultSize
            );

        // 임시 리스트
        // Get 하고 바로 Release 하면 1개만 사용됨
        List<Food> tempFoodList = new List<Food>();

        // 먹이 시작 수량 생성
        for (int i = 0; i < defaultSize; i++)
        {
            // 풀에 먹이 생성
            var food = foodPool.Get();

            // 임시 리스트에 추가
            tempFoodList.Add(food);
        }

        // 임시 리스트에 있는 모든 오브젝트 비활성화
        foreach (var food in tempFoodList)
            foodPool.Release(food);
    }

    // 풀 먹이 생성
    Food CreateNewFood()
    {
        Food food = Instantiate(foodPrefab, transform);

        food.Init(this);

        return food;
    }

    // 먹이 풀 반납
    public void ReturnToPool(Food fish)
    {
        foodPool.Release(fish);
    }
    #endregion

    // 먹이 풀 사용
    public void AddFood(Vector3 worldPosition)
    {
        // 비용 체크 
        if (GameManager.Instance.TryPurchase(foodData[currentFood].cost) == false) return;

        // 풀에서 먹이 가져옴
        Food newFood = foodPool.Get();

        // 먹이 위치 지정
        newFood.transform.position = worldPosition;

        // 선택된 먹이 데이터로 초기화
        newFood.InitFoodType(foodData[currentFood]);
    }


    // 먹이 변경
    public void ChangeFood(int dir)
    {
        int current = currentFood;

        current += dir;

        // 0 미만이면 마지막으로
        if (current < 0)
            current = foodData.Length - 1;

        // 마지막 이상이면 0 으로
        else if (current >= foodData.Length)
            current = 0;

        // 적용 후 알림
        CurrentFood = current;

        // 선택 먹이 텍스트 갱신
        UIManager.Instance.UpdateFoodType(foodData[current].foodName);
    }

    // 버튼 클릭 시 호출될 함수
    // 선택 먹이 변경
    public void OnFoodClick(int index)
    {
        CurrentFood = index;
    }
}
