using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] AudioMixer mixer;      // 믹서

    // 현재 볼륨
    private float masterVolume = 0.5f;
    private float bgmVolume = 0.5f;
    private float sfxVolume = 0.5f;

    public float MasterVolume => masterVolume;
    public float BGMVolume => bgmVolume;
    public float SFXVolume => sfxVolume;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadVolumePrefs();
    }

    // 볼륨 불러오기
    void LoadVolumePrefs()
    {
        // 저장된 볼륨 가져오기, 없으면 유지
        masterVolume = PlayerPrefs.GetFloat("Master", masterVolume);
        bgmVolume = PlayerPrefs.GetFloat("BGM", bgmVolume);
        sfxVolume = PlayerPrefs.GetFloat("SFX", sfxVolume);

        // 불러온 값 설정
        SetMasterVolume(masterVolume);
        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);
    }


    // 전체
    public void SetMasterVolume(float value)
    {
        // 볼륨 할당
        masterVolume = value;
        // 믹서 그룹 볼륨 설정
        mixer.SetFloat("Master", Mathf.Log10(value) * 20);
        // 저장
        PlayerPrefs.SetFloat("Master", value);
    }

    // 배경음
    public void SetBGMVolume(float value)
    {
        bgmVolume = value;
        mixer.SetFloat("BGM", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("BGM", value);
    }

    // 효과음
    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        mixer.SetFloat("SFX", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SFX", value);
    }
}
