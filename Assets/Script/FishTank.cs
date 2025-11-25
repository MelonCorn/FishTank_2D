using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FishTank : MonoBehaviour
{
    private ObjectPool<FishAI> fishPool;           // 물고기 풀

    private float padding = 0.5f;                  // 화면 끝에서 안쪽으로 여백  

    private Vector2 minBounds;                     // 화면 최소 좌표 (왼쪽 아래)
    private Vector2 maxBounds;                     // 화면 최대 좌표 (오른쪽 위)

    [SerializeField] FishAI fishPrefab;            // 물고기 프리팹
    [SerializeField] FishData[] fishData;          // 물고기 데이터 목록
    [SerializeField] int defaultSize = 100;        // 초기화 수


    public float Padding => padding;
    public Vector2 MinBounds => minBounds;
    public Vector2 MaxBounds => maxBounds;

    public FishData[] FishData => fishData;


    void Awake()
    {
        InitPool();
    }

    private void Start()
    {
        // 화면 경계 설정
        SetCameraBounds();
    }

    #region 풀링

    // 풀 초기화
    void InitPool()
    {
        // 풀 생성
        fishPool = new ObjectPool<FishAI>(
            createFunc: CreateNewFish,
            actionOnGet: (fish) => fish.gameObject.SetActive(true) ,
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

        // 기본 물고기
        fish.InitFishType(fishData[0]);

        return fish;
    }

    // 물고기 풀 반납
    public void ReturnToPool(FishAI fish)
    {
        fishPool.Release(fish);
        // 물고기 수 텍스트
        SetFishCountText();
    }

    #endregion

    // 물고기 풀 사용
    public void AddFish(int index)
    {
        if (GameManager.Instance.TryPurchase(fishData[index].cost) == false) return;

        // 풀에서 물고기 가져옴
        FishAI newFish = fishPool.Get();

        // 물고기 위치 지정
        newFish.transform.position = new Vector2(Random.Range(minBounds.x, maxBounds.x),Random.Range(minBounds.y, maxBounds.y));

        // 선택된 물고기 데이터로 초기화
        newFish.InitFishType(fishData[index]);

        // 물고기 수 텍스트
        SetFishCountText();
    }

    // 물고기 수 갱신
    void SetFishCountText()
    {
        UIManager.Instance.UpdateFishCount(fishPool.CountActive);
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
