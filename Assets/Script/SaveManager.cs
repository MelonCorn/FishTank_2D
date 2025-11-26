using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField] FishTank fishTank;     // 수조
    [SerializeField] GameObject waitingUI;  // 저장 로드 대기 UI

    // 저장 경로
    public static string SavePath => Path.Combine(Application.persistentDataPath, "FishTank_SaveData.json");

    private void Start()
    {
        // 시작하면 불러오기 시도
        LoadGame();
    }


    // 파일 저장 (FishTank에서 활성화 물고기 리스트, 배설물 리스트 보내줌)
    public void SaveGame()
    {
        // 대기 UI
        waitingUI.SetActive(true);

        // 활성화 물고기 리스트
        List<FishAI> fishes = fishTank.GetActiveItems<FishAI>();
        // 활성화 배설물 리스트
        List<Excrement> excrements = fishTank.GetActiveItems<Excrement>();

        // 저장 데이터 하나 생성
        SaveData data = new SaveData();
        
        // 재화 가져오기
        if (GameManager.Instance != null)
            data.currentMoney = GameManager.Instance.CurrentMoney;

        // 가져온 물고기 데이터마다
        foreach (FishAI fish in fishes)
        {
            // 저장 데이터 추출해서 리스트에 추가
            data.fishData.Add(fish.GetFishData());
        }
        
        // 가져온 배설물 데이터 추출해서 리스트에 추가
        foreach (Excrement excrement in excrements)
        {
            // 배설물 위치
            Vector3 pos = excrement.transform.position;

            // 배설물 저장 데이터 만들고 리스트에 추가
            data.excrementPosData.Add(pos);
        }

        // 직렬화 파일로 변환 저장 
        string dataString = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, dataString);
        waitingUI.SetActive(false);
        Debug.Log($"저장 완료");
    }

    // 저장 파일 불러오기
    public void LoadGame()
    {
        // 대기 UI
        waitingUI.SetActive(true);
        // 파일 없으면 무시
        if (File.Exists(SavePath) == false)
        {
            waitingUI.SetActive(false);
            return;
        }

        // 저장 파일 읽어서 데이터화
        string dataString = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(dataString);


        // 재화 적용
        if (GameManager.Instance != null)
            GameManager.Instance.CurrentMoney = data.currentMoney;

        // 저장된 물고기 데이터마다
        foreach (var fishData in data.fishData)
        {
            // 저장 데이터 기반으로 소환
            fishTank.SpawnSaveFishs(fishData);
        }

        // 저장된 배설물 데이터마다
        foreach (var pos in data.excrementPosData)
        {
            // 위치에 맞게 소환
            fishTank.SpawnSaveExcrements(pos);
        }

        // 대기 UI
        waitingUI.SetActive(false);
        Debug.Log("불러오기 완료");
    }


    // 저장 파일 있는지 확인하고 반환
    public static bool HasSaveFile()
    {
        return File.Exists(SavePath);
    }


    // 저장 파일 제거
    public static void DeleteSaveFile()
    {
        // 파일이 있는지 확인
        if (File.Exists(SavePath))
        {
            // 파일 삭제 수행
            File.Delete(SavePath);
        }
    }
}
