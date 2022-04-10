using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class WorldUI_Ctrl : MonoBehaviour
{
    private Transform tr;
    private Transform mainCameraTr;
    private Queue<Text> text_Pool = new Queue<Text>();

    public Transform text_Root = null;
    public Image hp_Image = null;
    Text damage_Txt = null;

    [Header("---- HP º¯¼ö ----")]
    int index = 0;
    float sum_Now = 0;
    float sum_Max = 0;
    int hp_Check = -1;

    void Start()
    {
        tr = GetComponent<Transform>();
        mainCameraTr = Camera.main.transform;

        for (int ii = 0; ii < text_Root.childCount; ii++)
            text_Pool.Enqueue(text_Root.GetChild(ii).GetComponent<Text>());
    }

    void LateUpdate()
    {
        tr.forward = mainCameraTr.forward;
    }

    public void TextActive()
    {
        for(int ii = 0; ii < text_Root.childCount; ii++)
            text_Root.GetChild(ii).gameObject.SetActive(false);
    }

    public void DamageText(float[] damage_Point, Color color)
    {
        damage_Txt = text_Pool.Dequeue();
        damage_Txt.gameObject.SetActive(false);
        damage_Txt.gameObject.SetActive(true);
        damage_Txt.color = color;
        damage_Txt.text = GlobalData.MymoneyToString(damage_Point);

        text_Pool.Enqueue(damage_Txt);
    }

    public bool HpUpdate(float[] hp_Max, float[] hp_Now)
    {
        index = 0;

        sum_Now = 0;
        sum_Max = 0;

        for (int ii = hp_Max.Length - 1; ii >= 0; ii--)
        {
            if (hp_Max[ii] > 0)
            {
                if (ii > 0)
                    sum_Max = (hp_Max[ii] * 10000) + hp_Max[ii - 1];
                else
                    sum_Max = hp_Max[ii] * 10000;

                index = ii;
                break;
            }
        }

        hp_Check = Array.FindIndex(hp_Now, x => x < 0.0f);

        if (hp_Check > 0)
        {
            sum_Now = 0;
        }
        else
        {
            if (index > 0)
                if (hp_Now[index] > 0)
                    sum_Now = (hp_Now[index] * 10000) + hp_Now[index - 1];
                else
                    sum_Now = hp_Now[index - 1];
            else
                if (hp_Now[index] > 0)
                    sum_Now = hp_Now[index] * 10000;
        }

        hp_Image.fillAmount = sum_Now / sum_Max;

        if (hp_Image.fillAmount > 0.0f)
            return false;
        else
            return true;
    }
}
