using UnityEngine;
using static UnityEditor.PlayerSettings.WSA;

public class FishUI : GridUI, ICreateButton
{
    [SerializeField] FishTank fishTank;

    // 滚瓢 积己
    public void CreateButtons()
    {
        // 积己 角青 (SO, 窃荐)
        GenerateButtons(
            datas: fishTank.FishData,
            clickAction: fishTank.AddFish,
            hoverAction: OnHover
            );
    }
}

