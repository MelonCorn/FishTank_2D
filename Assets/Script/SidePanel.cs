using System.Collections;
using UnityEngine;

public class SidePanel : MonoBehaviour
{
    [SerializeField] private RectTransform shopPanel;   // 상점 패널
    [SerializeField] private RectTransform foodPanel;   // 먹이 패널

    [SerializeField] private RectTransform openPos;  // 열렸을 때 위치
    [SerializeField] private RectTransform closePos; // 닫혔을 때 위치

    [SerializeField] float slideSpeed = 10f;  // 슬라이드 속도

    private bool isShopOpen;                  // 상점 패널 상태
    private bool isFoodOpen;                  // 먹이 패널 상태

    private Coroutine shopCoroutine;         // 상점슬라이드
    private Coroutine foodCoroutine;         // 먹이슬라이드

    private void Start()
    {
        // 시작할 땐 닫힘 위치로
        shopPanel.anchoredPosition = new Vector2(closePos.anchoredPosition.x, shopPanel.anchoredPosition.y);
        foodPanel.anchoredPosition = new Vector2(closePos.anchoredPosition.x, foodPanel.anchoredPosition.y);
        // 닫힘
        isShopOpen = false;
        isFoodOpen = false;
    }
    

    // 상점 패널 버튼
    public void OnClickShopButton()
    {
        isShopOpen = !isShopOpen;

        shopCoroutine = TogglePanel(shopPanel, isShopOpen, shopCoroutine);
    }

    // 먹이 패널 버튼
    public void OnClickFoodButton()
    {
        isFoodOpen = !isFoodOpen;

        foodCoroutine = TogglePanel(foodPanel, isFoodOpen, foodCoroutine);
    }
    // 패널 상태 전환
    public Coroutine TogglePanel(RectTransform panel, bool isOpen, Coroutine coroutine)
    {
        // 반대로
        isOpen = !isOpen;

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
