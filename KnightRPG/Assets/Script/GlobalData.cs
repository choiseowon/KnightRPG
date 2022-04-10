using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;

public class GlobalData
{
    public static int user_Number = -1;
    public static string user_Nick = "";

    public static float bgm_Volume = 1.0f;
    public static float sf_Volume = 1.0f;

    public static float[] user_Gold = new float[18];
    public static int user_Dia = -1;

    public static int user_APLv = -1;
    public static int user_HPLv = -1;
    public static int user_CriRandLv = -1;
    public static int user_CriPowLv = -1;
    //public static int user_SpeedLv = -1;
    //public static int user_MoveLv = -1;
    public static int user_BossMaxLv = -1;
    public static int user_BossNowLv = -1;

    public static int[] attItem_Array = new int[5];
    public static int[] hpItem_Array = new int[5];
    public static int[] speedItem_Array = new int[5];
    public static int[] moveItem_Array = new int[5];
    public static int[] criItem_Array = new int[5];
    public static int[] goldItem_Array = new int[5];

    public static float att_Value = 1.0f;
    public static float hp_Value = 1.0f;
    public static float speed_Value = 1.0f;
    public static float move_Value = 1.0f;
    public static float cri_Value = 1.0f;
    public static float gold_Value = 1.0f;

    public static string[] CountArray = { "", "만", "억", "조", "경", "해", "자", "양", "구", 
        "간", "정", "재", "극", "항하사", "아승기", "나유타", "불가사의", "무량대수" };

    public static string SaveDataUrl = "http://seowonserver.dothome.co.kr/KnightDB/SaveDataUrl.php";
    public static string SaveItemUrl = "http://seowonserver.dothome.co.kr/KnightDB/SaveItemUrl.php";
    public static string LoadDataUrl = "http://seowonserver.dothome.co.kr/KnightDB/LoadDataUrl.php";
    public static string LoadItemUrl = "http://seowonserver.dothome.co.kr/KnightDB/LoadItemUrl.php";

    public static IEnumerator SaveDataCo(int user_Number)
    {
        WWWForm form = new WWWForm();
        string gold_Str = "";

        form.AddField("Input_number", user_Number);

        for(int ii = 0; ii < user_Gold.Length; ii++)
            gold_Str += user_Gold[ii] + " ";

        form.AddField("Input_gold", gold_Str, System.Text.Encoding.UTF8);
        form.AddField("Input_dia", user_Dia);
        form.AddField("Input_attLv", user_APLv);
        form.AddField("Input_hpLv", user_HPLv);
        form.AddField("Input_crLv", user_CriRandLv);
        form.AddField("Input_cpLv", user_CriPowLv);
        form.AddField("Input_bossMaxLv", user_BossMaxLv);
        form.AddField("Input_bossNowLv", user_BossNowLv);

        UnityWebRequest a_www = UnityWebRequest.Post(SaveDataUrl, form);
        yield return a_www.SendWebRequest();

        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);

            if (sz.Contains("Data Save OK!") == false)
                yield break;

            Debug.Log("Data Save OK!");
        }
        else
        {
            Debug.Log(a_www.error);
        }
    }

    public static IEnumerator SaveItemCo(int user_Number)
    {
        WWWForm form = new WWWForm();
        string att_Str = "";
        string hp_Str = "";
        string speed_Str = "";
        string move_Str = "";
        string cri_Str = "";
        string gold_Str = "";

        form.AddField("Input_number", user_Number);

        for (int ii = 0; ii < attItem_Array.Length; ii++)
            att_Str += attItem_Array[ii] + " ";

        for (int ii = 0; ii < hpItem_Array.Length; ii++)
            hp_Str += hpItem_Array[ii] + " ";

        for (int ii = 0; ii < speedItem_Array.Length; ii++)
            speed_Str += speedItem_Array[ii] + " ";

        for (int ii = 0; ii < moveItem_Array.Length; ii++)
            move_Str += moveItem_Array[ii] + " ";

        for (int ii = 0; ii < criItem_Array.Length; ii++)
            cri_Str += criItem_Array[ii] + " ";

        for (int ii = 0; ii < goldItem_Array.Length; ii++)
            gold_Str += goldItem_Array[ii] + " ";

        form.AddField("Input_attitem", att_Str, System.Text.Encoding.UTF8);
        form.AddField("Input_hpitem", hp_Str, System.Text.Encoding.UTF8);
        form.AddField("Input_speeditem", speed_Str, System.Text.Encoding.UTF8);
        form.AddField("Input_moveitem", move_Str, System.Text.Encoding.UTF8);
        form.AddField("Input_criitem", cri_Str, System.Text.Encoding.UTF8);
        form.AddField("Input_golditem", gold_Str, System.Text.Encoding.UTF8);

        UnityWebRequest a_www = UnityWebRequest.Post(SaveItemUrl, form);
        yield return a_www.SendWebRequest();

        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);

            if (sz.Contains("Item Save OK!") == false)
                yield break;

            Debug.Log("Item Save OK!");
        }
        else
        {
            Debug.Log(a_www.error);
        }
    }

    public static IEnumerator LoadDataCo(int user_Number)
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_number", user_Number);

        UnityWebRequest a_www = UnityWebRequest.Post(LoadDataUrl, form);
        yield return a_www.SendWebRequest();

        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);

            if (sz.Contains("Data Load OK!") == false)
                yield break;

            var N = JSON.Parse(sz);
            string sum_Str = "";

            if (N["user_gold"] != null)
                sum_Str = N["user_gold"];

            string[] str = sum_Str.Split(' ');

            for (int ii = 0; ii < str.Length - 1; ii++)
                user_Gold[ii] = float.Parse(str[ii]);

            if (N["user_dia"] != null)
                user_Dia = N["user_dia"];

            if (N["att_Lv"] != null)
                user_APLv = N["att_Lv"];

            if (N["hp_Lv"] != null)
                user_HPLv = N["hp_Lv"];

            if (N["criRand_Lv"] != null)
                user_CriRandLv = N["criRand_Lv"];

            if (N["criPow_Lv"] != null)
                user_CriPowLv = N["criPow_Lv"];

            if (N["bossMax_Lv"] != null)
                user_BossMaxLv = N["bossMax_Lv"];

            if (N["bossNow_Lv"] != null)
                user_BossNowLv = N["bossNow_Lv"];

            Debug.Log("Data Load OK!");
        }
        else
        {
            Debug.Log(a_www.error);
        }
    }

    public static IEnumerator LoadItemCo(int user_Number)
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_number", user_Number);

        UnityWebRequest a_www = UnityWebRequest.Post(LoadItemUrl, form);
        yield return a_www.SendWebRequest();

        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);

            if (sz.Contains("Item Load OK!") == false)
                yield break;

            var N = JSON.Parse(sz);
            string sum_Str = "";

            if (N["att_item"] != null)
                sum_Str = N["att_item"];

            string[] str = sum_Str.Split(' ');

            for (int ii = 0; ii < attItem_Array.Length; ii++)
            {
                if(str.Length <= ii)
                    attItem_Array[ii] = 0;
                else
                    attItem_Array[ii] = int.Parse(str[ii]);
            }
                

            if (N["hp_item"] != null)
                sum_Str = N["hp_item"];

            str = sum_Str.Split(' ');

            for (int ii = 0; ii < hpItem_Array.Length; ii++)
            {
                if (str.Length <= ii)
                    hpItem_Array[ii] = 0;
                else
                    hpItem_Array[ii] = int.Parse(str[ii]);
            }

            if (N["speed_item"] != null)
                sum_Str = N["speed_item"];

            str = sum_Str.Split(' ');

            for (int ii = 0; ii < speedItem_Array.Length; ii++)
            {
                if (str.Length <= ii)
                    speedItem_Array[ii] = 0;
                else
                    speedItem_Array[ii] = int.Parse(str[ii]);
            }

            if (N["move_item"] != null)
                sum_Str = N["move_item"];

            str = sum_Str.Split(' ');

            for (int ii = 0; ii < moveItem_Array.Length; ii++)
            {
                if (str.Length <= ii)
                    moveItem_Array[ii] = 0;
                else
                    moveItem_Array[ii] = int.Parse(str[ii]);
            }

            if (N["cri_item"] != null)
                sum_Str = N["cri_item"];

            str = sum_Str.Split(' ');

            for (int ii = 0; ii < criItem_Array.Length; ii++)
            {
                if (str.Length <= ii)
                    criItem_Array[ii] = 0;
                else
                    criItem_Array[ii] = int.Parse(str[ii]);
            }

            if (N["gold_item"] != null)
                sum_Str = N["gold_item"];

            str = sum_Str.Split(' ');

            for (int ii = 0; ii < goldItem_Array.Length; ii++)
            {
                if (str.Length <= ii)
                    goldItem_Array[ii] = 0;
                else
                    goldItem_Array[ii] = int.Parse(str[ii]);
            }

            att_Value = 1.0f;

            for (int ii = 0; ii < attItem_Array.Length; ii++)
                att_Value += att_Value * attItem_Array[ii] * (ii + 2) * 0.5f;

            hp_Value = 1.0f;

            for (int ii = 0; ii < hpItem_Array.Length; ii++)
                hp_Value += hp_Value * hpItem_Array[ii] * (ii + 2) * 0.5f;

            speed_Value = 1.0f;

            for (int ii = 0; ii < speedItem_Array.Length; ii++)
                speed_Value += (speed_Value * 0.5f) * speedItem_Array[ii];

            move_Value = 1.0f;

            for (int ii = 0; ii < moveItem_Array.Length; ii++)
                move_Value += (move_Value * 0.3f) * moveItem_Array[ii];

            cri_Value = 1.0f;

            for (int ii = 0; ii < criItem_Array.Length; ii++)
                cri_Value += cri_Value * criItem_Array[ii] * (ii + 2) * 0.5f;

            gold_Value = 1.0f;

            for (int ii = 0; ii < goldItem_Array.Length; ii++)
                gold_Value += gold_Value * goldItem_Array[ii] * (ii + 2) * 0.5f;

            Debug.Log("Item Load OK!");
        }
        else
        {
            Debug.Log(a_www.error);
        }
    }

    public static float[] Theorem(float[] Money)    // 배열을 정리하는 함수
    {
        int index = 0;      // 값이 있는 제일 큰 인덱스를 저장하기 위한 변수
        float fir_value = 0.0f;     // 자리수의 값을 저장하기 위한 변수
        float sec_value = 0.0f;     // 자리수의 값을 저장하기 위한 변수

        for (int ii = 0; ii < CountArray.Length; ii++)      // 최대로 표시가 가능한 자리수 만큼 반복
        {
            if (Money[ii] > 0)      // 값이 0이 아닐 경우
                index = ii;
        }

        for (int ii = 0; ii <= index; ii++)
        {
            Money[ii] = Mathf.Round(Money[ii] * 100) * 0.01f;       // 소수점 2번째 자리까지 반올림

            if(Money[ii] < 1 && ii > 0)     // 값이 1 보다 작으며 인덱스가 0보다 클 경우
            {
                fir_value = Money[ii] * 10000;      // 해당 인덱스의 값을 이전 자리수와 맞추기 위해 10000을 곱해줌
                sec_value = Money[ii - 1] + fir_value;      // 한단계 낮은 자리의 수와 더한 값을 저장

                if(sec_value >= 10000)      // 한단계 낮은 자리수와 더한 값이 10000을 넘었을 경우
                {
                    Money[ii - 1] = sec_value - 10000;      // 한단계 낮은 자리수의 값을 정리
                    Money[ii] = 1;      // 10000이 넘었으니 값을 1로 변경해준다 이전 값은 아래 자리수와 더했으니 없는 값으로 친다
                }
            }

            while (Money[ii] >= 10000)      // 해당 인덱스의 값이 10000보다 크면 계속 반복
            {
                Money[ii] -= 10000;     // 10000 을 감소
                Money[ii + 1] += 1;     // 한단계 위 자리수에 1을 더해준다.
            }

            if (Money[ii] < 0)      // 해당 인덱스의 값이 -일 경우
                if (index > ii)     // 값이 있는 인덱스 중 가장 큰 수가 아닐 경우만
                {
                    Money[ii + 1] -= 1;     // 한단계 위 자리수에서 1을 감소
                    Money[ii] += 10000;     // 한단계 위 자리수에서 1을 감소했으니 10000을 더해준다.
                }
        }

        return Money;   // 정리된 배열을 반환
    }

    public static string MymoneyToString(float[] Money)     // 배열을 문자열로 표시해주기 위한 함수
    {
        int index = 0;      // 값이 있는 제일 큰 인덱스를 저장하기 위한 변수

        for (int ii = 0; ii < CountArray.Length; ii++)      // 최대로 표시가 가능한 자리수 만큼 반복
        {
            if (Money[ii] > 0)      // 값이 0이 아닐 경우
                index = ii;
        }

        float money_float = Money[index];       // 해당 인덱스의 값을 저장
        string number_1 = CountArray[index];    // 해당 인덱스와 맞는 자리수 표시 문자열을 저장
        string number_2 = "";       // 인덱스의 아래 자리수 표시 문자열을 저장할 변수

        if (index > 0)      // 인덱스의 값이 제일 아래 자리가 아닐 경우
        {
            money_float += Money[index - 1] / 10000;    // 아래 자리수의 값을 더해줌
            number_2 = CountArray[index - 1];       // 아래 자리수 표시 문자열을 저장
        }

        string money_Str = money_float.ToString("N4");      // 소수점 4자리까지 표시
        string[] array_Str = money_Str.Split('.');      // '.'을 기준으로 문자열 분리

        if (index == 0)     // 자리수가 맨 아래 자리일 경우
            array_Str[1] = "";  // 두번째 인덱스의 값은 공백으로 설정

        if (array_Str[1] != "")     // 두번째 인덱스의 값이 공백이 아닐 경우
        {
            int change = int.Parse(array_Str[1]);   // 해당 문자열을 int 변환

            if (change <= 0)    // 변환한 값이 0이거나 그보다 작을 경우
            {
                array_Str[1] = "";      // 문자열을 공백으로 설정
                number_2 = "";      // 자리수 표시 문자열을 공백으로 설정
            }
            else
                array_Str[1] = change.ToString();   // 숫자를 다시 문자열로 변환
        }

        money_Str = array_Str[0] + number_1 + array_Str[1] + number_2;      // 숫자와 그 사이에 자리수 표시 문자열을 삽입하여 새로운 문자열 생성

        return money_Str;       // 결합된 문자열을 반환
    }

    public static string StringCount(int value)     // 배열이 아닌 값도 자리수 표시가 필요한 경우 사용하는 함수
    {
        string value_Str = value.ToString();    // int 값을 문자열로 변환
        int index = value_Str.Length / 4;       // 최고 자리수가 몇번째인지 확인을 하기 위해 문자열을 4로 나눔

        int value_change = 0;       // 문자열을 변환한 값을 저장하기 위한 변수
        int length = -1;        // 문자열 길이의 비율을 저장할 값

        List<string> value_List = new List<string>();   // 최종 출력할 문자열을 순서대로 저장하기 위한 리스트

        for(int ii = 0; ii < index + 1; ii++)
        {
            length = value_Str.Length - ii - (ii * 4);      // 4자리 마다 문자를 삽입하기 위해 인덱스의 값을 구함

            if(length > 0)      // 인덱스가 0 보다 크다면
               value_Str = value_Str.Insert(length, "*");   // 해당 인덱스에 * 문자 삽입
        }

        string[] str = value_Str.Split('*');    // * 문자 기준으로 문자를 나누어 배열로 저장

        for (int ii = 0; ii < str.Length - 1; ii++)     // 나눈 문자열의 개수 만큼 반복 -1은 마지막은 *이기 때문
        {
            value_change = int.Parse(str[ii]);      // 문자를 int값으로 변환하여 맨 앞자리 수가 0이 들어가는 것을 막음
            value_List.Add(value_change.ToString());    // int로 변환한 값을 다시 문자열로 변환하여 리스트에 추가
        }

        length = 0;     // *의 개수를 체크하기 위한 변수 재활용

        for (int ii = 0; ii < value_Str.Length; ii++)
        {
            if (value_Str[ii].ToString().Contains("*") == true)     // *이 들어가 있으면 변수값 1 증가
                length++;
        }

        value_Str = "";     // 리턴해줄 문자열 초기화

        for (int ii = 0; ii < value_List.Count; ii++)       // 리스트의 길이 만큼 반복
        {
            value_Str += value_List[ii] + CountArray[length - 1 - ii];      // 리스트의 문자열과 뒤에 자리수를 표현해줄 문자열을 추가
        }

        return value_Str;       // 완성된 문자열을 반환
    }
}