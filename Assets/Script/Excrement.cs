using UnityEngine;

public class Excrement : MonoBehaviour
{
    private FishTank fishTank;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        // 활성화될 때
        // 랜덤 90도
        int rand = Random.Range(1, 5);
        transform.rotation = Quaternion.Euler(0f, 0f, 90f * rand);
    }

    public void Init(FishTank fishTank)
    {
        this.fishTank = fishTank;
    }

    // 활성화 될 때 스프라이트 변경
    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }


    // 비활성화 (청소 용구에서)
    public void Despawn()
    {
        // 풀 반납
        fishTank.ReturnToExcrementPool(this);
    }
}
