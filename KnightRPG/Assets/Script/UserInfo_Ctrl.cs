using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInfo_Ctrl : MonoBehaviour
{
    public Text attPoint_Txt = null;
    public Text criPoint_Txt = null;
    public Text hpPoint_Txt = null;
    public Text speedPoint_Txt = null;
    public Text movePoint_Txt = null;
    public Text goldPoint_Txt = null;

    float[] gold_Point = new float[18];
    float[] chest_Point = new float[18];
    float[] boss_Point = new float[18];

    string cri_Str = "";

    void OnEnable()
    {
        GoldFunc();
        chest_Point = GoldUpdate(5);
        boss_Point = GoldUpdate(10);

        attPoint_Txt.text = Player_Ctrl.Inst.StateGet("Attack") + "\n" +
                            GlobalData.StringCount((GlobalData.user_APLv + 1) * 10) + "\n" + (GlobalData.att_Value - 1) * 100 + "%";

        if (((GlobalData.user_CriPowLv + 2000) * 0.1f) < 10000)
            cri_Str = ((GlobalData.user_CriPowLv + 2000) * 0.1f).ToString();
        else
            cri_Str = GlobalData.StringCount((int)((GlobalData.user_CriPowLv + 2000) * 0.1f));

        criPoint_Txt.text = (GlobalData.user_CriRandLv * 0.1f) + "%\n" + Player_Ctrl.Inst.StateGet("CriPow") + "\n" +
                            cri_Str + "%\n" + (GlobalData.cri_Value - 1) * 100 + "%";

        hpPoint_Txt.text = Player_Ctrl.Inst.StateGet("Hp") + "\n" +
                            GlobalData.StringCount((GlobalData.user_HPLv + 1) * 100) + "\n" + (GlobalData.hp_Value - 1) * 100 + "%";

        speedPoint_Txt.text = (1.0f * GlobalData.speed_Value) + "\n" + "1" + "\n" + (GlobalData.speed_Value - 1) * 100 + "%";

        movePoint_Txt.text = (0.02f * GlobalData.move_Value) + "\n" + "0.02" + "\n" + (GlobalData.move_Value - 1) * 100 + "%";

        goldPoint_Txt.text = GlobalData.MymoneyToString(gold_Point) + "\n" + GlobalData.MymoneyToString(chest_Point) + "\n" +
                              GlobalData.MymoneyToString(boss_Point) + "\n" + (GlobalData.gold_Value - 1) * 100 + "%";
    }

    void GoldFunc()
    {
        gold_Point = new float[18];
        gold_Point[0] = 100 * GlobalData.gold_Value;
        gold_Point = GlobalData.Theorem(gold_Point);

        for (int ii = 0; ii < GlobalData.user_BossNowLv; ii++)
            for (int kk = gold_Point.Length - 1; kk >= 0; kk--)
            {
                if (gold_Point[kk] <= 0)
                    continue;

                gold_Point[kk] = gold_Point[kk] * 2;
                gold_Point = GlobalData.Theorem(gold_Point);
            }
    }

    float[] GoldUpdate(int value)
    {
        float[] array = new float[18];

        for (int ii = gold_Point.Length - 1; ii >= 0; ii--)
        {
            array[ii] = 0;
            array[ii] = gold_Point[ii] * value;
            array = GlobalData.Theorem(array);
        }

        return array;
    }

    void Start()
    {
    }

    void Update()
    {
        
    }
}
