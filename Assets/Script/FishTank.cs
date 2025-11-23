using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FishTank : MonoBehaviour
{
    public static FishTank Instance { get; private set; }

    private List<FishAI> fishes = new();           // 전체 물고기 리스트

    private float padding = 0.5f;                  // 화면 끝에서 안쪽으로 여백  
    [HideInInspector] public Vector2 minBounds;    // 화면 최소 좌표 (왼쪽 아래)
    [HideInInspector] public Vector2 maxBounds;    // 화면 최대 좌표 (오른쪽 위)


    [SerializeField] FishAI fish;                  // 물고기 프리팹
    [SerializeField] FishData[] fishData;          // 물고기 데이터 목록
    [SerializeField] TextMeshProUGUI fishCountText;// 물고기 수 텍스트

    public float Padding => padding;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // 화면 경계 설정
        SetCameraBounds();
    }

    // 물고기 생성 버튼
    public void AddFish(Vector3 worldPosition)
    {
        // 물고기 생성
        FishAI newFish = Instantiate(fish, worldPosition, Quaternion.identity, transform);

        // 랜덤 물고기 데이터 선정
        int rand = Random.Range(0, fishData.Length);

        // 데이터 초기화
        newFish.InitFishType(fishData[rand]);
    }


    // 물고기 생성될 때 등록
    public void AddFishList(FishAI fish)
    {
        fishes.Add(fish);
        fishCountText.text = "Fish Count : " + fishes.Count.ToString();
    }

    // 활동 범위 초기화
    void SetCameraBounds()
    {
        // 메인 카메라
        Camera mainCam = Camera.main;

        // 카메라의 높이 절반 (Orthographic Size)
        float vertExtent = mainCam.orthographicSize;

        // 카메라의 너비 절반 (높이 * 화면비율)
        float horzExtent = vertExtent * mainCam.aspect;

        // 경계값 설정 (padding만큼 안쪽으로 들임)
        minBounds = new Vector2(mainCam.transform.position.x - horzExtent + Padding,
                                mainCam.transform.position.y - vertExtent + Padding);
        maxBounds = new Vector2(mainCam.transform.position.x + horzExtent - Padding,
                                mainCam.transform.position.y + vertExtent - Padding);
    }

}
