using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("패널")]
    [SerializeField] private RectTransform shopPanel;   // 상점 패널
    [SerializeField] private RectTransform toolPanel;   // 도구 패널

    [Header("여닫이 위치")]
    [SerializeField] private RectTransform openPos;  // 열렸을 때 위치
    [SerializeField] private RectTransform closePos; // 닫혔을 때 위치

    [Header("슬라이드 속도")]
    [SerializeField] float slideSpeed = 10f;  // 슬라이드 속도

    [Header("버튼 프리팹")]
    [SerializeField] GridButton buttonPrefab;         // 버튼 프리팹
    [Header("버튼 정보 패널")]
    [SerializeField] HoverPanel hoverPanel;           // 정보 패널

    private bool isShopOpen = false;                  // 상점 패널 상태
    private bool isFoodOpen = false;                  // 먹이 패널 상태

    private Coroutine shopCoroutine;         // 상점슬라이드
    private Coroutine foodCoroutine;         // 먹이슬라이드

    ICreateButton[] createButtonUI;          // 버튼 생성 스크립트

    public GridButton ButtonPrefab => buttonPrefab;

    private void Awake()
    {
        // ICreateButton 다 가져옴
        createButtonUI = GetComponents<ICreateButton>();
    }

    private void OnEnable()
    {
        // 오픈 키 구독
        InputManager.Instance.OnShopKey += OnClickShop;
        InputManager.Instance.OnFoodKey += OnClickFood; 
    }
    private void OnDisable()
    {
        // 인풋 매니저 없으면 무시
        if (InputManager.Instance == null) return;

        // 오픈 키 해지
        InputManager.Instance.OnShopKey -= OnClickShop;
        InputManager.Instance.OnFoodKey -= OnClickFood;
    }




    private void Start()
    {
        // 인터페이스 마다 버튼 생성 호출
        foreach (var ui in createButtonUI)
            ui.CreateButtons();

        // x만
        // 시작할 땐 닫힘 위치로
        shopPanel.anchoredPosition = new Vector2(closePos.anchoredPosition.x, shopPanel.anchoredPosition.y);
        toolPanel.anchoredPosition = new Vector2(closePos.anchoredPosition.x, toolPanel.anchoredPosition.y);
        // 닫힘
        isShopOpen = false;
        isFoodOpen = false;
    }
    

    // 상점 패널 버튼
    public void OnClickShop()
    {
        isShopOpen = !isShopOpen;

        shopCoroutine = TogglePanel(shopPanel, isShopOpen, shopCoroutine);
    }

    // 먹이 패널 버튼
    public void OnClickFood()
    {
        isFoodOpen = !isFoodOpen;

        foodCoroutine = TogglePanel(toolPanel, isFoodOpen, foodCoroutine);
    }


    public void HoverButton(ItemData data, bool isActive)
    {
        // 데이터 주입!
        hoverPanel.SetInfo(data);
        // 활성화 상태
        hoverPanel.gameObject.SetActive(isActive);
    }



    // 패널 상태 전환
    public Coroutine TogglePanel(RectTransform panel, bool isOpen, Coroutine coroutine)
    {
        // 실행 중이면 스답
        if (coroutine != null)
            StopCoroutine(coroutine);

        // 열리면 open, 닫히면 close 마커로 
        RectTransform target = isOpen ? openPos : closePos;

        // 슬라이드 시작
        return StartCoroutine(SlideToTarget(panel, target));
    }

    // 슬라이드 
    IEnumerator SlideToTarget(RectTransform panel, RectTransform target)
    {
        Vector2 targetPos = new Vector2(target.anchoredPosition.x, panel.anchoredPosition.y);

        // 0.1 거리 될 때 까지
        while (Vector2.Distance(panel.position, target.position) > 0.1f)
        {
            panel.anchoredPosition = Vector3.Lerp(panel.anchoredPosition, targetPos, Time.deltaTime * slideSpeed);
            yield return null;
        }

        // 딱 맞춰주기
        panel.anchoredPosition = target.anchoredPosition;
    }
}
