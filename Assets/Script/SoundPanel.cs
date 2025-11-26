using UnityEngine;
using UnityEngine.UI;

public class SoundPanel : MonoBehaviour
{
    [Header("볼륨 조절 슬라이더")]
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;

    [Header("확인 버튼")]
    [SerializeField] Button applyButton;

    [Header("On, Off 패널")]
    [SerializeField] GameObject panel;

    public bool isActive => panel.activeSelf;   // 활성화 상태

    private void Start()
    {
        // 매니저에서 볼륨 불러오기
        if (masterSlider != null)
            masterSlider.value = SoundManager.Instance.MasterVolume;
        if (bgmSlider != null)
            bgmSlider.value = SoundManager.Instance.BGMVolume;
        if (sfxSlider != null)
            sfxSlider.value = SoundManager.Instance.SFXVolume;

        // 버튼 이벤트 연결
        applyButton?.onClick.AddListener(()=>
        {
            if (InputManager.Instance != null)
                InputManager.Instance.SetPause();
            else
                SetActivePanel();
        });

        // 슬라이더 이벤트 연결
        masterSlider?.onValueChanged.AddListener(SoundManager.Instance.SetMasterVolume);
        bgmSlider?.onValueChanged.AddListener(SoundManager.Instance.SetBGMVolume);
        sfxSlider?.onValueChanged.AddListener(SoundManager.Instance.SetSFXVolume);
    }

    // 활성화 상태 변경
    public void SetActivePanel()
    {
        panel.SetActive(!panel.activeSelf);
    }
}
