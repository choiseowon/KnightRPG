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

    public static string[] CountArray = { "", "��", "��", "��", "��", "��", "��", "��", "��", 
        "��", "��", "��", "��", "���ϻ�", "�ƽ±�", "����Ÿ", "�Ұ�����", "�������" };

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

    public static float[] Theorem(float[] Money)    // �迭�� �����ϴ� �Լ�
    {
        int index = 0;      // ���� �ִ� ���� ū �ε����� �����ϱ� ���� ����
        float fir_value = 0.0f;     // �ڸ����� ���� �����ϱ� ���� ����
        float sec_value = 0.0f;     // �ڸ����� ���� �����ϱ� ���� ����

        for (int ii = 0; ii < CountArray.Length; ii++)      // �ִ�� ǥ�ð� ������ �ڸ��� ��ŭ �ݺ�
        {
            if (Money[ii] > 0)      // ���� 0�� �ƴ� ���
                index = ii;
        }

        for (int ii = 0; ii <= index; ii++)
        {
            Money[ii] = Mathf.Round(Money[ii] * 100) * 0.01f;       // �Ҽ��� 2��° �ڸ����� �ݿø�

            if(Money[ii] < 1 && ii > 0)     // ���� 1 ���� ������ �ε����� 0���� Ŭ ���
            {
                fir_value = Money[ii] * 10000;      // �ش� �ε����� ���� ���� �ڸ����� ���߱� ���� 10000�� ������
                sec_value = Money[ii - 1] + fir_value;      // �Ѵܰ� ���� �ڸ��� ���� ���� ���� ����

                if(sec_value >= 10000)      // �Ѵܰ� ���� �ڸ����� ���� ���� 10000�� �Ѿ��� ���
                {
                    Money[ii - 1] = sec_value - 10000;      // �Ѵܰ� ���� �ڸ����� ���� ����
                    Money[ii] = 1;      // 10000�� �Ѿ����� ���� 1�� �������ش� ���� ���� �Ʒ� �ڸ����� �������� ���� ������ ģ��
                }
            }

            while (Money[ii] >= 10000)      // �ش� �ε����� ���� 10000���� ũ�� ��� �ݺ�
            {
                Money[ii] -= 10000;     // 10000 �� ����
                Money[ii + 1] += 1;     // �Ѵܰ� �� �ڸ����� 1�� �����ش�.
            }

            if (Money[ii] < 0)      // �ش� �ε����� ���� -�� ���
                if (index > ii)     // ���� �ִ� �ε��� �� ���� ū ���� �ƴ� ��츸
                {
                    Money[ii + 1] -= 1;     // �Ѵܰ� �� �ڸ������� 1�� ����
                    Money[ii] += 10000;     // �Ѵܰ� �� �ڸ������� 1�� ���������� 10000�� �����ش�.
                }
        }

        return Money;   // ������ �迭�� ��ȯ
    }

    public static string MymoneyToString(float[] Money)     // �迭�� ���ڿ��� ǥ�����ֱ� ���� �Լ�
    {
        int index = 0;      // ���� �ִ� ���� ū �ε����� �����ϱ� ���� ����

        for (int ii = 0; ii < CountArray.Length; ii++)      // �ִ�� ǥ�ð� ������ �ڸ��� ��ŭ �ݺ�
        {
            if (Money[ii] > 0)      // ���� 0�� �ƴ� ���
                index = ii;
        }

        float money_float = Money[index];       // �ش� �ε����� ���� ����
        string number_1 = CountArray[index];    // �ش� �ε����� �´� �ڸ��� ǥ�� ���ڿ��� ����
        string number_2 = "";       // �ε����� �Ʒ� �ڸ��� ǥ�� ���ڿ��� ������ ����

        if (index > 0)      // �ε����� ���� ���� �Ʒ� �ڸ��� �ƴ� ���
        {
            money_float += Money[index - 1] / 10000;    // �Ʒ� �ڸ����� ���� ������
            number_2 = CountArray[index - 1];       // �Ʒ� �ڸ��� ǥ�� ���ڿ��� ����
        }

        string money_Str = money_float.ToString("N4");      // �Ҽ��� 4�ڸ����� ǥ��
        string[] array_Str = money_Str.Split('.');      // '.'�� �������� ���ڿ� �и�

        if (index == 0)     // �ڸ����� �� �Ʒ� �ڸ��� ���
            array_Str[1] = "";  // �ι�° �ε����� ���� �������� ����

        if (array_Str[1] != "")     // �ι�° �ε����� ���� ������ �ƴ� ���
        {
            int change = int.Parse(array_Str[1]);   // �ش� ���ڿ��� int ��ȯ

            if (change <= 0)    // ��ȯ�� ���� 0�̰ų� �׺��� ���� ���
            {
                array_Str[1] = "";      // ���ڿ��� �������� ����
                number_2 = "";      // �ڸ��� ǥ�� ���ڿ��� �������� ����
            }
            else
                array_Str[1] = change.ToString();   // ���ڸ� �ٽ� ���ڿ��� ��ȯ
        }

        money_Str = array_Str[0] + number_1 + array_Str[1] + number_2;      // ���ڿ� �� ���̿� �ڸ��� ǥ�� ���ڿ��� �����Ͽ� ���ο� ���ڿ� ����

        return money_Str;       // ���յ� ���ڿ��� ��ȯ
    }

    public static string StringCount(int value)     // �迭�� �ƴ� ���� �ڸ��� ǥ�ð� �ʿ��� ��� ����ϴ� �Լ�
    {
        string value_Str = value.ToString();    // int ���� ���ڿ��� ��ȯ
        int index = value_Str.Length / 4;       // �ְ� �ڸ����� ���°���� Ȯ���� �ϱ� ���� ���ڿ��� 4�� ����

        int value_change = 0;       // ���ڿ��� ��ȯ�� ���� �����ϱ� ���� ����
        int length = -1;        // ���ڿ� ������ ������ ������ ��

        List<string> value_List = new List<string>();   // ���� ����� ���ڿ��� ������� �����ϱ� ���� ����Ʈ

        for(int ii = 0; ii < index + 1; ii++)
        {
            length = value_Str.Length - ii - (ii * 4);      // 4�ڸ� ���� ���ڸ� �����ϱ� ���� �ε����� ���� ����

            if(length > 0)      // �ε����� 0 ���� ũ�ٸ�
               value_Str = value_Str.Insert(length, "*");   // �ش� �ε����� * ���� ����
        }

        string[] str = value_Str.Split('*');    // * ���� �������� ���ڸ� ������ �迭�� ����

        for (int ii = 0; ii < str.Length - 1; ii++)     // ���� ���ڿ��� ���� ��ŭ �ݺ� -1�� �������� *�̱� ����
        {
            value_change = int.Parse(str[ii]);      // ���ڸ� int������ ��ȯ�Ͽ� �� ���ڸ� ���� 0�� ���� ���� ����
            value_List.Add(value_change.ToString());    // int�� ��ȯ�� ���� �ٽ� ���ڿ��� ��ȯ�Ͽ� ����Ʈ�� �߰�
        }

        length = 0;     // *�� ������ üũ�ϱ� ���� ���� ��Ȱ��

        for (int ii = 0; ii < value_Str.Length; ii++)
        {
            if (value_Str[ii].ToString().Contains("*") == true)     // *�� �� ������ ������ 1 ����
                length++;
        }

        value_Str = "";     // �������� ���ڿ� �ʱ�ȭ

        for (int ii = 0; ii < value_List.Count; ii++)       // ����Ʈ�� ���� ��ŭ �ݺ�
        {
            value_Str += value_List[ii] + CountArray[length - 1 - ii];      // ����Ʈ�� ���ڿ��� �ڿ� �ڸ����� ǥ������ ���ڿ��� �߰�
        }

        return value_Str;       // �ϼ��� ���ڿ��� ��ȯ
    }
}