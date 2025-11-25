using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FishTank : MonoBehaviour
{
    private ObjectPool<FishAI> fishPool;           // 물고기 풀
    private ObjectPool<Excrement> excrementPool;   // 배설물 풀

    private float padding = 0.5f;                  // 화면 끝에서 안쪽으로 여백  

    private Vector2 minBounds;                     // 화면 최소 좌표 (왼쪽 아래)
    private Vector2 maxBounds;                     // 화면 최대 좌표 (오른쪽 위)

    [Header("물고기")]
    [SerializeField] FishAI fishPrefab;            // 물고기 프리팹
    [SerializeField] FishData[] fishData;          // 물고기 데이터 목록
    [SerializeField] int fishPoolDefaultSize = 100;// 초기화 수

    [Header("배설물")]
    [SerializeField] Excrement excrementPrefab;    // 배설물 프리팹
    [SerializeField] int excrementPoolDefaultSize = 100;// 초기화 수

    public float Padding => padding;
    public Vector2 MinBounds => minBounds;
    public Vector2 MaxBounds => maxBounds;

    public FishData[] FishData => fishData;


    void Awake()
    {
        // 풀 초기화
        InitFishPool();
        InitExcrementPool();
    }

    private void Start()
    {
        // 화면 경계 설정
        SetCameraBounds();
    }

    #region 물고기 풀링

    // 물고기 풀 초기화
    void InitFishPool()
    {
        // 풀 생성
        fishPool = new ObjectPool<FishAI>(
            createFunc: CreateNewFish,
            actionOnGet: (fish) => fish.gameObject.SetActive(true) ,
            actionOnRelease: (fish) => fish.gameObject.SetActive(false),
            actionOnDestroy: (fish) => Destroy(fish),
            defaultCapacity: fishPoolDefaultSize
            );

        // 임시 리스트
        // Get 하고 바로 Release 하면 1개만 사용됨
        List<FishAI> tempFishList = new List<FishAI>();

        // 물고기 시작 수량 생성
        for (int i = 0; i < fishPoolDefaultSize; i++)
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
    public void ReturnToFishPool(FishAI fish)
    {
        fishPool.Release(fish);
        // 물고기 수 텍스트
        SetFishCountText();
    }


    #endregion

    #region 배설물 풀링
    // 배설물 풀 초기화
    void InitExcrementPool()
    {
        // 풀 생성
        excrementPool = new ObjectPool<Excrement>(
            createFunc: CreateNewExcrement,
            actionOnGet: (excrement) => excrement.gameObject.SetActive(true),
            actionOnRelease: (excrement) => excrement.gameObject.SetActive(false),
            actionOnDestroy: (excrement) => Destroy(excrement),
            defaultCapacity: excrementPoolDefaultSize
            );

        // 임시 리스트
        // Get 하고 바로 Release 하면 1개만 사용됨
        List<Excrement> tempList = new List<Excrement>();

        // 시작 수량 생성
        for (int i = 0; i < excrementPoolDefaultSize; i++)
        {
            // 풀에 생성
            var excrement = excrementPool.Get();

            // 임시 리스트에 추가
            tempList.Add(excrement);
        }

        // 임시 리스트에 있는 모든 오브젝트 비활성화
        foreach (var fish in tempList)
            excrementPool.Release(fish);
    }

    // 배설물 생성
    Excrement CreateNewExcrement()
    {
        Excrement excrement = Instantiate(excrementPrefab, transform);

        excrement.Init(this);

        return excrement;
    }

    // 풀 반납
    public void ReturnToExcrementPool(Excrement excrement)
    {
        excrementPool.Release(excrement);
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
        GameManager.Instance.UpdateFishCount(fishPool.CountActive);
    }

    // 배설물 풀 사용
    public void Excretion(Vector3 fishPosition)
    {
        Excrement excrement = excrementPool.Get();

        excrement.transform.position = fishPosition;
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
