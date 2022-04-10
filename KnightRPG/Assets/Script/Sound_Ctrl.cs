using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_Ctrl : MonoBehaviour
{
    public static Sound_Ctrl Inst;             // �ش� ��ũ��Ʈ �̱��� ����
    public AudioSource bgm_Audio = null;    // ��������� ����� �����
    AudioSource[] sfUi_Audio;       // Ui���� ȿ������ ����� �����
    AudioSource[] sfPlayer_Audio;       // �÷��̾� ���� ȿ������ ����� �����
    AudioSource[] sfMon_Audio;       // ���� ���� ȿ������ ����� �����
    // �������� ����Ǵ� ȿ������ ������ �����ϱ� ���� ������Ʈ Ǯ
    Queue<AudioSource> sfUi_Pool = new Queue<AudioSource>();    // Ui���� ȿ������ ������Ʈ Ǯ
    Queue<AudioSource> sfPlayer_Pool = new Queue<AudioSource>();    // �÷��̾� ���� ȿ������ ������Ʈ Ǯ
    Queue<AudioSource> sfMon_Pool = new Queue<AudioSource>();    // ���� ���� ȿ������ ������Ʈ Ǯ
    Dictionary<string, AudioClip> bgm_Dict = new Dictionary<string, AudioClip>();   // ��������� �̸�(Ű��) Ŭ��(���)�� �����ϱ� ���� ��ųʸ�
    Dictionary<string, AudioClip> sf_Dict = new Dictionary<string, AudioClip>();    // ȿ������ �̸�(Ű��) Ŭ��(���)�� �����ϱ� ���� ��ųʸ�
    AudioClip[] m_bgmClip;      // ��������� Ŭ����
    AudioClip[] m_sfClip;       // ȿ������ Ŭ����

    void Awake()
    {
        Inst = this;

        if (PlayerPrefs.HasKey("BgmVolume") == true)    // ���÷� ����� ������� �������� �ִ��� üũ
            GlobalData.bgm_Volume = PlayerPrefs.GetFloat("BgmVolume", 0);   // ���÷� ����� ��������� �������� ����

        if (PlayerPrefs.HasKey("SfVolume") == true)     // ���÷� ����� ȿ������ �������� �ִ��� üũ
            GlobalData.sf_Volume = PlayerPrefs.GetFloat("SfVolume", 0);     // ���÷� ����� ȿ������ �������� ����

        m_bgmClip = Resources.LoadAll<AudioClip>("Sound/BGM");      // Sound/BGM ������ �մ� �����Ŭ������ �迭�� ����
        m_sfClip = Resources.LoadAll<AudioClip>("Sound/SF");        // Sound/SF ������ �ִ� �����Ŭ������ �迭�� ����
        string clip_Str = "";   // Ŭ���� �̸��� ������ ����

        sfUi_Audio = this.transform.Find("Sf_Ui_Root").GetComponentsInChildren<AudioSource>();      
        // Ŭ���� ����� ��������� �迭�� ����
        sfPlayer_Audio = this.transform.Find("Sf_Player_Root").GetComponentsInChildren<AudioSource>();
        // Ŭ���� ����� ��������� �迭�� ����
        sfMon_Audio = this.transform.Find("Sf_Monster_Root").GetComponentsInChildren<AudioSource>();
        // Ŭ���� ����� ��������� �迭�� ����

        foreach (AudioClip clip in m_bgmClip)   // ������� Ŭ���鸸ŭ �ݺ�
        {
            clip_Str = clip.name;   // Ŭ���� �̸��� ������ ����
            bgm_Dict.Add(clip_Str, clip);   // Ŭ���� �̸��� Ű��, Ŭ���� ����� ��ųʸ��� �߰�
        }

        clip_Str = "";  // ���ڿ� �ʱ�ȭ

        foreach (AudioClip clip in m_sfClip)    // ȿ���� Ŭ���� ��ŭ �ݺ�
        {
            clip_Str = clip.name;   // Ŭ���� �̸��� ������ ����
            sf_Dict.Add(clip_Str, clip);   // Ŭ���� �̸��� Ű��, Ŭ���� ����� ��ųʸ��� �߰�
        }

        foreach (AudioSource audio in sfUi_Audio)   // Ui���� ȿ������ ����� ������� ���� ��ŭ �ݺ�
        {
            sfUi_Pool.Enqueue(audio);   // ������Ʈ Ǯ�� �߰�
        }

        foreach (AudioSource audio in sfPlayer_Audio)   // �÷��̾� ���� ȿ������ ����� ������� ���� ��ŭ �ݺ�
        {
            sfPlayer_Pool.Enqueue(audio);   // ������Ʈ Ǯ�� �߰�
        }

        foreach (AudioSource audio in sfMon_Audio)   // ���� ���� ȿ������ ����� ������� ���� ��ŭ �ݺ�
        {
            sfMon_Pool.Enqueue(audio);   // ������Ʈ Ǯ�� �߰�
        }
    }

    void Update()
    {
        bgm_Audio.volume = GlobalData.bgm_Volume;   // ��������� ������ ���� ����
    }

    public void BgmSoundPlay(string sound_Str)      // ������� ����� �Լ�
    {
        AudioClip a_Clip = null;    // Ŭ���� ������ ����
        bgm_Dict.TryGetValue(sound_Str, out a_Clip);    // �Ű������� �Ѿ�� ���� ���� �̸��� ����� Ŭ���� ã��

        bgm_Audio.Stop();   // ������� ��������� ����
        bgm_Audio.clip = a_Clip;    // ������� Ŭ���� ã�� Ŭ������ ����
        bgm_Audio.Play();   // ������� ���
    }

    public void SfSoundPlay(string sound_Str, string a_Type)      // ������� ����� �Լ�
    {
        AudioClip a_Clip = null;    // Ŭ���� ������ ����
        sf_Dict.TryGetValue(sound_Str, out a_Clip);    // �Ű������� �Ѿ�� ���� ���� �̸��� ����� Ŭ���� ã��

        AudioSource a_Audio = null;     // Ŭ���� ����� ������� ������ ����

        switch (a_Type)     // �Ű������� �Ѿ�� Ÿ�� ���� üũ
        {
            case "Ui":  // Ui ����
                {
                    a_Audio = sfUi_Pool.Dequeue();  // ������Ʈ Ǯ���� �ϳ��� ������
                    sfUi_Pool.Enqueue(a_Audio);     // �ٽ� ������Ʈ Ǯ�� �߰�
                }
                break;
            case "Player":  // �÷��̾� ����
                {
                    a_Audio = sfPlayer_Pool.Dequeue();  // ������Ʈ Ǯ���� �ϳ��� ������
                    sfPlayer_Pool.Enqueue(a_Audio);     // �ٽ� ������Ʈ Ǯ�� �߰�
                }
                break;
            case "Monster": // ���� ����
                {
                    a_Audio = sfMon_Pool.Dequeue();  // ������Ʈ Ǯ���� �ϳ��� ������
                    sfMon_Pool.Enqueue(a_Audio);     // �ٽ� ������Ʈ Ǯ�� �߰�
                }
                break;
        }

        if (a_Audio == null)    // ������ ������� ���� ��� �Լ��� ��������
            return;

        a_Audio.volume = 1.0f;  // ���� �� �ʱ�ȭ
        a_Audio.volume = a_Audio.volume * GlobalData.sf_Volume;     // ����� ���� ����ŭ ����
        a_Audio.Stop();     // ������� ȿ���� ����
        a_Audio.clip = a_Clip;      // ã�� Ŭ������ Ŭ�� ����
        a_Audio.Play();     // ȿ���� ���
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("BgmVolume", GlobalData.bgm_Volume);   // ���� ���� �� ������ ���÷� ����
        PlayerPrefs.SetFloat("SfVolume", GlobalData.sf_Volume);   // ���� ���� �� ������ ���÷� ����
    }
}
