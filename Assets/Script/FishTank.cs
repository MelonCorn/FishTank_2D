using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class FishTank : MonoBehaviour
{

    private ObjectPool<FishAI> fishPool;           // 물고기 풀

    private float padding = 0.5f;                  // 화면 끝에서 안쪽으로 여백  

    private Vector2 minBounds;                     // 화면 최소 좌표 (왼쪽 아래)
    private Vector2 maxBounds;                     // 화면 최대 좌표 (오른쪽 위)

    [SerializeField] FishAI fishPrefab;            // 물고기 프리팹
    [SerializeField] FishData[] fishData;          // 물고기 데이터 목록
    [SerializeField] int defaultSize = 100;        // 초기화 수

    [SerializeField] TextMeshProUGUI fishCountText;// 물고기 수 텍스트


    public float Padding => padding;
    public Vector2 MinBounds => minBounds;
    public Vector2 MaxBounds => maxBounds;



    void Awake()
    {
        InitPool();
    }

    private void Start()
    {
        // 화면 경계 설정
        SetCameraBounds();
    }

    // 풀 초기화
    void InitPool()
    {
        // 풀 생성
        fishPool = new ObjectPool<FishAI>(
            createFunc: CreateNewFish,
            actionOnGet: (fish) => fish.gameObject.SetActive(true),
            actionOnRelease: (fish) => fish.gameObject.SetActive(false),
            actionOnDestroy: (fish) => Destroy(fish),
            defaultCapacity: defaultSize
            );

        // 임시 리스트
        // Get 하고 바로 Release 하면 1개만 사용됨
        List<FishAI> tempFishList = new List<FishAI>();

        // 물고기 시작 수량 생성
        for (int i = 0; i < defaultSize; i++)
        {
            // 풀에 물고기 생성
            var fish = fishPool.Get();

            // 임시 리스트에 추가
            tempFishList.Add(fish);
        }

        // 임시 리스트에 있는 모든 오브젝트 비활성화
        foreach (var fish in tempFishList)
            fishPool.Release(fish);
    }

    // 풀 물고기 생성
    FishAI CreateNewFish()
    {
        FishAI fish = Instantiate(fishPrefab, transform);

        fish.Init(this);

        return fish;
    }

    // 물고기 풀에 반환
    public void ReturnToPool(FishAI fish)
    {
        fishPool.Release(fish);

        fishCountText.text = "Fish Count : " + fishPool.CountActive.ToString();
    }

    // 물고기 생성 버튼
    public void AddFish(Vector3 worldPosition)
    {
        // 풀에서 물고기 가져옴
        FishAI newFish = fishPool.Get();

        // 물고기 위치 지정
        newFish.transform.position = worldPosition;

        // 랜덤 물고기 데이터 선정
        int rand = Random.Range(0, fishData.Length);

        // 데이터 초기화
        newFish.InitFishType(fishData[rand]);

        // 물고기 수 텍스트 갱신
        fishCountText.text = "Fish Count : " + fishPool.CountActive.ToString();
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
