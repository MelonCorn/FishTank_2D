using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class ShopUI : GridUI, ICreateButton
{
    [SerializeField] FishTank fishTank;


    // 물고기 버튼 생성
    public void CreateButtons()
    {
        // 상속 받은 제네릭 함수
        // <ScriptableObject>
        // SO 배열, 버튼 세팅 람다식
        GenerateButtons<FishData>(
            fishTank.FishData,
            (index, fish, button) =>
            {
                button.Setting(index, fish.growthSprite, fish.fishName, fish.cost, fishTank.AddFish);
            });
    }


}

