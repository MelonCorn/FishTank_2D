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
    FishData fishData;

    private FishState currentState = FishState.Idle; // 현재 상태

    private Transform foodTarget;              // 발견한 먹이
    private Vector2 movePoint;                 // 이동 포인트
    private float waitTime;                    // 현재 대기 시간

    private float currentHungry;               // 현재 허기치

    private Coroutine detectFoodCoroutine;           // 먹이 감지 코루틴
    private WaitForSeconds detectFoodDelay;          // 코루틴 감지 간격


    public void InitFishType(FishData newData)
    {
        this.fishData = newData;
    }


    void Start()
    {
        // 먹이 감지 간격 설정
        detectFoodDelay = new WaitForSeconds(fishData.foodDetectInterval);

        // 시작 시 Idle 상태
        ChangeState(FishState.Idle);

        // 처음엔 바로 이동하게
        waitTime = 0;

        // 어항의 물고기 리스트에 자신 등록
        FishTank.Instance.AddFishList(this);
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
        transform.position = Vector2.Lerp(transform.position, movePoint, fishData.moveSpeed * Time.deltaTime);
        // 방향 보기
        FlipSprite(movePoint);

        // 목적지와 거리 0.1 거리 차이
        if (Vector2.Distance(transform.position, movePoint) < 0.1f)
        {
            // 대기 시간 0초 되면
            if (waitTime <= 0)
            {
                // 랜덤 이동 포인트 지정
                SetRandomTarget();
                // 랜덤 이동 대기 시간 설정
                waitTime = GetRandomWaitTime();
            }
            // 대기 시간 감소
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }

    // 이동 대기 시간 랜덤
    float GetRandomWaitTime()
    {
        return Random.Range(fishData.minWaitTime, fishData.maxWaitTime);
    }

    // 먹이 감지 코루틴
    IEnumerator DetectFood()
    {
        // 현재 상태 Idle 인 동안 반복
        while (currentState == FishState.Idle)
        {
            // 주변 반경 안에 Food 레이어 물체 감지 시도
            Collider2D food = Physics2D.OverlapCircle(transform.position, fishData.detectRange, fishData.foodLayer);

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
        if (foodTarget == null || Vector2.Distance(transform.position, foodTarget.position) > fishData.detectRange) 
        {
            ChangeState(FishState.Idle);
            return;
        }

        // 먹이 향해 이동
        Vector2 targetPos = foodTarget.position;
        // 추적 속도 배율

        // 위치가 화면 밖일 가능성 있으므로 Clamp로 제한
        float clampedX = Mathf.Clamp(transform.position.x, FishTank.Instance.minBounds.x, FishTank.Instance.maxBounds.x);
        float clampedY = Mathf.Clamp(transform.position.y, FishTank.Instance.minBounds.y, FishTank.Instance.maxBounds.y);

        transform.position = Vector2.MoveTowards(new Vector2(clampedX, clampedY), targetPos, fishData.moveSpeed * fishData.chaseMultiplier * Time.deltaTime);

        // 먹이 쪽 바라보기
        FlipSprite(targetPos); 
    }

    // 먹이 받아 먹음 (Food 에서 호출)
    public void EatFood()
    {
        // 먹이 추적 상태 아니면
        if (currentState != FishState.ChaseFood) return;

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
                // 먹이 타겟 제거, 나중에 강제 변환 시 
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
                waitTime = GetRandomWaitTime();
                // 먹이 탐지 코루틴 실행
                detectFoodCoroutine = StartCoroutine(DetectFood());
                break;
            case FishState.ChaseFood:
                break;
        }
    }

    // 새로운 랜덤 목표점 설정
    void SetRandomTarget()
    {
        // 랜덤 포인트 (0 ~ 1) 에 범위만큼
        Vector2 randomOffset = Random.insideUnitCircle * fishData.detectRange;

        // 본인 위치에 계산
        Vector2 randomPoint = new Vector2(transform.position.x, transform.position.y) + randomOffset;

        // 위치가 화면 밖일 가능성 있으므로 Clamp로 제한
        float clampedX = Mathf.Clamp(randomPoint.x, FishTank.Instance.minBounds.x, FishTank.Instance.maxBounds.x);
        float clampedY = Mathf.Clamp(randomPoint.y, FishTank.Instance.minBounds.y, FishTank.Instance.maxBounds.y);

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