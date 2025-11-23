using UnityEngine;

[CreateAssetMenu(fileName = "FoodData", menuName = "Scriptable Objects/FoodData")]
public class FoodData : ScriptableObject
{
    [Header("성장 경험치")]
    public int exp = 20;
    [Header("먹이 레이어")]
    public LayerMask layer;
    [Header("스프라이트")]
    public Sprite sprite;
}
