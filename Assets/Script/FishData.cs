using UnityEngine;

[CreateAssetMenu(fileName = "FishData", menuName = "Scriptable Objects/FishData")]
public class FishData : ScriptableObject
{
    [Header("이동 속도")]
    public float moveSpeed = 2f;           // 이동 속도
    [Header("이동 대기 시간")]
    public float minWaitTime = 2f;         // 최소 대기 시간
    public float maxWaitTime = 2f;         // 최대 대기 시간
    [Header("추적 속도 배율")]
    public float chaseMultiplier = 2f;     // 먹이 추적 배율
    [Header("먹이 감지 범위")]
    public float detectRange = 1.5f;       // 먹이 감지 범위
    [Header("먹이 감지 간격")]
    public float foodDetectInterval = 0.1f;// 먹이 감지 간격
    [Header("최대 허기")]
    public float maxHungry = 100f;         // 최대 허기
    [Header("먹이 레이어")]
    public LayerMask foodLayer;            // 먹이 레이어
}
