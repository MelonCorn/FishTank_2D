using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class FoodManager : MonoBehaviour
{
    private ObjectPool<Food> foodPool;            // 먹이 풀

    [SerializeField] Food foodPrefab;             // 먹이 프리팹
    [SerializeField] FoodData[] foodData;         // 먹이 데이터 목록
    [SerializeField] int defaultSize = 20;        // 초기화 수

    void Awake()
    {
        InitPool();
    }
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

    // 먹이 풀에 반환
    public void ReturnToPool(Food fish)
    {
        foodPool.Release(fish);
    }

    // 먹이 생성 버튼
    public void AddFood(Vector3 worldPosition)
    {
        // 풀에서 먹이 가져옴
        Food newFood = foodPool.Get();

        // 먹이 위치 지정
        newFood.transform.position = worldPosition;

        // 데이터 초기화
        newFood.InitFoodType(foodData[0]);
    }
}
