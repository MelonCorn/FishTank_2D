using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] Button continueButton;
    [SerializeField] Button deleteButton;

    private void Awake()
    {
        // 첨에 저장 파일 존재 여부에 따라 버튼 on / off
        bool hasSave = SaveManager.HasSaveFile(); ;

        continueButton.interactable = hasSave;
        deleteButton.interactable = hasSave;
    }

    // 새 게임 버튼
    public void NewGame()
    {
        // 저장 데이터 삭제
        SaveManager.DeleteSaveFile();
        // 씬 이동
        SceneController.Instance.LoadScene(1);
    }

    // 이어하기 버튼
    public void ContinueGame()
    {
        // 씬 이동
        SceneController.Instance.LoadScene(1);
    }

    // 데이터 삭제 버튼
    public void DeleteSave()
    {
        // 저장 데이터 삭제
        SaveManager.DeleteSaveFile();

        // 데이터 삭제 버튼 상호작용 off
        deleteButton.interactable = false;
        continueButton.interactable = false;
    }

    // 종료 버튼
    public void OnExit()
    {
        Application.Quit();
    }
}
