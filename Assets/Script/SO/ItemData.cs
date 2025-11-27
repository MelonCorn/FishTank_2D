using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{

    [Header("이름")]
    public string itemName;
    [Header("가격")]
    public int cost = 10;
    [Header("스프라이트")]
    public Sprite sprite;

    [Header("정보"), TextArea]
    public string info;
}
