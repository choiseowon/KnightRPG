using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Monster_Slime_Ctrl : Charactor, IMonster, ISerch, IMove, IAttack, IHitDamage, IDeath, IMoneyDrop, ITargetLost
{
    // Charactor - ĳ���͵��� �����ϰ� �ʿ��� ������ ��Ƴ��� Ŭ����
    // IMoster - ���� �ɷ�ġ�� ������ ���� �������̽�
    // ISerch - ���� Ÿ���� ã�� �������̽� , IMove �̵��� ���� �������̽�
    // IAttack - ������ ���� �������̽�, IHitDamage - ���ظ� �޴� �������̽�
    // IDeath - ĳ���� ���ó���� ���� �������̽�, IMoneyDrop - ������ ����� ���� �������̽�
    // ITargetLost - Ÿ���� ������� ��츦 ���� �������̽�
    public MonsterType monsterType = MonsterType.Slime;

    public Collider monster_Coll = null;
    float[] gold_Point = new float[18];
    int dia_Point = 1;

    void Awake()
    {
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        att_Point[0] = 2;
        hp_Max[0] = 100;
        gold_Point[0] = 100;
        serch_Size = 4.0f;
        cha_Runtime = cha_Anim.runtimeAnimatorController;
    }

    void OnEnable()
    {
        chaMode = ChaMode.Idle;
        navMeshAgent.radius = 0.5f;
        MonsterSetting();

        Vector3 pos = RandomPosition(5.0f, 30.0f, Monster_Pool.Inst.transform);
        this.transform.position = pos;
        monster_Coll.enabled = true;
    }

    void FixedUpdate()
    {
        switch (chaMode)
        {
            case ChaMode.Idle:
                Serch();
                break;
            case ChaMode.Attack:
                Attack();
                break;
        }
    }

    public void MonsterSetting()
    {
        // ������ ������ �����ϴ� �κ� ���� �迭, �⺻ ����, ���� ����, ���� ������ ������ �Ű������� ���ݷ��� ����
        att_Point = StateSetting(att_Point, 2, 1.6f, GlobalData.user_BossNowLv);
        hp_Max = StateSetting(hp_Max, 100, 2, GlobalData.user_BossNowLv);
        gold_Point = StateSetting(gold_Point, (100 * GlobalData.gold_Value), 2, GlobalData.user_BossNowLv);
        hp_Now = (float[])hp_Max.Clone();
        //for (int ii = 0; ii < hp_Max.Length; ii++)  // ������ �ִ� ü�°� ���� ü���� ���� ������ ����
        //    this.hp_Now[ii] = hp_Max[ii];   // �迭�� �״�� �����ϸ� ���� �迭�� �ν������� �ε��� �ϳ����� ����

        cha_Ui.HpUpdate(hp_Max, hp_Now);    // ü�¹� �̹��� ���� ����
    }

    public float[] StateSetting(float[] state_Array, float value, float state_Value, int state_Lv)
    {
        state_Array = new float[18];
        state_Array[0] = value;
        state_Array = GlobalData.Theorem(state_Array);

        for (int ii = 0; ii < state_Lv; ii++)
            for (int kk = state_Array.Length - 1; kk >= 0; kk--)
            {
                if (state_Array[kk] <= 0)
                    continue;

                state_Array[kk] = state_Array[kk] * state_Value;
                state_Array = GlobalData.Theorem(state_Array);
            }

        return state_Array;
    }

    Vector3 RandomPosition(float min_Pos, float max_Pos, Transform center_Tr)
    {
        Vector3 rand_Pos = Random.insideUnitCircle.normalized;
        rand_Pos.z = rand_Pos.y;
        rand_Pos.y = 0.0f;
        float radius = Random.Range(min_Pos, max_Pos);

        return (rand_Pos * radius) + center_Tr.position;
    }

    public void Serch()
    {
        if (attack_Co != null)
            return;

        string tag_Str = "Player";

        float dis = 0.0f;
        target_Coll = Physics.OverlapSphere(this.transform.position, this.serch_Size);
        target_List.Clear();

        foreach (Collider coll in target_Coll)
        {
            if (coll.tag.Contains(tag_Str) != true)
                continue;

            dis = Vector3.Distance(this.transform.position, coll.transform.position);
            target_List.Add(coll.gameObject, dis);
        }

        if (target_List.Count <= 0)
        {
            TargetLost();
            return;
        }

        float values = target_List.Values.Min();
        GameObject obj = target_List.FirstOrDefault(x => x.Value == values).Key; 
        target_Tr = obj.transform;

        if (target_Tr != null)
            Move();
    }

    public void Move()
    {
        float dis = Vector3.Distance(this.transform.position, target_Tr.position);

        if (dis > 2.0f)
        {
            navMeshAgent.SetDestination(target_Tr.position);
            //this.transform.position =
            //    Vector3.MoveTowards(this.transform.position, target_Tr.position, this.move_Speed);

            cha_Anim.SetBool("serch", true);
            this.cha_Model.transform.LookAt(target_Tr);
        }
        else
        {
            navMeshAgent.ResetPath();
            cha_Anim.SetBool("serch", false);
            chaMode = ChaMode.Attack;
        }
    }

    IEnumerator attack_Co = null;

    public void Attack()
    {
        if (attack_Co != null)
            return;

        float dis = Vector3.Distance(this.transform.position, target_Tr.position);

        if (dis > 2.0f)
        {
            chaMode = ChaMode.Idle;
            return;
        }

        Charactor target_Cha = target_Tr.GetComponent<Charactor>();

        if (target_Cha == null || target_Cha.chaMode == ChaMode.Death)
            return;

        float time = 0.0f;

        for (int ii = 0; ii < cha_Runtime.animationClips.Length; ii++)
            if (cha_Runtime.animationClips[ii].name.Contains("Attack01"))
            {
                time = cha_Runtime.animationClips[ii].length;
            }

        cha_Anim.SetTrigger("attack");
        this.cha_Model.transform.LookAt(target_Tr);

        attack_Co = AttackCo(time);
        StartCoroutine(attack_Co);
    }

    IEnumerator AttackCo(float time)
    {
        yield return new WaitForSeconds(time / 2);
        HitAttack();
        yield return new WaitForSeconds(time / 2);
        attack_Co = null;
    }

    void HitAttack()    // ������ �´� Ÿ�ֿ̹� ȣ��Ǵ� �Լ�
    {
        if (target_Tr == null)      // Ÿ���� ���ٸ� �Լ��� ���� ����
            return;

        IHitDamage target_Hit = target_Tr.GetComponent<IHitDamage>();       // Ÿ���� IHitDamage �������̽� ����

        if (target_Hit == null)     // ������ �������̽��� ���ٸ� �Լ��� ���� ����
            return;

        bool target_Bool = false;   // Ÿ���� ��� ���θ� Ȯ���� ����
        target_Bool = target_Hit.HitDamage(att_Point, new Color(255, 0, 0));
        // ���ݷ� ��ġ�� �⺻ ����(������)�� �Ű������� HitDamage �Լ� ȣ��

        if (target_Bool == true)    // Ÿ���� ������� ���
        {
            chaMode = ChaMode.Idle;     // ���� ����� ã�� ���� ��ȯ
            target_Tr = null;       // Ÿ���� ���
        }

        Sound_Ctrl.Inst.SfSoundPlay("Hit", "Monster");      // �ǰ� ���� ȣ��
    }

    public bool HitDamage(float[] damage_Point, Color a_Color)      // ������ �޾��� �� ȣ��Ǵ� �Լ�
    {
        for (int ii = damage_Point.Length - 1; ii >= 0; ii--)       // ���ݷ� �迭�� ���� ������ �ε��� ���� �ݺ��ؼ� �����´�
        {
            hp_Now[ii] -= damage_Point[ii];     // ���� ���� �ڸ����� ���� ������ ���
            GlobalData.Theorem(hp_Now);     // �迭 ������ ����
        }   // ���� ���� ������ ����� ��� ���� �������� �� �ڸ����� ���� ������� ä��� ������ �ѹ� �� üũ�� ����� �Ѵ�

        cha_Ui.DamageText(damage_Point, a_Color);       // ������� ��ġ�� �ؽ�Ʈ�� ǥ���ϴ� �Լ� �÷��� ��� ġ��Ÿ�� �߻��ϸ� ���� �ٲپ���
        bool hp_Bool = cha_Ui.HpUpdate(hp_Max, hp_Now);     // ü�¹��� �̹��� ���� ������Ʈ �Լ� ������ 0�� ��� true�� ������ �ش�
        Sound_Ctrl.Inst.SfSoundPlay("SwordHit", "Player");      // ���� ȣ�� �Լ�

        if (hp_Bool == true)    // ü���� ������ 0�� ���
            Death();        // ��� �Լ� ȣ��

        return hp_Bool;     // ������ �� ��뿡�� ������θ� ��ȯ����
    }

    public void Death()
    {
        if(attack_Co != null)
        {
            StopCoroutine(attack_Co);
            attack_Co = null;
        }

        chaMode = ChaMode.Death;
        cha_Anim.SetTrigger("death");
        this.target_Tr = null;

        monster_Coll.enabled = false;
        //navMeshAgent.radius = 0.0f;

        Sound_Ctrl.Inst.SfSoundPlay("Drop", "Ui");

        MoneyDrop();
        StartCoroutine(DeathCo());
    }

    public void MoneyDrop()
    {
        MainUI_Ctrl.Inst.GetGold(gold_Point, GlobalData.user_Gold);
        MainUI_Ctrl.Inst.GetDiamond(dia_Point);
    }

    WaitForSeconds death_Time = new WaitForSeconds(1.0f);

    IEnumerator DeathCo()
    {
        yield return death_Time;

        this.gameObject.SetActive(false);
    }

    public void TargetLost()
    {
        if (target_Tr == null)
            return;

        chaMode = ChaMode.Idle;

        target_Tr = null;
        cha_Anim.SetBool("serch", false);
        this.cha_Model.transform.rotation = Quaternion.identity;
        navMeshAgent.ResetPath();
        hp_Now = (float[])hp_Max.Clone();

        cha_Ui.HpUpdate(hp_Max, hp_Now);
    }

    void OnDisable()
    {
        if (Monster_Pool.Inst == null)
            return;

        cha_Ui.TextActive();

        Monster_Pool.Inst.monster_Count--;
        Monster_Pool.Inst.monster_Pool.Enqueue(this.gameObject);
    }
}