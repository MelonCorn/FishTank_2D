using System.Collections.Generic;
using UnityEngine;

public class FoodUI : GridUI, ICreateButton
{
    [SerializeField] FoodManager foodManager;       
    [SerializeField] RectTransform selectedEdge;    // 선택 오브젝트

    private void OnEnable()
    {
        // 먹이 변경 구독
        foodManager.OnFoodChanged += UpdateSelection;
    }
    private void OnDisable()
    {
        // 먹이 변경 구독 해지
        foodManager.OnFoodChanged -= UpdateSelection;
    }


    // 먹이 버튼 생성
    public void CreateButtons()
    {
        // 상속 받은 제네릭 함수
        // <ScriptableObject>
        // SO 배열, 버튼 세팅 람다식
        GenerateButtons<FoodData>(
            foodManager.FoodData,
            (index, food, button) =>
            {
                button.Setting(index, food.sprite, food.foodName, food.cost, foodManager.OnFoodClick);
            });
    }


    // 선택 영역 갱신
    void UpdateSelection(int index)
    {
        // 선택된 버튼의 위치로 테두리 이동
        // 부모 변경
        selectedEdge.SetParent(buttons[index].transform, false);
        // 1번으로
        selectedEdge.SetAsFirstSibling();
        // 정중앙
        selectedEdge.anchoredPosition = Vector2.zero;
        // 활성화
        selectedEdge.gameObject.SetActive(true);
    }
}
