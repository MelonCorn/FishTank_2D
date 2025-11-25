using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridButton : MonoBehaviour
{

    [SerializeField] Image iconImg;                 // 아이콘
    [SerializeField] TextMeshProUGUI nameText;      // 이름 텍스트
    [SerializeField] TextMeshProUGUI costText;      // 가격 텍스트
    Button button;                                  // 버튼

    private int indexNumber;                        // 번호
    private ItemData data;                          // 데이터


    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Setting(ItemData data, int index, Action<int> callback)
    {
        // 번호 지정
        indexNumber = index;

        // 데이터 연동
        iconImg.sprite = data.sprite;
        nameText.text = data.itemName;

        // 비용 0 이면 비우기
        if(data.cost == 0)
            costText.text = "";
        else
            costText.text = data.cost.ToString();

        // 버튼 리스너 연결 (캡처)
        button.onClick.AddListener(() => callback?.Invoke(indexNumber));
    }
}
