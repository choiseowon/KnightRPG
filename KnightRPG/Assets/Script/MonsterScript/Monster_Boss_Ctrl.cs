using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Monster_Boss_Ctrl : Charactor, IMonster, ISerch, IMove, IAttack, IHitDamage, IDeath, IMoneyDrop, ITargetLost
{
    // Charactor - 캐릭터들의 동일하게 필요한 변수들 모아놓은 클래스
    // IMoster - 몬스터 능력치를 셋팅을 위한 인터페이스
    // ISerch - 공격 타겟을 찾는 인터페이스 , IMove 이동을 위한 인터페이스
    // IAttack - 공격을 위한 인터페이스, IHitDamage - 피해를 받는 인터페이스
    // IDeath - 캐릭터 사망처리를 위한 인터페이스, IMoneyDrop - 아이템 드랍을 위한 인터페이스
    // ITargetLost - 타겟이 사라졌을 경우를 위한 인터페이스
    public MonsterType monsterType = MonsterType.Boss;

    public Collider monster_Coll = null;
    float[] gold_Point = new float[18];
    int dia_Point = 20;

    void Awake()
    {
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        att_Point[0] = 10;
        hp_Max[0] = 1000;
        gold_Point[0] = 2000;
        serch_Size = 8.0f;
        cha_Runtime = cha_Anim.runtimeAnimatorController;
    }

    void OnEnable()
    {
        chaMode = ChaMode.Idle;
        MonsterSetting();
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
        // 몬스터의 스탯을 설정하는 부분 스탯 배열, 기본 스탯, 증가 비율, 최고 레벨을 매개변수로 공격력을 설정
        att_Point = StateSetting(att_Point, 10, 1.6f, GlobalData.user_BossMaxLv);
        hp_Max = StateSetting(hp_Max, 1000, 2, GlobalData.user_BossMaxLv);
        gold_Point = StateSetting(gold_Point, (2000 * GlobalData.gold_Value), 2, GlobalData.user_BossMaxLv);
        hp_Now = (float[])hp_Max.Clone();
        //for (int ii = 0; ii < hp_Max.Length; ii++) // 설정된 최대 체력과 현재 체력을 같은 값으로 설정
            //this.hp_Now[ii] = hp_Max[ii];   // 배열을 그대로 대입하면 같은 배열로 인식함으로 인덱스 하나마다 대입

        cha_Ui.HpUpdate(hp_Max, hp_Now);    // 체력바 이미지 비율 설정
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

        if (dis > 3.0f)
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

        if (dis > 3.0f)
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
        yield return new WaitForSeconds(time * 0.4f);
        HitAttack();
        yield return new WaitForSeconds(time * 0.6f);
        attack_Co = null;
    }

    void HitAttack()
    {
        if (target_Tr == null)
            return;

        IHitDamage target_Hit = target_Tr.GetComponent<IHitDamage>();
        bool target_Bool = false;

        if (target_Hit == null)
            return;

        target_Bool = target_Hit.HitDamage(att_Point, new Color(255, 0, 0));

        if (target_Bool == true)
        {
            TargetLost();
        }

        Sound_Ctrl.Inst.SfSoundPlay("Hit", "Monster");
    }

    public bool HitDamage(float[] damage_Point, Color a_Color)      // 공격을 받았을 때 호출되는 함수
    {
        for (int ii = damage_Point.Length - 1; ii >= 0; ii--)       // 공격력 배열을 가장 마지막 인덱스 부터 반복해서 내려온다
        {
            hp_Now[ii] -= damage_Point[ii];     // 서로 같은 자리수이 값만 빼도록 계산
            GlobalData.Theorem(hp_Now);     // 배열 정리용 변수
        }   // 가장 낮은 수부터 계산할 경우 정리 과정에서 앞 자리수의 값을 가지고와 채우기 때문에 한번 더 체크를 해줘야 한다

        cha_Ui.DamageText(damage_Point, a_Color);       // 대미지의 수치를 텍스트로 표시하는 함수 컬러의 경우 치명타가 발생하면 색을 바꾸어줌
        bool hp_Bool = cha_Ui.HpUpdate(hp_Max, hp_Now);     // 체력바의 이미지 비율 업데이트 함수 비율이 0일 경우 true를 리턴해 준다
        Sound_Ctrl.Inst.SfSoundPlay("SwordHit", "Player");      // 사운드 호출 함수

        if (hp_Bool == true)    // 체력의 비율이 0일 경우
            Death();        // 사망 함수 호출

        return hp_Bool;     // 공격을 한 상대에게 사망여부를 반환해줌
    }

    public void Death()
    {
        if (attack_Co != null)
        {
            StopCoroutine(attack_Co);
            attack_Co = null;
        }

        chaMode = ChaMode.Death;
        cha_Anim.SetTrigger("death");
        this.target_Tr = null;

        GlobalData.user_BossMaxLv++;
        MainUI_Ctrl.Inst.ClearFunc();
        Player_Ctrl.Inst.BossBattleEnd();
        Monster_Pool.Inst.MonStateUpdate();

        MoneyDrop();
        StartCoroutine(GlobalData.SaveDataCo(GlobalData.user_Number));
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
        this.transform.position = this.transform.parent.position;
        navMeshAgent.ResetPath();
        hp_Now = (float[])hp_Max.Clone();
        //for (int ii = 0; ii < hp_Max.Length; ii++)
        //    hp_Now[ii] = hp_Max[ii];

        cha_Ui.HpUpdate(hp_Max, hp_Now);
    }

    void OnDisable()
    {
        if (Monster_Pool.Inst == null)
            return;

        cha_Ui.TextActive();
        Invoke("BossReset", 1.0f);
    }

    void BossReset()
    {
        this.gameObject.SetActive(true);
        this.transform.position = this.transform.parent.position;
    }
}
