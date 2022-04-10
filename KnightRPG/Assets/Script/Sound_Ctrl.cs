using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_Ctrl : MonoBehaviour
{
    public static Sound_Ctrl Inst;             // 해당 스크립트 싱글톤 선언
    public AudioSource bgm_Audio = null;    // 배경음악을 재생할 오디오
    AudioSource[] sfUi_Audio;       // Ui관련 효과음을 재생할 오디오
    AudioSource[] sfPlayer_Audio;       // 플레이어 관련 효과음을 재생할 오디오
    AudioSource[] sfMon_Audio;       // 몬스터 관련 효과음을 재생할 오디오
    // 여러개가 재생되는 효과음의 개수를 조절하기 위한 오브젝트 풀
    Queue<AudioSource> sfUi_Pool = new Queue<AudioSource>();    // Ui관련 효과음의 오브젝트 풀
    Queue<AudioSource> sfPlayer_Pool = new Queue<AudioSource>();    // 플레이어 관련 효과음의 오브젝트 풀
    Queue<AudioSource> sfMon_Pool = new Queue<AudioSource>();    // 몬스터 관련 효과음의 오브젝트 풀
    Dictionary<string, AudioClip> bgm_Dict = new Dictionary<string, AudioClip>();   // 배경음악을 이름(키값) 클립(밸류)로 저장하기 위한 딕셔너리
    Dictionary<string, AudioClip> sf_Dict = new Dictionary<string, AudioClip>();    // 효과음을 이름(키값) 클립(밸류)로 저장하기 위한 딕셔너리
    AudioClip[] m_bgmClip;      // 배경음악의 클립들
    AudioClip[] m_sfClip;       // 효과음의 클립들

    void Awake()
    {
        Inst = this;

        if (PlayerPrefs.HasKey("BgmVolume") == true)    // 로컬로 저장된 배경음악 볼륨값이 있는지 체크
            GlobalData.bgm_Volume = PlayerPrefs.GetFloat("BgmVolume", 0);   // 로컬로 저장된 배경음악의 볼륨으로 설정

        if (PlayerPrefs.HasKey("SfVolume") == true)     // 로컬로 저장된 효과음의 볼륨값이 있는지 체크
            GlobalData.sf_Volume = PlayerPrefs.GetFloat("SfVolume", 0);     // 로컬로 저장된 효과음의 볼륨으로 설정

        m_bgmClip = Resources.LoadAll<AudioClip>("Sound/BGM");      // Sound/BGM 폴더에 잇는 오디오클립들을 배열에 저장
        m_sfClip = Resources.LoadAll<AudioClip>("Sound/SF");        // Sound/SF 폴더에 있는 오디오클립들을 배열에 저장
        string clip_Str = "";   // 클립의 이름을 저장할 변수

        sfUi_Audio = this.transform.Find("Sf_Ui_Root").GetComponentsInChildren<AudioSource>();      
        // 클립을 재생할 오디오들을 배열에 저장
        sfPlayer_Audio = this.transform.Find("Sf_Player_Root").GetComponentsInChildren<AudioSource>();
        // 클립을 재생할 오디오들을 배열에 저장
        sfMon_Audio = this.transform.Find("Sf_Monster_Root").GetComponentsInChildren<AudioSource>();
        // 클립을 재생할 오디오들을 배열에 저장

        foreach (AudioClip clip in m_bgmClip)   // 배경음악 클립들만큼 반복
        {
            clip_Str = clip.name;   // 클립의 이름을 변수로 저장
            bgm_Dict.Add(clip_Str, clip);   // 클립의 이름을 키값, 클립을 밸류로 딕셔너리에 추가
        }

        clip_Str = "";  // 문자열 초기화

        foreach (AudioClip clip in m_sfClip)    // 효과음 클립들 만큼 반복
        {
            clip_Str = clip.name;   // 클립의 이름을 변수로 저장
            sf_Dict.Add(clip_Str, clip);   // 클립의 이름을 키값, 클립을 밸류로 딕셔너리에 추가
        }

        foreach (AudioSource audio in sfUi_Audio)   // Ui관련 효과음을 재생할 오디오의 개수 만큼 반복
        {
            sfUi_Pool.Enqueue(audio);   // 오브젝트 풀에 추가
        }

        foreach (AudioSource audio in sfPlayer_Audio)   // 플레이어 관련 효과음을 재생할 오디오의 개수 만큼 반복
        {
            sfPlayer_Pool.Enqueue(audio);   // 오브젝트 풀에 추가
        }

        foreach (AudioSource audio in sfMon_Audio)   // 몬스터 관련 효과음을 재생할 오디오의 개수 만큼 반복
        {
            sfMon_Pool.Enqueue(audio);   // 오브젝트 풀에 추가
        }
    }

    void Update()
    {
        bgm_Audio.volume = GlobalData.bgm_Volume;   // 배경음악의 볼륨을 값을 조절
    }

    public void BgmSoundPlay(string sound_Str)      // 배경음악 재생용 함수
    {
        AudioClip a_Clip = null;    // 클립을 저장할 변수
        bgm_Dict.TryGetValue(sound_Str, out a_Clip);    // 매개변수로 넘어온 값과 같은 이름의 오디오 클립을 찾음

        bgm_Audio.Stop();   // 재생중인 배경음악을 멈춤
        bgm_Audio.clip = a_Clip;    // 오디오의 클립을 찾은 클립으로 변경
        bgm_Audio.Play();   // 배경음악 재생
    }

    public void SfSoundPlay(string sound_Str, string a_Type)      // 배경음악 재생용 함수
    {
        AudioClip a_Clip = null;    // 클립을 저장할 변수
        sf_Dict.TryGetValue(sound_Str, out a_Clip);    // 매개변수로 넘어온 값과 같은 이름의 오디오 클립을 찾음

        AudioSource a_Audio = null;     // 클립을 재생할 오디오를 저장할 변수

        switch (a_Type)     // 매개변수로 넘어온 타입 값을 체크
        {
            case "Ui":  // Ui 관련
                {
                    a_Audio = sfUi_Pool.Dequeue();  // 오브젝트 풀에서 하나를 가져옴
                    sfUi_Pool.Enqueue(a_Audio);     // 다시 오브젝트 풀에 추가
                }
                break;
            case "Player":  // 플레이어 관련
                {
                    a_Audio = sfPlayer_Pool.Dequeue();  // 오브젝트 풀에서 하나를 가져옴
                    sfPlayer_Pool.Enqueue(a_Audio);     // 다시 오브젝트 풀에 추가
                }
                break;
            case "Monster": // 몬스터 관련
                {
                    a_Audio = sfMon_Pool.Dequeue();  // 오브젝트 풀에서 하나를 가져옴
                    sfMon_Pool.Enqueue(a_Audio);     // 다시 오브젝트 풀에 추가
                }
                break;
        }

        if (a_Audio == null)    // 저장한 오디오가 없을 경우 함수를 빠져나감
            return;

        a_Audio.volume = 1.0f;  // 볼륨 값 초기화
        a_Audio.volume = a_Audio.volume * GlobalData.sf_Volume;     // 저장된 볼륨 값만큼 조정
        a_Audio.Stop();     // 재생중인 효과음 정지
        a_Audio.clip = a_Clip;      // 찾은 클립으로 클립 변경
        a_Audio.Play();     // 효과음 재생
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("BgmVolume", GlobalData.bgm_Volume);   // 게임 종료 시 볼륨값 로컬로 저장
        PlayerPrefs.SetFloat("SfVolume", GlobalData.sf_Volume);   // 게임 종료 시 볼륨값 로컬로 저장
    }
}
