using UnityEngine;

public class CleanTool : MonoBehaviour
{
    private Collider2D toolCollider;
    private SpriteRenderer spriteRenderer;

    private bool isCleaning;

    [SerializeField] int gainMoney;         // 획득 재화

    // 색상
    [SerializeField] Color activeColor;
    [SerializeField] Color deactiveColor;

    private void Awake()
    {
        toolCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 일단 다 비활성화
        gameObject.SetActive(false);
        toolCollider.enabled = false;
    }

    private void OnEnable()
    {
        InputManager.Instance.OnInteractionClean += SetClean;

        isCleaning = false;
    }
    private void OnDisable()
    {
        if(InputManager.Instance != null)
            InputManager.Instance.OnInteractionClean -= SetClean;

        // 상태 초기화
        ResetTool();
    }


    // 상태 초기화 (비활성화 될 때)
    private void ResetTool()
    {
        isCleaning = false;
        toolCollider.enabled = false;
        spriteRenderer.color = deactiveColor;
    }

    // 청소 상태
    public void SetClean()
    {
        isCleaning = !isCleaning;

        // 상태에 따라 콜라이더 활성화
        toolCollider.enabled = isCleaning;

        // 상태에 따라 색상
        spriteRenderer.color = isCleaning ? activeColor : deactiveColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 배설물
        if (other.CompareTag("Excrement"))
        {
            // 컴포넌트 체크
            if(other.TryGetComponent<Excrement>(out Excrement excrement))
            {
                // 재화 추가
                GameManager.Instance.AddMoney(gainMoney);
                // 비활성화
                excrement.Despawn();
            }
        }
    }
}
