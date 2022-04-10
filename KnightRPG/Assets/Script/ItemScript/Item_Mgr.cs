using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Mgr : MonoBehaviour
{
    public ScrollRect[] item_View;

    GameObject item_Att;
    GameObject item_Hp;
    GameObject item_Speed;
    GameObject item_Move;
    GameObject item_Cri;
    GameObject item_Gold;
    Button[] tab_Btn;

    void Awake()
    {
        item_Att = Resources.Load<GameObject>("Prefabs/Item/Item_Att");
        item_Hp = Resources.Load<GameObject>("Prefabs/Item/Item_Hp");
        item_Speed = Resources.Load<GameObject>("Prefabs/Item/Item_Speed");
        item_Move = Resources.Load<GameObject>("Prefabs/Item/Item_Move");
        item_Cri = Resources.Load<GameObject>("Prefabs/Item/item_Cri");
        item_Gold = Resources.Load<GameObject>("Prefabs/Item/Item_Gold");

        ItemCreate(item_Att, ItemType.AttItem);
        ItemCreate(item_Hp, ItemType.HpItem);
        ItemCreate(item_Speed, ItemType.SpeedItem);
        ItemCreate(item_Move, ItemType.MoveItem);
        ItemCreate(item_Cri, ItemType.CriItem);
        ItemCreate(item_Gold, ItemType.GoldItem);

        tab_Btn = this.transform.Find("ItemTab_Root").GetComponentsInChildren<Button>();
    }

    void ItemCreate(GameObject item, ItemType a_Type)
    {
        for (int ii = 0; ii < GlobalData.attItem_Array.Length; ii++)
        {
            GameObject obj = Instantiate(item);
            obj.transform.SetParent(item_View[(int)a_Type].content.transform);
            obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            obj.GetComponent<Item_Class>().item_Index = ii;
            obj.GetComponent<Item_Class>().ItemSetting();
        }
    }

    void Start()
    {
        ItemTabView(ItemType.AttItem, tab_Btn[0]);

        for(int ii = 0; ii < tab_Btn.Length; ii++)
        {
            if (tab_Btn[ii] != null)
            {
                Button btn = tab_Btn[ii];
                ItemType m_Type = (ItemType)ii;
                tab_Btn[ii].onClick.AddListener(() =>
                {
                    ItemTabView(m_Type, btn);
                });
            }
        }
    }

    public void ItemTabView(ItemType a_Type, Button a_Btn)
    {
        foreach (Button btn in tab_Btn)
        {
            btn.image.color = Color.white;
        }

        a_Btn.image.color = Color.gray;

        for (int ii = 0; ii < item_View.Length; ii++)
        {
            item_View[ii].gameObject.SetActive(false);
        }

        item_View[(int)a_Type].gameObject.SetActive(true);
    }
}
