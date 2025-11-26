using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Food : MonoBehaviour
{
    private FoodManager foodManager;        // 먹이 매니저
    private SpriteRenderer spriteRenderer;
    private FoodData foodData;              // 먹이 정보
    private bool isEaten = false;           // 먹혔는지 체크

    private float currentSpeed;             // 현재 속도

    [SerializeField] float defaultSpeed;    // 물 밖 기본 하강 속도
        
    // 생성 시 초기화
    public void Init(FoodManager foodManager)
    {
        this.foodManager = foodManager;
    }

    // 활성화 시 데이터 초기화
    public void InitFoodType(FoodData newData)
    {
        foodData = newData;

        // 이미지 적용
        spriteRenderer.sprite = foodData.sprite;

        // 수명 시간 뒤에 풀 반환
        Invoke(nameof(Despawn), foodData.lifeTime);

        isEaten = false;
    }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        // 활성화될 때
        // 랜덤 90도
        int rand = Random.Range(1, 5);
        spriteRenderer.transform.rotation = Quaternion.Euler(0f, 0f, 90f * rand);

        // 활성화 시 속도 기본
        currentSpeed = defaultSpeed;
    }

    private void Update()
    {
        // 아래로 가라앉음
        transform.Translate(Vector3.down * currentSpeed * Time.deltaTime);
    }




    // 풀 반납
    private void Despawn()
    {
        // 반납 전에 타이머 취소
        CancelInvoke();

        foodManager.ReturnToPool(this);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // 먹힌 상태면 무시.
        // 한 번에 여러 물고기 섭취 방지
        if (isEaten == true) return;

        // Fish 태그
        if (other.CompareTag("Fish"))
        {
            // FishAI 컴포넌트 있으면
            if (other.TryGetComponent<FishAI>(out FishAI fish))
            {
                // 배고픔 상태 아니면 무시
                if (fish.CheckHungry() == false) return;

                // 먹힘 체크
                isEaten = true;

                // 물고기가 먹음
                fish.EatFood(foodData.exp, foodData.fill);

                // 풀 반납
                Despawn();
            }
        }
        else if (other.CompareTag("Water"))
        {
            currentSpeed = foodData.sinkSpeed;
        }
        else if (other.CompareTag("Environment"))
        {
            // 풀 반납
            Despawn();
        }
    }

}
