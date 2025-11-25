using UnityEngine;

public class Excrement : MonoBehaviour
{
    private FishTank fishTank;
    private SpriteRenderer spriteRenderer;

    [SerializeField] Sprite[] sprites;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        // 활성화될 때
        // 랜덤 90도
        int rand = Random.Range(1, 5);
        transform.rotation = Quaternion.Euler(0f, 0f, 90f * rand);
        // 랜덤 스프라이트
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
    }

    public void Init(FishTank fishTank)
    {
        this.fishTank = fishTank;
    }

    // 나중에 청소도구 사용으로
    // 비활성화
    void Despawn()
    {
        // 풀 반납
        fishTank.ReturnToExcrementPool(this);
    }
}
