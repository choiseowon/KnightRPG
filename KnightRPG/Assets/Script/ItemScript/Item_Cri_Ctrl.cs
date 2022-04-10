using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Cri_Ctrl : Item_Class
{
    public Item_Cri_Ctrl() : base(ItemType.CriItem, GlobalData.criItem_Array) { }
    public RawImage item_Img = null;
    public Texture[] item_Texture;
    string[] item_Name = { "천 투구", "가죽 투구", "강철 투구", "미스릴 투구", "용사의 투구" };
    string[] item_Str = { "치명 피해 100% 증가", "치명 피해 150% 증가",
                                "치명 피해 200% 증가", "치명 피해 250% 증가", "치명 피해 300% 증가" };
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

        for (int ii = 0; ii < GlobalData.criItem_Array.Length; ii++)
        {
            sum += sum * GlobalData.criItem_Array[ii] * (ii + 2) * 0.5f;
        }

        GlobalData.cri_Value = sum;
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
                    DlgBox_Ctrl.Inst.DlgBoxSetting("다이아가 부족합니다", null);
                    return;
                }

                DlgBox_Ctrl.Inst.DlgBoxSetting(item_Name[item_Index] + "을 구매합니까?", ItemBuy, true);
            });
    }
}
