using UnityEngine;

public class DecoUI : GridUI, ICreateButton
{
    [Header("장식 리스트")]
    [SerializeField] Deco[] decos;

    // 버튼 생성
    public void CreateButtons()
    {
        ItemData[] decoDatas = new ItemData[decos.Length];

        // 등록된 장식 수만큼 데이터 가져오기
        for (int i = 0; i < decos.Length; i++)
        {
            decoDatas[i] = decos[i].data;
        }

        // 생성
        GenerateButtons(
            datas: decoDatas,
            clickAction: TryBuyDeco,
            hoverAction: OnHover
            );
    }


    // 장싱 버튼 클릭 시 실행
    public void TryBuyDeco(int index)
    {
        Deco deco = decos[index];

        // 이미 활성화 되어있다면 중복 구매 방지
        if (deco.target.activeSelf) return;

        // 구매 시도
        if (GameManager.Instance.TryPurchase(deco.data.cost) == false) return;

        // 비용 지불하면 오브젝트 활성화
        deco.target.SetActive(true);
    }
}
