using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Gold_Ctrl : Item_Class
{
    public Item_Gold_Ctrl() : base(ItemType.GoldItem, GlobalData.goldItem_Array) { }
    public RawImage item_Img = null;
    public Texture[] item_Texture;
    string[] item_Name = { "û�� ����", "�� ����", "�� ����", "������ ����", "����� ����" };
    string item_Str = "��� ȹ�� 100% ����";
    int[] item_Cost = { 100, 200, 300, 400, 500 };

    public override void ItemSetting()
    {
        name_Txt.text = item_Name[item_Index];
        value_Txt.text = item_Str;
        ok_Txt.text = item_Cost[item_Index].ToString();
        item_Img.texture = item_Texture[item_Index];

        if (item_Array[item_Index] == 1)
            soldOut_Root.SetActive(true);
    }

    public override void ItemBuy()
    {
        MainUI_Ctrl.Inst.GetDiamond(item_Cost[item_Index], false);
        soldOut_Root.SetActive(true);
        item_Array[item_Index] = 1;

        Sound_Ctrl.Inst.SfSoundPlay("Buy", "Ui");

        ItemState();
    }

    public override void ItemState()
    {
        float sum = 1.0f;

        for (int ii = 0; ii < GlobalData.goldItem_Array.Length; ii++)
        {
            sum += sum * GlobalData.goldItem_Array[ii];
        }

        GlobalData.gold_Value = sum;
        Monster_Pool.Inst.MonStateUpdate();
        StartCoroutine(GlobalData.SaveItemCo(GlobalData.user_Number));
    }

    void Start()
    {
        if (ok_Btn != null)
            ok_Btn.onClick.AddListener(() =>
            {
                if (GlobalData.user_Dia < item_Cost[item_Index])
                {
                    DlgBox_Ctrl.Inst.DlgBoxSetting("���̾ư� �����մϴ�", null);
                    return;
                }

                DlgBox_Ctrl.Inst.DlgBoxSetting(item_Name[item_Index] + "�� �����մϱ�?", ItemBuy, true);
            });
    }
}
