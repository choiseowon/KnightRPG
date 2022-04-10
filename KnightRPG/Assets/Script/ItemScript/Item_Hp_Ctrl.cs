using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Hp_Ctrl : Item_Class
{
    public Item_Hp_Ctrl() : base(ItemType.HpItem, GlobalData.hpItem_Array) { }
    public RawImage item_Img = null;
    public Texture[] item_Texture;
    string[] item_Name = { "Ãµ °©¿Ê", "°¡Á× °©¿Ê", "°­Ã¶ °©¿Ê", "¹Ì½º¸± °©¿Ê", "¿ë»çÀÇ °©¿Ê" };
    string[] item_Str = { "Ã¼·Â 100% Áõ°¡", "Ã¼·Â 150% Áõ°¡",
                                "Ã¼·Â 200% Áõ°¡", "Ã¼·Â 250% Áõ°¡", "Ã¼·Â 300% Áõ°¡" };
    int[] item_Cost = { 100, 200, 300, 400, 500 };

    public override void ItemSetting()
    {
        name_Txt.text = item_Name[item_Index];
        value_Txt.text = item_Str[item_Index];
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

        for (int ii = 0; ii < GlobalData.hpItem_Array.Length; ii++)
        {
            sum += sum * GlobalData.hpItem_Array[ii] * (ii + 2) * 0.5f;
        }

        GlobalData.hp_Value = sum;
        Player_Ctrl.Inst.PlayerSetting();
        StartCoroutine(GlobalData.SaveItemCo(GlobalData.user_Number));
    }

    void Start()
    {
        if (ok_Btn != null)
            ok_Btn.onClick.AddListener(() =>
            {
                if (GlobalData.user_Dia < item_Cost[item_Index])
                {
                    DlgBox_Ctrl.Inst.DlgBoxSetting("´ÙÀÌ¾Æ°¡ ºÎÁ·ÇÕ´Ï´Ù", null);
                    return;
                }

                DlgBox_Ctrl.Inst.DlgBoxSetting(item_Name[item_Index] + "À» ±¸¸ÅÇÕ´Ï±î?", ItemBuy, true);
            });
    }
}
