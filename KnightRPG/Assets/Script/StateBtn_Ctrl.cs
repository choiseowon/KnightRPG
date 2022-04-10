using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum StateType
{
    AttPoint,
    HealthPoint,
    CriticalRand,
    CriticalPower,
    Count
}

public class StateBtn_Ctrl : MonoBehaviour
{
    public StateType stateType = StateType.Count;
    public Button state_Btn = null;
    public Text state_Txt = null;
    public Text gold_Txt = null;
    int state_Lev = 0;
    float[] state_Cost = new float[18];
    bool btn_Bool = false;

    IEnumerator BtnDealy_Co;

    void Start()
    {
        state_Lev = GetStateLev(stateType);
        state_Cost[0] = 100;
        state_Cost = StateUpdate(state_Cost, state_Lev);
        state_Txt.text = UpdateText(stateType, state_Txt, state_Lev).text;
        gold_Txt.text = "가격 : " + GlobalData.MymoneyToString(state_Cost);

        EventTrigger trigger = state_Btn.GetComponent<EventTrigger>();
        EventTrigger.Entry entryUp = new EventTrigger.Entry();
        entryUp.eventID = EventTriggerType.PointerUp;
        entryUp.callback.AddListener((data) =>
        {
            PointUp();
        }); 

        EventTrigger.Entry entryDown = new EventTrigger.Entry();
        entryDown.eventID = EventTriggerType.PointerDown;
        entryDown.callback.AddListener((data) =>
        {
            PointDown();
        });

        if (trigger != null)
        {
            trigger.triggers.Add(entryUp);
            trigger.triggers.Add(entryDown);
        }

        if (state_Btn != null)
        {
            state_Btn.onClick.AddListener(() =>
            {
                Sound_Ctrl.Inst.SfSoundPlay("Buy", "Ui");
                ButtonFunc();
            });
        }

        if (stateType == StateType.CriticalRand && GlobalData.user_CriRandLv >= 1000)
            state_Btn.gameObject.SetActive(false);
    }

    void ButtonFunc()
    {

        if (stateType == StateType.CriticalRand && GlobalData.user_CriRandLv >= 1000)
            return;

        int gold_index = 0;

        for (int ii = 0; ii < GlobalData.user_Gold.Length; ii++)
        {
            if (GlobalData.user_Gold[ii] > 0)
                gold_index = ii;
        }

        int cost_index = 0;

        for (int ii = 0; ii < state_Cost.Length; ii++)
        {
            if (state_Cost[ii] > 0)
                cost_index = ii;
        }

        if (gold_index < cost_index)
            return;

        if (GlobalData.user_Gold[gold_index] < state_Cost[gold_index])
            return;

        MainUI_Ctrl.Inst.GetGold(state_Cost, GlobalData.user_Gold, false);

        state_Lev++;
        state_Cost = StateUpdate(state_Cost, 1);
        SetStateLev(stateType, state_Lev);

        state_Txt.text = UpdateText(stateType, state_Txt, state_Lev).text;
        gold_Txt.text = "가격 : " + GlobalData.MymoneyToString(state_Cost);

        Player_Ctrl.Inst.PlayerSetting();

        if (stateType == StateType.CriticalRand && GlobalData.user_CriRandLv >= 1000)
        {
            PointUp();
            state_Btn.gameObject.SetActive(false);
            gold_Txt.text = "";
        }
    }

    public void PointUp()
    {
        StopCoroutine(BtnDealy_Co);
        btn_Bool = false;
    }

    public void PointDown()
    {
        BtnDealy_Co = ButtonDealy_Co();
        StartCoroutine(BtnDealy_Co);
    }

    IEnumerator ButtonDealy_Co()
    {
        yield return new WaitForSeconds(0.5f);
        btn_Bool = true;
        Sound_Ctrl.Inst.SfSoundPlay("Buy", "Ui");
    }

    float[] StateUpdate(float[] array, int state_Lev)
    {
        for (int ii = 0; ii < state_Lev; ii++)
        {
            for (int kk = array.Length - 1; kk >= 0; kk--)
            {
                if (array[kk] <= 0)
                    continue;

                array[kk] = array[kk] * 1.005f;
                array = GlobalData.Theorem(array);
            }
        }

        return array;
    }

    void SetStateLev(StateType a_tpye, int state_Lev)
    {
        switch (a_tpye)
        {
            case StateType.AttPoint:
                GlobalData.user_APLv = state_Lev;
                break;
            case StateType.HealthPoint:
                GlobalData.user_HPLv = state_Lev;
                break;
            case StateType.CriticalRand:
                GlobalData.user_CriRandLv = state_Lev;
                break;
            case StateType.CriticalPower:
                GlobalData.user_CriPowLv = state_Lev;
                break;
        }
    }

    int GetStateLev(StateType a_tpye)
    {
        int value = 0;

        switch (a_tpye)
        {
            case StateType.AttPoint:
                value = GlobalData.user_APLv;
                break;
            case StateType.HealthPoint:
                value = GlobalData.user_HPLv;
                break;
            case StateType.CriticalRand:
                value = GlobalData.user_CriRandLv;
                break;
            case StateType.CriticalPower:
                value = GlobalData.user_CriPowLv;
                break;
        }

        return value;
    }

    Text UpdateText(StateType a_type, Text a_Text, int state_Lev)
    {
        state_Lev++;

        switch (a_type)
        {
            case StateType.AttPoint:
                {
                    float[] state_Array = new float[18];
                    state_Array[0] = 10;

                    for (int ii = 0; ii < state_Lev; ii++)
                    {
                        state_Array[0] += 10;
                        state_Array = GlobalData.Theorem(state_Array);
                    }

                    a_Text.text = "공격력 " + GlobalData.MymoneyToString(state_Array) +" 증가";
                }
                break;
            case StateType.HealthPoint:
                {
                    float[] state_Array = new float[18];
                    state_Array[0] = 100;

                    for (int ii = 0; ii < state_Lev; ii++)
                    {
                        state_Array[0] += 100;
                        state_Array = GlobalData.Theorem(state_Array);
                    }

                    a_Text.text = "체력 " + GlobalData.MymoneyToString(state_Array) + " 증가";
                }
                break;
            case StateType.CriticalRand:
                a_Text.text = "치명타 확률 " + (state_Lev * 0.1f) + " % 증가";
                break;
            case StateType.CriticalPower:
                a_Text.text = "치명타 피해 " + (state_Lev * 0.1f) + " % 증가";
                break;
        }

        return a_Text;
    }

    void Update()
    {
        if(btn_Bool == true)
            ButtonFunc();
    }
}
