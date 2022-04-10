using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Pool : MonoBehaviour
{
    public static Monster_Pool Inst = null;

    public Queue<GameObject> monster_Pool = new Queue<GameObject>();
    public Queue<GameObject> chest_Pool = new Queue<GameObject>();

    Charactor[] monster_Array;
    Charactor[] chest_Array;
    GameObject monster;
    public int monster_Count = 0;
    public int monster_Max = 1;

    int rand = 0;

    void Awake()
    {
        Inst = this;
        monster_Array = this.transform.Find("Monster_Root").GetComponentsInChildren<Charactor>(true);
        chest_Array = this.transform.Find("Chest_Root").GetComponentsInChildren<Charactor>(true);

        for (int ii = 0; ii < monster_Array.Length; ii++)
            monster_Pool.Enqueue(monster_Array[ii].gameObject);

        for (int ii = 0; ii < chest_Array.Length; ii++)
            chest_Pool.Enqueue(chest_Array[ii].gameObject);
    }

    void Start()
    {
    }

    void FixedUpdate()
    {
        if (monster_Count < monster_Max)
        {
            rand = Random.Range(0, 10);

            if(rand == 0)
                monster = chest_Pool.Dequeue();
            else
                monster = monster_Pool.Dequeue();

            monster.gameObject.SetActive(true);
            monster_Count++;
            return;
        }
    }

    public void MonStateUpdate()
    {
        for (int ii = 0; ii < monster_Array.Length; ii++)
            monster_Array[ii].GetComponent<IMonster>().MonsterSetting();

        for (int ii = 0; ii < chest_Array.Length; ii++)
            chest_Array[ii].GetComponent<IMonster>().MonsterSetting();
    }
}
