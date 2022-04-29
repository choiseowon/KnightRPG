using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Player_Ctrl : Charactor, ISerch, IMove, IAttack, IHitDamage, IDeath, IRecover
{
    // Charactor - ĳ���͵��� �����ϰ� �ʿ��� ������ ��Ƴ��� Ŭ����
    // ISerch - ���� Ÿ���� ã�� �������̽� , IMove �̵��� ���� �������̽�
    // IAttack - ������ ���� �������̽�, IHitDamage - ���ظ� �޴� �������̽�
    // IDeath - ĳ���� ���ó���� ���� �������̽�, IRecover - ĳ���� ��Ȱ�� ���� �������̽�
    public static Player_Ctrl Inst = null;
    float move_Speed = 2.5f;

    public Transform boss_Pos = null;
    public Transform main_Pos = null;
    public GameObject camera_Obj = null;
    bool boss_Challenge = false;

    Vector3 save_Pos = Vector3.zero;

    void Awake()
    {
        Inst = this;
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        serch_Size = 100.0f;
        cha_Runtime = cha_Anim.runtimeAnimatorController;
    }

    void Start()
    {
        PlayerSetting();

        for (int ii = 0; ii < hp_Max.Length; ii++)
            this.hp_Now[ii] = hp_Max[ii];

        cha_Ui.HpUpdate(hp_Max, hp_Now);
    }

    void FixedUpdate()
    {
        switch(chaMode)
        {
            case ChaMode.Idle:
                Serch();
                break;
            case ChaMode.Attack:
                Attack();
                break;
        }
    }

    public void BossChallenge()
    {
        if (boss_Challenge == true)
            return;

        if (chaMode == ChaMode.Death)
            return;

        navMeshAgent.enabled = false;
        this.transform.position = boss_Pos.position;
        navMeshAgent.enabled = true;
        camera_Obj.transform.position = this.transform.position;
        boss_Challenge = true;

        MainUI_Ctrl.Inst.bossTime_Root.SetActive(true);
        Sound_Ctrl.Inst.BgmSoundPlay("BossBGM");
    }

    public void BossBattleEnd()
    {
        navMeshAgent.enabled = false;
        this.transform.position = main_Pos.position;
        navMeshAgent.enabled = true;
        camera_Obj.transform.position = this.transform.position;
        boss_Challenge = false;

        MainUI_Ctrl.Inst.bossTime_Root.SetActive(false);
        Sound_Ctrl.Inst.BgmSoundPlay("MainBGM");
    }

    public void PlayerSetting()
    {
        att_Point = StateSetting(att_Point, 10, GlobalData.att_Value, GlobalData.user_APLv);
        hp_Max = StateSetting(hp_Max, 100, GlobalData.hp_Value, GlobalData.user_HPLv);

        critical_Rand = GlobalData.user_CriRandLv;
        critical_Power = ((GlobalData.user_CriPowLv * 0.001f) + 2.0f) * GlobalData.cri_Value;
        critical_Point = StateSetting(critical_Point, (10 * critical_Power), GlobalData.att_Value, GlobalData.user_APLv);

        float speed = 1.0f * GlobalData.speed_Value;
        cha_Anim.SetFloat("attSpeed", speed);

        move_Speed = 2.5f * GlobalData.move_Value;
        navMeshAgent.speed = move_Speed;
        cha_Anim.SetFloat("moveSpeed", move_Speed);

        cha_Ui.HpUpdate(hp_Max, hp_Now);
    }

    public float[] StateSetting(float[] state_Array, float value, 
                                    float state_Value, int state_Lv)    // �ɷ�ġ�� �������ִ� �Լ�
    {
        state_Array = new float[18];        // �ɷ�ġ�� ������ �迭 �ʱ�ȭ
        state_Array[0] = value * state_Value;       // �迭�� ù��° �ε����� ���� ����
        state_Array = GlobalData.Theorem(state_Array);      // �迭 ������ ����

        for (int ii = 0; ii < state_Lv; ii++)       // �ɷ�ġ�� ���� ��ŭ �ݺ�
        {
            state_Array[0] += value * state_Value;      // ù��° �ε����� �߰��Ǵ� �ɷ�ġ ���� ��� ������
            state_Array = GlobalData.Theorem(state_Array);      // �迭�� �����ϴ� �ɷ� �ڸ����� �ʰ��Ǹ� ���� �ε��� ���� �÷��ش�
        }

        return state_Array;     // ����� �Ϸ�� �迭�� ��ȯ
    }

    public void Serch()     // Ÿ���� ã�� �Լ�
    {
        if (attack_Co != null)      // ������ �ϰ� ������ �Լ��� ��������
            return;

        string tag_Str = "Monster";     // ���� ����� ���ͷ� ����

        float dis = 0.0f;       // �Ÿ� ���� ������ ����
        target_Coll = Physics.OverlapSphere(this.transform.position, this.serch_Size);      // Ư�� ��ǥ���� ������ ũ�⸦ ���� ���� �ݶ��̴��� �浹�Ǵ� �ݶ��̴��� �迭�� ����
        target_List.Clear();    // ��ųʸ� ���� ���

        foreach (Collider coll in target_Coll)      // �浹�� �ݶ��̴��� ��ŭ �ݺ�
        {
            if (coll.tag.Contains(tag_Str) != true)     // �ݶ��̴��� �±װ� ������� ���ѰͰ� ������ üũ
                continue;   // ���� �ٸ��ٸ� ���� �ݶ��̴��� �Ѿ

            dis = Vector3.Distance(this.transform.position, coll.transform.position);   // �ش� ��ǥ�� �ݶ��̴��� �Ÿ����� float�� ����
            target_List.Add(coll.gameObject, dis);      // �ݶ��̴��� ������Ʈ�� Ű��, �Ÿ��� ����� ��ųʸ��� �߰�
        }

        if (target_List.Count <= 0)     // ��ųʸ��� �߰��� ���� 0�̸� �Լ��� ��������
            return;

        float values = target_List.Values.Min();    // ��ųʸ��� ����� �߿��� ���� ���� ���� ����
        GameObject obj = target_List.FirstOrDefault(x => x.Value == values).Key;    // �ش� ������� ���� Ű���� ������Ʈ ������ ����
        target_Tr = obj.transform;      // �ش� ������Ʈ�� Ÿ������ ����

        if (target_Tr != null)      // Ÿ���� �ִ��� üũ
            Move();     // Ÿ���� ���� �̵��ϴ� �Լ� ȣ��
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

    IEnumerator attack_Co = null;   // �ڷ�ƾ�� ������ ��ü ����

    public void Attack()    // ������ ���� �Լ�
    {
        if (attack_Co != null)      // ���� �����̰� ������ �Լ��� ��������
            return;

        float dis = Vector3.Distance(this.transform.position, target_Tr.position);      // ���� ������ �Ÿ� üũ

        if (dis > 2.0f)     // ���� ������ �Ÿ��� �����Ÿ� �̻��� �Ǹ� ����
        {
            chaMode = ChaMode.Idle;     // �̵��� ���� ���� ���� �� �Լ��� ��������
            return;
        }

        Charactor target_Cha = target_Tr.GetComponent<Charactor>();     // ���� Ÿ���� Charactor Ŭ������ �޾ƿ�

        if (target_Cha == null || target_Cha.chaMode == ChaMode.Death)  // ���� Ÿ���� ��� �����̸� �Լ��� ��������
            return;

        float time = 0.0f;      // ���� �ִϸ��̼��� �ð��� ������ ����

        for (int ii = 0; ii < cha_Runtime.animationClips.Length; ii++)      // ĳ������ �ִϸ��̼��� Ŭ���� ��ŭ �ݺ�
            if (cha_Runtime.animationClips[ii].name.Contains("NormalAttack01_SwordShield"))     // �ش� �ִϸ��̼��� �̸��� �� ���ڿ� ��
            {
                time = cha_Runtime.animationClips[ii].length;   // �ش� �ִϸ��̼��� �ð� ���̸� ����
            }

        time = time / cha_Anim.GetFloat("attSpeed");    // �ִϸ��̼��� �ð��� �ִϸ��̼� �ӵ��� ������ ����

        cha_Anim.SetTrigger("attack");      // ���� �ִϸ��̼� ���
        this.cha_Model.transform.LookAt(target_Tr);     // ĳ���� ������Ʈ�� ���ݴ���� �ٶ󺸵��� ����

        attack_Co = AttackCo(time);     // �ִϸ��̼� �ð� ���� �Ű������� �ϴ� �ڷ�ƾ ����
        StartCoroutine(attack_Co);      // ����� �ڷ�ƾ ȣ�� / ���߿� �ڷ�ƾ�� ���߱����� ��ü�� �����Ͽ� �����صд�


        Sound_Ctrl.Inst.SfSoundPlay("SwordSwing", "Player");    // ���� ȣ��
    }

    IEnumerator AttackCo(float time)
    {
        yield return new WaitForSeconds(time * 0.5f);
        HitAttack();
        yield return new WaitForSeconds(time * 0.5f);
        attack_Co = null;
    }

    void HitAttack()    // ������ �´� Ÿ�ֿ̹� ȣ��Ǵ� �Լ�
    {
        if (target_Tr == null)      // Ÿ���� ���ٸ� �Լ��� ���� ����
            return;

        IHitDamage target_Hit = target_Tr.GetComponent<IHitDamage>();       // Ÿ���� IHitDamage �������̽� ����

        if (target_Hit == null)     // ������ �������̽��� ���ٸ� �Լ��� ���� ����
            return;

        int rand = Random.Range(0, 1000);       // ġ��Ÿ Ȯ���� ���� ���� �� ����
        bool target_Bool = false;   // Ÿ���� ��� ���θ� Ȯ���� ����

        if (rand < critical_Rand)       // ġ��Ÿ�� �߻����� ���
            target_Bool = target_Hit.HitDamage(critical_Point, new Color(255, 255, 0));     
            // ġ��Ÿ ��ġ�� ġ��Ÿ ǥ�ø� ���� ����(�����) �� �Ű������� HitDamage �Լ� ȣ��
        else
            target_Bool = target_Hit.HitDamage(att_Point, new Color(255, 255, 255));
            // ���ݷ� ��ġ�� �⺻ ����(������)�� �Ű������� HitDamage �Լ� ȣ��

        if (target_Bool == true)    // Ÿ���� ������� ���
        {
            chaMode = ChaMode.Idle;     // ���� ����� ã�� ���� ��ȯ
            target_Tr = null;       // Ÿ���� ���
        }
    }

    public bool HitDamage(float[] damage_Point, Color a_Color)      // ������ �޾��� �� ȣ��Ǵ� �Լ�
    {
        for (int ii = damage_Point.Length - 1; ii >= 0; ii--)       // ���ݷ� �迭�� ���� ������ �ε��� ���� �ݺ��ؼ� �����´�
        {
            hp_Now[ii] -= damage_Point[ii];     // ���� ���� �ڸ����� ���� ������ ���
            GlobalData.Theorem(hp_Now);     // �迭 ������ ����
        }   // ���� ���� ������ ����� ��� ���� �������� �� �ڸ����� ���� ������� ä��� ������ �ѹ� �� üũ�� ����� �Ѵ�

        bool hp_Bool = cha_Ui.HpUpdate(hp_Max, hp_Now);       // ������� ��ġ�� �ؽ�Ʈ�� ǥ���ϴ� �Լ� �÷��� ��� ġ��Ÿ�� �߻��ϸ� ���� �ٲپ���

        if (hp_Bool == true)    // ü���� ������ 0�� ���
            Death();        // ��� �Լ� ȣ��

        return hp_Bool;     // ������ �� ��뿡�� ������θ� ��ȯ����
    }

    IEnumerator recover_Co = null;

    public void Death()
    {
        if (recover_Co == null)
        {
            if (attack_Co != null)
            {
                StopCoroutine(attack_Co);
                attack_Co = null;
            }

            navMeshAgent.ResetPath();
            chaMode = ChaMode.Death;
            cha_Anim.SetTrigger("death");
            target_Tr = null;

            if (boss_Challenge == true)
                BossBattleEnd();

            Sound_Ctrl.Inst.SfSoundPlay("Death", "Player");
            MainUI_Ctrl.Inst.recover_Txt.gameObject.SetActive(true);
            Recover();
        }
    }

    public void Recover()
    {
        recover_Co = Recover_Co();
        StartCoroutine(recover_Co);
    }

    WaitForSeconds recover_Time = new WaitForSeconds(1.0f);

    IEnumerator Recover_Co()
    {
        for (int ii = 5; ii > 0; ii--)
        {
            MainUI_Ctrl.Inst.recover_Txt.text = ii + "�� �� ��Ȱ�մϴ�";
            yield return recover_Time;
        }

        cha_Anim.SetTrigger("recover");
        MainUI_Ctrl.Inst.recover_Txt.gameObject.SetActive(false);

        yield return recover_Time;
        yield return recover_Time;

        for (int ii = 0; ii < this.hp_Max.Length; ii++)
            this.hp_Now[ii] = this.hp_Max[ii];

        cha_Anim.ResetTrigger("attack");
        cha_Anim.ResetTrigger("death");

        cha_Ui.HpUpdate(hp_Max, hp_Now);
        chaMode = ChaMode.Idle;
        recover_Co = null;
    }

    public string StateGet(string a_Str)
    {
        string m_Str = "";

        switch(a_Str)
        {
            case "Attack":
                {
                    m_Str = GlobalData.MymoneyToString(att_Point);
                }
                break;
            case "Hp":
                {
                    m_Str = GlobalData.MymoneyToString(hp_Max);
                }
                break;
            case "CriPow":
                {
                    m_Str = GlobalData.MymoneyToString(critical_Point);
                }
                break;
        }

        return m_Str;
    }
}
