using System.Xml;
using UnityEngine;
using UnityEngine.InputSystem;

public class FeedContoller : MonoBehaviour
{
    [SerializeField] GameObject foodPrefab;     // 먹이 프리팹

    

    void Update()
    {

    }

    public void OnFeed(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            // 마우스 위치
            Vector2 screenPosition = Mouse.current.position.ReadValue();

            // 월드 위치
            Vector3 wolrdPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            // z는 사용하지 않음
            wolrdPosition.z = 0f;

            // 생성
            Instantiate(foodPrefab, wolrdPosition , Quaternion.identity);
        }
    }
}
