using UnityEngine;

[CreateAssetMenu(fileName = "FoodData", menuName = "Scriptable Objects/FoodData")]
public class FoodData : ScriptableObject
{
    [Header("이름")]
    public string foodName;
    [Header("비용")]
    public int cost = 1;
    [Header("성장 경험치")]
    public int exp = 1;
    [Header("감소 허기")]
    public int fill = 25;
    [Header("수명 시간")]
    public float lifeTime = 10f;
    [Header("하강 속도")]
    public float sinkSpeed = 1f;
    [Header("먹이 레이어")]
    public LayerMask layer;
    [Header("스프라이트")]
    public Sprite sprite;
}
