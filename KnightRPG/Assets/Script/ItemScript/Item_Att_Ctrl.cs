using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Att_Ctrl : Item_Class
{
    public Item_Att_Ctrl() : base(ItemType.AttItem, GlobalData.attItem_Array) { }
    public RawImage item_Img = null;
    public Texture[] item_Texture;
    string[] item_Name = { "���� ��", "��ö ��", "�̽��� ��", "����� ��", "����� ��" };
    string[] item_Str = { "���ݷ� 100% ����", "���ݷ� 150% ����", 
                                "���ݷ� 200% ����", "���ݷ� 250% ����", "���ݷ� 300% ����" };
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
        if (ok_Btn != null)     // ������ ���Ÿ� ���� ��� ����Ǵ� ����
            ok_Btn.onClick.AddListener(() =>
            {
                if (GlobalData.user_Dia < item_Cost[item_Index])    // ������ ������ ������ ���̾��� �������� ���� ���
                {
                    DlgBox_Ctrl.Inst.DlgBoxSetting("���̾ư� �����մϴ�", null);     // ��������Ʈ�� �߰��� �Լ� ���� ���� �������� ���̾�α� �ڽ� ����
                    return;
                }

                DlgBox_Ctrl.Inst.DlgBoxSetting(item_Name[item_Index] + "�� �����մϱ�?", ItemBuy, true);
                // ������ ���Ÿ� Ȯ���ϴ� 2�� ������ ���̾�α� �ڽ� ���� yes ��ư�� ������ ItemBuy �Լ��� ȣ���ϵ��� ��������Ʈ�� �ش� �Լ��� �߰�
            });
    }
}
