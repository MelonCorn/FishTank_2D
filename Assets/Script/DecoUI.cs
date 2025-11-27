using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DecoUI : GridUI, ICreateButton, ISaveable
{
    [Header("장식 리스트")]
    [SerializeField] Deco[] decos;


    //---------------------------------------------

    #region 저장 및 불러오기

    // 저장
    public void Save(SaveData saveData)
    {
        // 장식 데이터 저장
        // 모든 장식
        for (int i = 0; i < decos.Length; i++)
        {
            // 그 장식 활성화 상태 저장
            saveData.decoActiveData.Add(decos[i].target.activeSelf);
        }
    }

    // 불러오기
    public void Load(SaveData saveData)
    {
        // 모든 장식은
        for (int i = 0; i < decos.Length; i++)
        {
            // 불러온 데이터의 활성화 상태 적용
            decos[i].target.SetActive(saveData.decoActiveData[i]);
        }
    }

    #endregion

    //---------------------------------------------

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

        // 버튼 비활성화
        buttons[index].GetComponent<Button>().interactable = false;

        // 비용 지불하면 오브젝트 활성화
        deco.target.SetActive(true);
    }


}
