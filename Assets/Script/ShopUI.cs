using UnityEngine;

public class ShopUI : GridUI, ICreateButton
{
    [SerializeField] FishTank fishTank;


    // 물고기 버튼 생성
    public void CreateButtons()
    {
        // 생성 실행 (SO, 함수)
        GenerateButtons(fishTank.FishData, fishTank.AddFish);
    }


}

