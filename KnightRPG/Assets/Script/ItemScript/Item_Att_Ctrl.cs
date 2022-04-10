using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Att_Ctrl : Item_Class
{
    public Item_Att_Ctrl() : base(ItemType.AttItem, GlobalData.attItem_Array) { }
    public RawImage item_Img = null;
    public Texture[] item_Texture;
    string[] item_Name = { "낡은 검", "강철 검", "미스릴 검", "기사의 검", "용사의 검" };
    string[] item_Str = { "공격력 100% 증가", "공격력 150% 증가", 
                                "공격력 200% 증가", "공격력 250% 증가", "공격력 300% 증가" };
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

        for (int ii = 0; ii < GlobalData.attItem_Array.Length; ii++)
        {
            sum += sum * GlobalData.attItem_Array[ii] * (ii + 2) * 0.5f;
        }

        GlobalData.att_Value = sum;
        Player_Ctrl.Inst.PlayerSetting();
        StartCoroutine(GlobalData.SaveItemCo(GlobalData.user_Number));
    }

    void Start()
    {
        if (ok_Btn != null)     // 아이템 구매를 누를 경우 실행되는 내용
            ok_Btn.onClick.AddListener(() =>
            {
                if (GlobalData.user_Dia < item_Cost[item_Index])    // 아이템 가격이 소지한 다이아의 개수보다 높을 경우
                {
                    DlgBox_Ctrl.Inst.DlgBoxSetting("다이아가 부족합니다", null);     // 델리게이트에 추가할 함수 없이 단일 선택지로 다이얼로그 박스 생성
                    return;
                }

                DlgBox_Ctrl.Inst.DlgBoxSetting(item_Name[item_Index] + "을 구매합니까?", ItemBuy, true);
                // 아이템 구매를 확인하는 2중 선택지 다이얼로그 박스 생성 yes 버튼을 누르면 ItemBuy 함수를 호출하도록 델리게이트에 해당 함수를 추가
            });
    }
}
