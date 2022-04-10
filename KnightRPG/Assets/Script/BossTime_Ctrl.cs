using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossTime_Ctrl : MonoBehaviour
{
    public Image time_Image = null;
    float time = 20.0f;

    void OnEnable()
    {
        time = 20.0f;
    }

    void FixedUpdate()
    {
        time -= 0.01f;
        time_Image.fillAmount = time / 20.0f;

        if (time <= 0.0f)
            Player_Ctrl.Inst.BossBattleEnd();

    }
}
