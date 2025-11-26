using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class SaveData   // 저장할 데이터
{
    public int currentMoney;    // 재화

    public List<FishSaveData> fishData = new List<FishSaveData>();  // 물고기 데이터
    public List<Vector3> excrementPosData = new List<Vector3>();    // 배설물 위치 데이터
}


[System.Serializable]
public class FishSaveData  // 물고기 저장 데이터
{
    public int fishID;          // 물고기 번호

    public float posX;                 // X 포지션
    public float posY;                 // Y 포지션

    // 생성자
    public FishSaveData(int id, Vector3 pos)
    {
        fishID = id;
        posX = pos.x;
        posY = pos.y;
    }

    // 다시 벡터로
    public Vector3 GetVector3()
    {
        // 평면이니까 Z 제외
        return new Vector3(posX, posY, 0f);
    }
}