using Unity.VisualScripting;
using UnityEngine;

public class Food : MonoBehaviour
{
    // 먹혔는지 체크
    bool isEaten = false;

    private void OnEnable()
    {
        isEaten = false;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 먹혔으면 돌아가
        if (isEaten == true) return;

        // Fish 태그
        if (collision.CompareTag("Fish"))
        {
            // FishAI 컴포넌트 있으면
            if (collision.TryGetComponent<FishAI>(out FishAI fish))
            {
                // 먹힘 체크
                isEaten = true;

                // 먹이 받아 먹음
                fish.EatFood();

                // 나중에 풀링
                Destroy(gameObject);
            }
        }
    }

}
