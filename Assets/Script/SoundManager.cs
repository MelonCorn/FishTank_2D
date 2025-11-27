using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] AudioMixer mixer;      // 믹서

    [SerializeField] AudioSource bgmSource; // 배경음 소스
    [SerializeField] AudioSource sfxSource; // 효과음 소스

    [SerializeField] AudioClip[] bgms;      // 배경음악 리스트

    WaitUntil bgmStop;                      // 배경음 재생 코루틴 조건
    private int currentBgmIndex = 0;        // 현재 재생중인 배경음


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
    }


    private void OnEnable()
    {
        // 볼륨 불러오기
        LoadVolumePrefs();
        // 배경음 재생 조건
        bgmStop = new WaitUntil(() => bgmSource.isPlaying == false);
    }


    private void Start()
    {
        // 배경음 목록 있고, 소스 있으면
        if (bgms.Length <= 0 || bgmSource == null) return;
            StartCoroutine(PlayBGM());
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


    // 배경음 재생
    IEnumerator PlayBGM()
    {
        // 무한 반복
        while (true)
        {
            // 멈출 때 까지 기다림
            yield return bgmStop;

            // 갈아 끼우고
            bgmSource.clip = bgms[currentBgmIndex];
            // 재생
            bgmSource.Play();

            // 다음 번호
            currentBgmIndex = (currentBgmIndex + 1) % bgms.Length;
        }
    }
}
