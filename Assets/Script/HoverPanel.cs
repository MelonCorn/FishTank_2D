using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoverPanel : MonoBehaviour
{
    [SerializeField] Image iconImage;               // 아이콘 이미지
    [SerializeField] TextMeshProUGUI nameText;      // 이름
    [SerializeField] TextMeshProUGUI costText;      // 비용
    [SerializeField] TextMeshProUGUI infoText;      // 정보

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetInfo(ItemData data)
    {
        iconImage.sprite = data.sprite;
        nameText.text = data.itemName;
        if(data.cost == 0)
            costText.text = "";
        else
            costText.text = data.cost.ToString();
        infoText.text = data.info;
    }
}
