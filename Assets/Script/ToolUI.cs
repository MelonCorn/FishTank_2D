using System.Collections.Generic;
using UnityEngine;

public class ToolUI : GridUI, ICreateButton
{
    [SerializeField] FoodManager foodManager;
    [Header("도구 리스트")]
    [SerializeField] ToolData cleanToolData;

    [Header("선택 하이라이트")]
    [SerializeField] RectTransform selectedEdge;    // 선택 오브젝트

    private int currentIndex = 0;           // 몇 번째 도구인지


    private void OnEnable()
    {
        InputManager.Instance.OnScroll += OnScroll;
    }
    private void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnScroll -= OnScroll;
    }


    private void Start()
    {
        // 0 번 툴 사용
        SelectTool(0);
    }

    // 버튼 생성
    public void CreateButtons()
    {
        // 전체 도구 데이터 임시
        List<ItemData> tools = new ();
        // 먹이 데이터들 넣고
        tools.AddRange(foodManager.FoodData);
        // 맨 뒤에 청소 도구 데이터 추가
        tools.Add(cleanToolData);             

        // 생성 실행 (SO배열, 함수)
        GenerateButtons(
            datas : tools.ToArray(),
            clickAction : SelectTool,
            hoverAction : OnHover
            );
    }
    
    // 휠 굴렸을 때
    private void OnScroll(int dir)
    {
        // 입력 방향 사용해서 다음 번호
        int nextIndex = currentIndex + dir;

        // 0 미만이면 끝으로
        if (nextIndex < 0)
            nextIndex = buttons.Count - 1;
        // 버튼 길이 넘어서면 0 번으로
        else if (nextIndex >= buttons.Count)
            nextIndex = 0;

        // 도구 선택
        SelectTool(nextIndex);
    }

    // 도구 선택
    private void SelectTool(int index)
    {
        // 현재 번호
        currentIndex = index;

        // 하이라이트 이동
        UpdateSelection(currentIndex);

        // 이게 먹이인지 도구인지 구분
        if (index < foodManager.FoodData.Length)
        {
            // 먹이 선택
            FoodData selectedFood = foodManager.FoodData[currentIndex];

            // 상태 전환
            InputManager.Instance.ChangeState(InputState.SpawnFood);

            // FoodManager에 먹이 데이터 적용
            foodManager.SetCurrentFood(selectedFood);
        }
        else
        {
            // 상태 전환
            InputManager.Instance.ChangeState(InputState.CleanTool);
        }
    }

    // 하이라이트 영역 갱신
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
