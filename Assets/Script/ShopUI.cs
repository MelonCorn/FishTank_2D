using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] FishTank fishTank;


    private List<PanelButton> foodButtons = new();  // 버튼 목록

    [SerializeField] Transform fishGrid;        // 물고기 버튼 패널
    [SerializeField] PanelButton buttonPrefab;  // 버튼 프리팹


    private void Start()
    {
        // 버튼 생성
        CreateFishButton();
    }


    // 물고기 버튼 생성
    void CreateFishButton()
    {
        // 물고기 데이터 수 만큼
        for (int i = 0; i < fishTank.FishData.Length; i++)
        {
            // 버튼 생성
            PanelButton button = Instantiate(buttonPrefab, fishGrid);

            FishData data = fishTank.FishData[i];

            // 버튼 세팅
            button.Setting(i, data.growthSprite, data.fishName, data.cost, fishTank.AddFish);

            // 리스트에 추가
            foodButtons.Add(button);
        }
    }


}

