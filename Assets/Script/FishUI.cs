using UnityEngine;
using static UnityEditor.PlayerSettings.WSA;

public class FishUI : GridUI, ICreateButton
{

    [Header("수조 스크립트")]
    [SerializeField] FishTank fishTank;

    // 버튼 생성
    public void CreateButtons()
    {
        // 생성 실행 (SO, 함수)
        GenerateButtons(
            datas: fishTank.FishData,
            clickAction: fishTank.AddFish,
            hoverAction: OnHover
            );
    }
}

