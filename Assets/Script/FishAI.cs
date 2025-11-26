using System.Collections;
using UnityEngine;

// 물고기의 상태 정의
public enum FishState
{
    Idle,
    ChaseFood,
    Dead,
}

public class FishAI : MonoBehaviour
{
    private FishTank fishTank;                       // 어항 
    private FishData fishData;                       // 물고기 정보

    private SpriteRenderer spriteRenderer;           // 물고기
    [SerializeField] SpriteRenderer stateIcon;       // 상태 아이콘

    private FishState currentState = FishState.Idle; // 현재 상태

    private Transform foodTarget;                    // 감지된 먹이 정보
    private Vector2 movePoint;                       // 이동 포인트

    private float waitTimer;                         // 현재 대기 시간
    private int currentHungry;                       // 현재 허기치
    private int currentExp;                          // 현재 성장치

    private bool isGrowth;                           // 성장 여부

    private Coroutine detectFoodCoroutine;           // 먹이 감지 코루틴
    private WaitForSeconds detectFoodDelay;          // 감지 간격

    private WaitForSeconds hungerDelay;              // 허기 간격
    private WaitUntil untilHungry;                   // 배고픔 상태 체크

    private WaitForSeconds excreteDelay;             // 배설 간격

    private int id;         // 저장용 번호
    public int ID => id;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    // 생성 시 초기화
    public void Init(FishTank fishTank)
    {
        this.fishTank = fishTank;
        untilHungry = new WaitUntil(() => Hungry());
    }

    // 활성화 시 데이터 초기화
    public void InitFishType(FishData newData, int id)
    {
        fishData = newData;

        // 먹이 감지 간격 설정
        detectFoodDelay = new WaitForSeconds(fishData.foodDetectInterval);
        // 허기 간격 설정
        hungerDelay = new WaitForSeconds(fishData.hungerInterval);
        // 배설 간격 설정
        excreteDelay = new WaitForSeconds(fishData.excreteInterval);

        // 치어 이미지 적용
        spriteRenderer.sprite = fishData.babySprite;

        // Idle 상태
        ChangeState(FishState.Idle);
        
        // 살아있는 동안 계속 돌아가므로 안 담아도 됨
        StartCoroutine(Hunger());          // 허기 코루틴 시작
        StartCoroutine(Defecation());      // 배변 코루틴 시작
    }

    private void OnEnable()
    {
        currentHungry = 0;
        currentExp = 0;

        // 랜덤 색상
        spriteRenderer.color = new Color(Random.value, Random.value, Random.value, 1f);
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


    // 상태 변경
    private void ChangeState(FishState newState)
    {
        // 이전 상태 정리
        switch (currentState)
        {
            case FishState.Idle:
                // 감지 코루틴이 돌고 있으면 정지
                if (detectFoodCoroutine != null)
                    detectFoodCoroutine = null;
                break;

            case FishState.ChaseFood:
                // 먹이 타겟 제거, 나중에 강제 변환 시 
                foodTarget = null;
                break;

            case FishState.Dead:
                break;
        }

        currentState = newState;

        // 새 상태 설정
        switch (currentState)
        {
            case FishState.Idle:
                // 다른 지점 이동
                SetRandomTarget();
                // 바로 움직이게
                waitTimer = 0f;
                // 먹이 탐지 코루틴 실행
                detectFoodCoroutine = StartCoroutine(DetectFood());
                break;

            case FishState.ChaseFood:
                break;

            case FishState.Dead:
                // 사망 아이콘
                stateIcon.sprite = fishData.deadSprite;
                // 아이콘 활성화
                stateIcon.gameObject.SetActive(true);
                // 하얗게..
                spriteRenderer.color = Color.white;
                // 하강 후 반납
                StartCoroutine(Sink());
                break;
        }
    }




    // -------------------------------------------

    #region Idle


    // 이동
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
            if (waitTimer <= 0)
            {
                // 랜덤 이동 포인트 지정
                SetRandomTarget();
                // 랜덤 이동 대기 시간 설정
                waitTimer = GetRandomWaitTime();
            }
            // 대기 시간 감소
            else
            {
                waitTimer -= Time.deltaTime;
            }
        }
    }

    // 이동 대기 시간 랜덤
    float GetRandomWaitTime()
    {
        return Random.Range(fishData.minWaitTime, fishData.maxWaitTime);
    }

    // 새로운 랜덤 목표점 설정
    void SetRandomTarget()
    {
        // 랜덤 포인트 (0 ~ 1) 에 범위만큼
        Vector2 randomOffset = Random.insideUnitCircle * fishData.detectRange;

        // 본인 위치에 계산
        Vector2 randomPoint = new Vector2(transform.position.x, transform.position.y) + randomOffset;

        // 최종 목표 설정
        movePoint = ClampPosition(randomPoint);
    }

    // 먹이 감지 코루틴
    IEnumerator DetectFood()
    {
        // 현재 상태 Idle 인 동안 반복
        while (currentState == FishState.Idle)
        {
            // 배고픔 상태까지 대기
            yield return untilHungry;

            // 주변 반경 안에 Food 레이어 물체 감지 시도
            Collider2D food = Physics2D.OverlapCircle(transform.position, fishData.detectRange, fishData.detectFoodLayer);

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


    #endregion

    // -------------------------------------------

    #region FoodChase


    // 먹이 추적
    void ChaseFood()
    {
        // 쫓던 먹이가 사라짐, 거리 벗어남
        // Idle 상태로 전환
        if (foodTarget == null ||
            foodTarget.gameObject.activeSelf == false ||
            Vector2.Distance(transform.position, foodTarget.position) > fishData.detectRange)
        {
            ChangeState(FishState.Idle);
            return;
        }

        // 먹이 향해 이동
        Vector2 targetPos = foodTarget.position;

        // 다음 위치
        // 추적 배율 적용
        Vector2 nexPos = Vector2.MoveTowards(transform.position, targetPos, fishData.moveSpeed * fishData.chaseMultiplier * Time.deltaTime);

        // 결과 제한
        transform.position = ClampPosition(nexPos);

        // 먹이 쪽 바라보기
        FlipSprite(targetPos);
    }


    #endregion

    // -------------------------------------------

    #region Dead


    // 사망 후 일정시간 가라 앉음
    IEnumerator Sink()
    {
        float timer = 0f;

        while (timer < 3f)
        {
            timer += Time.deltaTime;

            // 수조 안에서만 아래로 이동
            if(transform.position.y >= fishTank.MinBounds.y)
                transform.position += Vector3.down * 0.3f * Time.deltaTime;

            yield return null; // 다음 프레임까지 대기
        }

        // 풀 반납
        fishTank.ReturnToFishPool(this);
    }


    #endregion

    // -------------------------------------------

    #region Food

    // 먹이 받아 먹음 (Food 에서 호출)
    public void EatFood(int exp, int fill)
    {
        // 성장 상태 아니면
        // 성장 경험치 증가
        if(isGrowth == false) IncreaseEXP(exp);

        // 허기 감소
        bool isAlive = DecreaseHungry(fill);

        // 먹고 살았는지 체크
        // 죽었으면 무시
        if (isAlive == false) return;

        // 먹이 없어져서 다시 한가해짐
        ChangeState(FishState.Idle);
    }

    // 성장치 증가
    void IncreaseEXP(int amount)
    {
        currentExp += amount;

        // 최대 성장치 넘으면 
        if(currentExp >= fishData.requireExp)
        {
            //성장
            isGrowth = true;

            // 스프라이트 성장 스프라이트로 변경
            spriteRenderer.sprite = fishData.sprite;
        }
    }

    // 허기 감소
    // 생존 여부 반환
    bool DecreaseHungry(int amount)
    {
        currentHungry -= amount;

        // 배고픔 상태 체크
        Hungry();

        // 0 이하면 0 고정
        if (currentHungry <= 0)
            currentHungry = 0;

        // 잘못 먹어서 허기 최대 넘어가면 즉시 사망
        else if (currentHungry >= fishData.maxHungry)
        {
            ChangeState(FishState.Dead);
            return false;
        }

        return true;
    }

    #endregion

    // -------------------------------------------

    #region Hunger


    // 허기
    private IEnumerator Hunger()
    {
        // 사망 상태가 아니면 루프
        while (currentState != FishState.Dead)
        {
            // 허기 간격만큼 대기
            yield return hungerDelay;

            // 현재 허기 증가
            currentHungry += fishData.hungerAmount;

            // 배고픔 상태 체크
            Hungry();

            // 현재 허기가 최대 허기 이상이면
            if (currentHungry >= fishData.maxHungry)
            {
                currentHungry = fishData.maxHungry;
                // 사망 상태로 전환
                ChangeState(FishState.Dead);
                //코루틴 종료
                yield break;
            }
        }
    }

    // 배고픔 상태 체크
    public bool Hungry()
    {
        // 허기 일정 이상이면 배고픔 상태
        bool isHungry = currentHungry >= fishData.maxHungry * 0.7f;

       // 배고픔 상태면 아이콘 변경
       if (isHungry == true)
           stateIcon.sprite = fishData.hungrySprite;
       
       // 배고픔 상태에 따라 아이콘 활성화
       stateIcon.gameObject.SetActive(isHungry);

        return isHungry;
    }


    #endregion

    // -------------------------------------------

    #region Excrete

    // 배설
    IEnumerator Defecation()
    {
        // 사망 상태가 아니면 루프
        while (currentState != FishState.Dead)
        {
            // 배변 간격만큼 대기
            yield return excreteDelay;

            // 대기 중 사망하면 루프 깨기
            if (currentState == FishState.Dead) break;

            // 현재 위치에 배설
            fishTank.Excretion(transform.position);
        }
    }


    #endregion

    // -------------------------------------------

    // 스프라이트 좌우 반전
    private void FlipSprite(Vector2 target)
    {
        // x 차이 계산
        float xDifference = target.x - transform.position.x;

        // 일정 거리 이상이어야 뒤집음
        if (Mathf.Abs(xDifference) > 0.1f)
        {
            if (xDifference > 0)
                spriteRenderer.flipX = true; // 오른쪽

            else
                spriteRenderer.flipX = false; // 왼쪽
        }

        
    }

    // 위치 제한 
    private Vector2 ClampPosition(Vector3 position)
    {
        float clampedX = Mathf.Clamp(position.x, fishTank.MinBounds.x, fishTank.MaxBounds.x);
        float clampedY = Mathf.Clamp(position.y, fishTank.MinBounds.y, fishTank.MaxBounds.y);

        return new Vector2(clampedX, clampedY);
    }
}