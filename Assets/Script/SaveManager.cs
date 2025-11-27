using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField] GameObject waitingUI;     // 저장, 불러오기 대기 UI
    [SerializeField] AudioClip saveClip;       // 저장 효과음

    private ISaveable[] saveables;             // 저장되는 스크립트

    // 저장 경로
    public static string SavePath => Path.Combine(Application.persistentDataPath, "FishTank_SaveData.json");

    private void Awake()
    {
        // 시작하면 부모의 모든 하위 객체에서
        // 저장 가능한 스크립트 가져옴
        saveables = transform.parent.GetComponentsInChildren<ISaveable>();
    }

    private void Start()
    {
        // 시작하면 불러오기 시도
        LoadGame();
    }


    // 파일 저장 (FishTank에서 활성화 물고기 리스트, 배설물 리스트 보내줌)
    public void SaveGame()
    {
        // 효과음 재생
        SoundManager.Instance.PlaySFX(saveClip);

        // 대기 UI On
        waitingUI.SetActive(true);

        // 저장 데이터 하나 생성
        SaveData data = new SaveData();

        // 저장되는 스크립트들 돌아다니면서 데이터 수집
        foreach (ISaveable saveAble in saveables)
        {
            saveAble.Save(data);
        }

        // 재화 가져오기
        if (GameManager.Instance != null)
            data.currentMoney = GameManager.Instance.CurrentMoney;

        // 직렬화 파일로 변환 저장 
        string dataString = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, dataString);

        // 대기 UI Off
        waitingUI.SetActive(false);
        Debug.Log($"저장 완료");
    }

    // 저장 파일 불러오기
    public void LoadGame()
    {
        // 대기 UI On
        waitingUI.SetActive(true);

        // 파일 없으면 무시
        if (File.Exists(SavePath) == false)
        {
            // 대기 UI Off
            waitingUI.SetActive(false);
            return;
        }

        // 저장 파일 읽어서 데이터화
        string dataString = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(dataString);


        // 재화 적용
        if (GameManager.Instance != null)
            GameManager.Instance.CurrentMoney = data.currentMoney;

        // 저장되는 스크립트들 돌아다니면서 데이터 뿌리기
        foreach (ISaveable saveAble in saveables)
        {
            saveAble.Load(data);
        }

        // 대기 UI Off
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
