using System.Collections;
using TMPro;
using UnityEngine;

// 물고기의 상태 정의
public enum FishState
{
    Idle,
    ChaseFood,
}

public class FishAI : MonoBehaviour
{
    private FishState currentState = FishState.Idle; // 현재 상태

    private Camera mainCamera;

    private Vector2 minBounds;                 // 화면 최소 좌표 (왼쪽 아래)
    private Vector2 maxBounds;                 // 화면 최대 좌표 (오른쪽 위)
    [SerializeField] float padding = 0.5f;     // 화면 끝에서 얼마나 안쪽으로 목표를 잡을지 여백

    private Transform foodTarget;              // 발견한 먹이
    private Vector2 movePoint;                 // 이동 포인트
    
    [SerializeField] float startWaitTime = 2f; // 대기 시간
    private float waitTime;                    // 현재 대기 시간

    [SerializeField] float maxHungry = 100f;   // 허기 최대치
    private float currentHungry;               // 현재 허기치

    [SerializeField] float moveSpeed = 2f;          // 이동 속도
    [SerializeField] float chaseMultiplier = 2f;    // 먹이 추적 배율
    [SerializeField] float detectRange = 1.5f;      // 먹이 감지 범위

    private Coroutine detectFoodCoroutine;           // 먹이 감지 코루틴
    private WaitForSeconds detectFoodDelay;          // 코루틴 감지 간격
    [SerializeField] float foodDetectInterval = 0.1f;// 먹이 감지 간격
    [SerializeField] LayerMask foodLayer;            // 먹이 레이어




    void Start()
    {
        mainCamera = Camera.main;

        // 먹이 감지 간격 설정
        detectFoodDelay = new WaitForSeconds(foodDetectInterval);

        // 화면 경계 계산
        SetCameraBounds();

        // 시작 시 Idle 상태
        ChangeState(FishState.Idle);
    }

    void Update()
    {
        // 상태에 따라
        switch (currentState)
        {
            case FishState.Idle:
                IdleMove();
                break;

            case FishState.ChaseFood:
                ChaseFood();
                break;
        }
    }


    // Idle 이동
    void IdleMove()
    {
        // 이동
        transform.position = Vector2.Lerp(transform.position, movePoint, moveSpeed * Time.deltaTime);
        // 방향 보기
        FlipSprite(movePoint);

        // 목적지와 거리 0.1 거리 차이
        if (Vector2.Distance(transform.position, movePoint) < 0.1f)
        {
            // 대기 시간 0초 되면
            if (waitTime <= 0)
            {
                SetRandomTarget();
                waitTime = startWaitTime;
            }
            // 대기 시간 감소
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }

    // 먹이 감지 코루틴
    IEnumerator DetectFood()
    {
        // 현재 상태 Idle 인 동안 반복
        while (currentState == FishState.Idle)
        {
            // 주변 반경 안에 Food 레이어 물체 감지 시도
            Collider2D food = Physics2D.OverlapCircle(transform.position, detectRange, foodLayer);

            // 먹이 발견
            if (food != null)
            {
                // 먹이 추적 상태로 변경
                foodTarget = food.transform;
                ChangeState(FishState.ChaseFood);
            }

            yield return detectFoodDelay;
        }

        // Idle 빠져나왔으니 코루틴 null
        detectFoodCoroutine = null;
    }

    // 먹이 추적
    void ChaseFood()
    {
        // 쫓던 먹이가 사라짐, 거리 벗어남
        // Idle 상태로 전환
        if (foodTarget == null || Vector2.Distance(transform.position, foodTarget.position) > detectRange) 
        {
            ChangeState(FishState.Idle);
            return;
        }

        // 먹이 향해 이동
        Vector2 targetPos = foodTarget.position;
        // 추적 속도 배율

        // 위치가 화면 밖일 가능성 있으므로 Clamp로 제한
        float clampedX = Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(transform.position.y, minBounds.y, maxBounds.y);

        transform.position = Vector2.MoveTowards(new Vector2(clampedX, clampedY), targetPos, moveSpeed * chaseMultiplier * Time.deltaTime);

        // 먹이 쪽 바라보기
        FlipSprite(targetPos); 
    }


    // 먹이 받아 먹음 (Food 에서 호출)
    public void EatFood()
    {
        // 먹이 추적 상태 아니면
        if (currentState != FishState.ChaseFood) return;

        Debug.Log("먹이 먹음");

        // 먹이 없어져서 다시 한가해짐
        ChangeState(FishState.Idle);
    }




    // 상태 변경
    void ChangeState(FishState newState)
    {
        // 이전 상태 정리
        switch (currentState)
        {
            case FishState.Idle:
                // 감지 코루틴이 돌고 있으면 정지
                if (detectFoodCoroutine != null)
                {
                    StopCoroutine(detectFoodCoroutine);
                    detectFoodCoroutine = null;
                }
                break;
            case FishState.ChaseFood:
                // 먹이 타겟 제거
                foodTarget = null;
                break;
        }

        currentState = newState;

        // 새 상태 설정
        switch (currentState)
        {
            case FishState.Idle:
                // 잠깐 서있기
                movePoint = transform.position;
                // 대기 시간 초기화
                waitTime = startWaitTime;
                // 먹이 탐지 코루틴 실행
                detectFoodCoroutine = StartCoroutine(DetectFood());
                break;
            case FishState.ChaseFood:
                break;
        }
    }

    // 활동 범위 초기화
    void SetCameraBounds()
    {
        // 카메라의 높이 절반 (Orthographic Size)
        float vertExtent = mainCamera.orthographicSize;
        // 카메라의 너비 절반 (높이 * 화면비율)
        float horzExtent = vertExtent * mainCamera.aspect;

        // 경계값 설정 (padding만큼 안쪽으로 들임)
        minBounds = new Vector2(mainCamera.transform.position.x - horzExtent + padding,
                                mainCamera.transform.position.y - vertExtent + padding);
        maxBounds = new Vector2(mainCamera.transform.position.x + horzExtent - padding,
                                mainCamera.transform.position.y + vertExtent - padding);
    }
    
    // 새로운 랜덤 목표점 설정
    void SetRandomTarget()
    {
        // 랜덤 포인트 (0 ~ 1) 에 범위만큼
        Vector2 randomOffset = Random.insideUnitCircle * detectRange;

        // 본인 위치에 계산
        Vector2 randomPoint = new Vector2(transform.position.x, transform.position.y) + randomOffset;

        // 위치가 화면 밖일 가능성 있으므로 Clamp로 제한
        float clampedX = Mathf.Clamp(randomPoint.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(randomPoint.y, minBounds.y, maxBounds.y);

        // 최종 목표 설정
        movePoint = new Vector2(clampedX, clampedY);
    }

    // 스프라이트 좌우 반전
    void FlipSprite(Vector2 target)
    {
        if (target.x > transform.position.x)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        else
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
    }
}