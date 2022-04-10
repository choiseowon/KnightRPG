using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Item_Class : MonoBehaviour
{
    protected ItemType item_Type = ItemType.Count;
    protected int[] item_Array;
    public int item_Index = -1;

    public Text name_Txt = null;
    public Text value_Txt = null;
    public Button ok_Btn = null;
    public Text ok_Txt = null;
    public GameObject soldOut_Root = null;

    public Item_Class(ItemType a_Type, int[] a_Array)
    {
        item_Type = a_Type;
        item_Array = a_Array;
    }

    public abstract void ItemSetting();
    public abstract void ItemBuy();
    public abstract void ItemState();

}
