using System;
using System.Collections.Generic;
using UnityEngine;

public class GridUI : MonoBehaviour
{
    private UIManager uiManager;
    protected List<GridButton> buttons = new();  // 버튼 목록

    [Header("버튼 그리드")]
    [SerializeField] protected Transform buttonGrid;



    private void Awake()
    {
       uiManager = GetComponent<UIManager>();
    }

    // ItemData 타입 SO 처리
    protected void GenerateButtons(ItemData[] datas, Action<int> clickAction, Action<ItemData, bool> hoverAction)
    {
        for (int i = 0; i < datas.Length; i++)
        {
            // 버튼 생성 , 그리드 자식으로
            GridButton button = Instantiate(uiManager.ButtonPrefab, buttonGrid);

            // 버튼 세팅 (데이터, 번호, 클릭, 호버)
            button.Setting(datas[i], i, clickAction, hoverAction);

            // 목록 추가
            buttons.Add(button);
        }
    }
    // 버튼 호버
    protected void OnHover(ItemData data, bool isActive)
    {
        uiManager.HoverButton(data, isActive);
    }
}
