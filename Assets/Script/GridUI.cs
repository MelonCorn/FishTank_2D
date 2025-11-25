using System;
using System.Collections.Generic;
using UnityEngine;

public class GridUI : MonoBehaviour
{
    protected List<GridButton> buttons = new();  // 버튼 목록

    [SerializeField] protected Transform buttonGrid;     // 버튼 그리드
    [SerializeField] protected GridButton buttonPrefab;  // 버튼 프리팹


    // 버튼 생성 제네릭 함수
    // 여러 데이터SO 처리 가능
    protected void GenerateButtons<T>(T[] datas, Action<int, T, GridButton> setupAction) where T : ScriptableObject
    {
        for (int i = 0; i < datas.Length; i++)
        {
            // 버튼 생성 , 그리드 자식으로
            GridButton button = Instantiate(buttonPrefab, buttonGrid);

            // 세팅
            T data = datas[i];
            // 람다식 실행
            setupAction(i, data, button);

            // 목록 추가
            buttons.Add(button);
        }
    }

}
