using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI_Ctrl : MonoBehaviour
{
    public static MainUI_Ctrl Inst = null;

    public Text level_Txt = null;
    public Button up_Btn = null;
    public Button down_Btn = null;

    public Text gold_Txt = null;
    public Text getGold_Txt = null;
    public Text dia_Txt = null;
    public Text getDia_Txt = null;

    public Text clear_Txt = null;
    public Text recover_Txt = null;

    public Button boss_Btn = null;
    public Image boss_CollImg = null;
    public float boss_Dealy = 0.0f;

    public Button user_Btn = null;
    public Button item_Btn = null;
    public Button upgrade_Btn = null;

    public Button user_CBtn = null;
    public Button item_CBtn = null;
    public Button upgrade_CBtn = null;

    public GameObject user_Root = null;
    public GameObject item_Root = null;
    public GameObject upgrade_Root = null;

    public GameObject bossTime_Root = null;

    [HideInInspector] public StateBtn_Ctrl[] state_Array;

    public float[] gold_test = new float[18];

    void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        level_Txt.text = "Lv : " + GlobalData.user_BossNowLv;

        if (up_Btn != null)
            up_Btn.onClick.AddListener(() =>
            {
                if (GlobalData.user_BossNowLv < GlobalData.user_BossMaxLv)
                {
                    GlobalData.user_BossNowLv++;
                    level_Txt.text = "Lv : " + GlobalData.user_BossNowLv;
                    Monster_Pool.Inst.MonStateUpdate();
                    StartCoroutine(GlobalData.SaveDataCo(GlobalData.user_Number));
                }
            });

        if (down_Btn != null)
            down_Btn.onClick.AddListener(() =>
            {
                if (GlobalData.user_BossNowLv > 0)
                {
                    GlobalData.user_BossNowLv--;
                    level_Txt.text = "Lv : " + GlobalData.user_BossNowLv;
                    Monster_Pool.Inst.MonStateUpdate();
                    StartCoroutine(GlobalData.SaveDataCo(GlobalData.user_Number));
                }
            });

        state_Array = upgrade_Root.GetComponentsInChildren<StateBtn_Ctrl>();

        for (int ii = 0; ii < state_Array.Length; ii++)
            state_Array[ii].stateType = (StateType)ii;

        gold_Txt.text = GlobalData.MymoneyToString(GlobalData.user_Gold);
        dia_Txt.text = GlobalData.StringCount(GlobalData.user_Dia);

        if (upgrade_Btn != null)
            upgrade_Btn.onClick.AddListener(() =>
            {
                upgrade_Root.SetActive(true);
            });

        if (upgrade_CBtn != null)
            upgrade_CBtn.onClick.AddListener(() =>
            {
                upgrade_Root.SetActive(false);
            });

        if (user_Btn != null)
            user_Btn.onClick.AddListener(() =>
            {
                user_Root.SetActive(true);
            });

        if (user_CBtn != null)
            user_CBtn.onClick.AddListener(() =>
            {
                user_Root.SetActive(false);
            });

        if (item_Btn != null)
            item_Btn.onClick.AddListener(() =>
            {
                item_Root.SetActive(true);
            });

        if (item_CBtn != null)
            item_CBtn.onClick.AddListener(() =>
            {
                item_Root.SetActive(false);
            });

        if (boss_Btn != null)
            boss_Btn.onClick.AddListener(() =>
            {
                if(boss_Dealy <= 0.0f)
                    Player_Ctrl.Inst.BossChallenge();
            });

        Sound_Ctrl.Inst.BgmSoundPlay("MainBGM");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            GetDiamond(10010);
    }

    void FixedUpdate()
    {
        if(boss_Dealy > 0.0f)
        {
            boss_Dealy -= Time.deltaTime;
            boss_CollImg.fillAmount = boss_Dealy / 2.0f;

            if (boss_Dealy <= 0.0f)
                boss_Dealy = 0.0f;
        }
    }

    public void ClearFunc()
    {
        clear_Txt.gameObject.SetActive(true);
        GlobalData.user_BossNowLv = GlobalData.user_BossMaxLv;
        level_Txt.text = "Lv : " + GlobalData.user_BossNowLv;
        Sound_Ctrl.Inst.SfSoundPlay("BossClear", "Ui");
        boss_Dealy = 2.0f;
    }

    public void GetGold(float[] gold_Point, float[] user_Gold, bool pulminu = true)
    {
        for (int ii = 0; ii < gold_Point.Length; ii++)
        {
            if(pulminu == true)
                user_Gold[ii] += gold_Point[ii] * GlobalData.gold_Value;
            else if(pulminu == false)
                user_Gold[ii] -= gold_Point[ii];

            GlobalData.Theorem(user_Gold);
        }

        getGold_Txt.gameObject.SetActive(false);
        getGold_Txt.gameObject.SetActive(true);

        string updown = "";

        if (pulminu == true)
            updown = "+ ";
        else
            updown = "- ";

        updown += GlobalData.MymoneyToString(gold_Point);

        getGold_Txt.text = updown + " G";
        gold_Txt.text = GlobalData.MymoneyToString(user_Gold);

        StartCoroutine(GlobalData.SaveDataCo(GlobalData.user_Number));
    }

    public void GetDiamond(int dia_Point, bool pulminu = true)
    {
        if (pulminu == true)
            GlobalData.user_Dia += dia_Point;
        else if (pulminu == false)
            GlobalData.user_Dia -= dia_Point;

        getDia_Txt.gameObject.SetActive(false);
        getDia_Txt.gameObject.SetActive(true);

        string updown = "";

        if (pulminu == true)
            updown = "+ ";
        else
            updown = "- ";

        updown += dia_Point;

        getDia_Txt.text = updown;
        dia_Txt.text = GlobalData.StringCount(GlobalData.user_Dia);

        StartCoroutine(GlobalData.SaveDataCo(GlobalData.user_Number));
    }
}
