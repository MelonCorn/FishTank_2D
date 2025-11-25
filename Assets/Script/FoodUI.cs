using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodUI : MonoBehaviour
{
    private FoodManager foodManager;

    private List<PanelButton> foodButtons = new();

    [SerializeField] Transform foodGrid;            // 먹이 버튼 패널
    [SerializeField] PanelButton foodButtonPrefab;  // 먹이 버튼 프리팹
    [SerializeField] RectTransform selectedEdge;    // 선택 오브젝트

    private void Awake()
    {
        foodManager = GetComponent<FoodManager>();
    }

    private void OnEnable()
    {
        // 먹이 변경 구독
        foodManager.OnFoodChanged += UpdateSelection;

        // 먹이 버튼 생성
        CreateFoodButton();
    }
    private void OnDisable()
    {
        // 먹이 변경 구독 해지
        foodManager.OnFoodChanged -= UpdateSelection;
    }

    // 먹이 버튼 생성
    void CreateFoodButton()
    {
        // 먹이 데이터 수 만큼
        for (int i = 0; i < foodManager.FoodData.Length; i++)
        {
            // 버튼 생성
            PanelButton button = Instantiate(foodButtonPrefab, foodGrid);

            FoodData data = foodManager.FoodData[i];

            // 버튼 세팅
            button.Setting(i, data.sprite, data.foodName, data.cost, foodManager.OnFoodClick);

            // 리스트에 추가
            foodButtons.Add(button);
        }
    }


    // 선택 영역 갱신
    void UpdateSelection(int index)
    {
        // 선택된 버튼의 위치로 테두리 이동
        // 부모 변경
        selectedEdge.SetParent(foodButtons[index].transform, false);
        // 1번으로
        selectedEdge.SetAsFirstSibling();
        // 정중앙
        selectedEdge.anchoredPosition = Vector2.zero;
        // 활성화
        selectedEdge.gameObject.SetActive(true);
    }

}
