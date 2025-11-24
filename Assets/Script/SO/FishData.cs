using UnityEngine;

[CreateAssetMenu(fileName = "FishData", menuName = "Scriptable Objects/FishData")]
public class FishData : ScriptableObject
{
    [Header("이름")]
    public string fishName;
    [Header("이동 속도")]
    public float moveSpeed = 2f;
    [Header("이동 대기 시간")]
    public float minWaitTime = 2f;
    public float maxWaitTime = 2f;
    [Header("추적 속도 배율")]
    public float chaseMultiplier = 2f;
    [Header("먹이 감지 범위")]
    public float detectRange = 1.5f;
    [Header("먹이 감지 간격")]
    public float foodDetectInterval = 0.1f;
    [Header("허기 증가치")]
    public int hungerAmount = 1;
    [Header("허기 간격")]
    public float hungerInterval = 1f;
    [Header("최대 허기")]
    public int maxHungry = 100;
    [Header("필요 성장 경험치")]
    public int requireExp = 20;
    [Header("감지 먹이 레이어")]
    public LayerMask detectFoodLayer;
    [Header("스프라이트")]
    public Sprite babySprite;         // 치어 스프라이트
    public Sprite growthSprite;       // 성어 스프라이트
}
