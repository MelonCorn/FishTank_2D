using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FishTank : MonoBehaviour
{
    public static FishTank Instance { get; private set; }

    private List<FishAI> fishes = new();

    [SerializeField] FishAI fish;

    [SerializeField] TextMeshProUGUI fishCountText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // 물고기 생성 버튼
    public void AddFish(Vector3 worldPosition)
    {
        Instantiate(fish, worldPosition, Quaternion.identity, transform);
    }


    // 물고기 생성될 때 등록
    public void AddFishList(FishAI fish)
    {
        fishes.Add(fish);
        fishCountText.text = "Fish Count : " + fishes.Count.ToString();
    }
}
