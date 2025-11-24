using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Food : MonoBehaviour
{
    private FoodManager foodManager;        // 먹이 매니저
    private SpriteRenderer spriteRenderer;
    private FoodData foodData;              // 먹이 정보
    private bool isEaten = false;           // 먹혔는지 체크

        
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
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // 아래로 가라앉음
        transform.Translate(Vector3.down * foodData.sinkSpeed * Time.deltaTime);
    }




    // 풀 반납
    private void Despawn()
    {
        // 반납 전에 타이머 취소
        CancelInvoke();

        foodManager.ReturnToPool(this);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 먹혔으면 돌아가, 한 번에 여러 물고기 먹이기 방지
        if (isEaten == true) return;

        // Fish 태그
        if (collision.CompareTag("Fish"))
        {
            // FishAI 컴포넌트 있으면
            if (collision.TryGetComponent<FishAI>(out FishAI fish))
            {
                // 먹힘 체크
                isEaten = true;

                // 물고기가 먹음
                fish.EatFood(foodData.exp, foodData.fill);

                // 풀 반납
                Despawn();
            }
        }
        else if (collision.CompareTag("Environment"))
        {
            // 풀 반납
            Despawn();
        }
    }

}
