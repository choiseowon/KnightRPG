using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Option_Ctrl : MonoBehaviour
{
    public Button option_Btn = null;
    public Button option_CBtn = null;
    public GameObject option_Root = null;

    public Slider bgm_Slider = null;
    public Slider sf_Slider = null;

    public Button logOut_Btn = null;
    public Button exit_Btn = null;

    void Start()
    {
        bgm_Slider.value = GlobalData.bgm_Volume;
        sf_Slider.value = GlobalData.sf_Volume;

        if (option_Btn != null)
            option_Btn.onClick.AddListener(() =>
            {
                option_Root.SetActive(true);
            });

        if (option_CBtn != null)
            option_CBtn.onClick.AddListener(() =>
            {
                option_Root.SetActive(false);
            });

        if (logOut_Btn != null)
            logOut_Btn.onClick.AddListener(() =>
            {
                LogOutFunc();
            });

        if (exit_Btn != null)
            exit_Btn.onClick.AddListener(() =>
            {
                GameExitFunc();
            });
    }

    void Update()
    {
        GlobalData.bgm_Volume = bgm_Slider.value;
        GlobalData.sf_Volume = sf_Slider.value;
    }

    void LogOutFunc()
    {
        StartCoroutine(GlobalData.SaveDataCo(GlobalData.user_Number));
        PlayerPrefs.SetFloat("BgmVolume", GlobalData.bgm_Volume);   // 게임 종료 시 볼륨값 로컬로 저장
        PlayerPrefs.SetFloat("SfVolume", GlobalData.sf_Volume);   // 게임 종료 시 볼륨값 로컬로 저장
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
    }

    void GameExitFunc()
    {
        Application.Quit();
    }
}
