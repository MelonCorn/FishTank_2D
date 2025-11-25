using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] Image iconImg;                 // 아이콘
    [SerializeField] TextMeshProUGUI nameText;      // 이름 텍스트
    [SerializeField] TextMeshProUGUI costText;      // 가격 텍스트
    Button button;                                  // 버튼

    private int indexNumber;                        // 번호
    private ItemData data;                          // 데이터

    private Action<ItemData, bool> OnHover;         // 마우스 올라오면, 내려가면

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Setting(ItemData data, int index, Action<int> onClickCallBack, Action<ItemData, bool> OnHoverCallBack)
    {
        // 데이터 지정
        this.data = data;

        // 번호 지정
        indexNumber = index;

        // 데이터 연동
        iconImg.sprite = data.sprite;
        nameText.text = data.itemName;

        // 비용 0 이면 비우기
        if(data.cost == 0)
            costText.text = "";
        else
            costText.text = $"{data.cost} 원";

        // 호버 이벤트
        OnHover = OnHoverCallBack;

        // 버튼 리스너 연결 (캡처)
        button.onClick.AddListener(() => onClickCallBack?.Invoke(indexNumber));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHover?.Invoke(data, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHover?.Invoke(data, false);
    }
}
