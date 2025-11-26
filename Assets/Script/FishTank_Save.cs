using System.Collections.Generic;
using UnityEngine;

public partial class FishTank
{
    // 활성화 된 오브젝트 리스트 반환
    public List<T>GetActiveItems<T>() where T : Component
    {
        // 활성화 T 리스트 생성
        List<T> activeList = new List<T>();

        // 하위의 모든 T
        T[] allItems = GetComponentsInChildren<T>(true);

        // T 전부 체크
        foreach (T item in allItems)
        {
            // 활성화 상태면
            if (item.gameObject.activeSelf)
            {
                // 활성화 리스트에 추가
                activeList.Add(item);
            }
        }

        // 리스트 반환
        return activeList;
    }


    // 저장된 물고기 데이터 기반으로 생성
    public void SpawnSaveFishs(FishSaveData saveData)
    {
        // 번호에 맞는 데이터 장전
        FishData targetData = fishData[saveData.fishID];

        // 풀에서 꺼냄
        FishAI newFish = fishPool.Get();

        // 위치 잡아줌
        newFish.transform.position = saveData.GetVector3();
        
        // 데이터 넣어줌
        newFish.InitFishType(targetData, saveData.fishID);
        newFish.LoadData(saveData);

        // 물고기 수 갱신
        SetFishCountText();
    }

    // 저장된 배설물 데이터 기반으로 생성
    public void SpawnSaveExcrements(Vector3 position)
    {
        // 풀에서 꺼냄
        Excrement excrement = excrementPool.Get();

        // 위치 잡아줌
        excrement.transform.position = position;
    }
}
