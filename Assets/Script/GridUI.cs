using System;
using System.Collections.Generic;
using UnityEngine;

public class GridUI : MonoBehaviour
{
    protected List<GridButton> buttons = new();  // 버튼 목록

    [SerializeField] protected Transform buttonGrid;     // 버튼 그리드
    [SerializeField] protected GridButton buttonPrefab;  // 버튼 프리팹


    // ItemData 타입 SO 처리
    protected void GenerateButtons(ItemData[] datas, Action<int> setupAction)
    {
        for (int i = 0; i < datas.Length; i++)
        {
            // 버튼 생성 , 그리드 자식으로
            GridButton button = Instantiate(buttonPrefab, buttonGrid);

            // 버튼 세팅
            button.Setting(datas[i], i, setupAction);

            // 목록 추가
            buttons.Add(button);
        }
    }

}
