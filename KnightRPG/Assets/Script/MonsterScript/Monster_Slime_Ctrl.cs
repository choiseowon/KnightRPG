using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Monster_Slime_Ctrl : Charactor, IMonster, ISerch, IMove, IAttack, IHitDamage, IDeath, IMoneyDrop, ITargetLost
{
    // Charactor - 캐릭터들의 동일하게 필요한 변수들 모아놓은 클래스
    // IMoster - 몬스터 능력치를 셋팅을 위한 인터페이스
    // ISerch - 공격 타겟을 찾는 인터페이스 , IMove 이동을 위한 인터페이스
    // IAttack - 공격을 위한 인터페이스, IHitDamage - 피해를 받는 인터페이스
    // IDeath - 캐릭터 사망처리를 위한 인터페이스, IMoneyDrop - 아이템 드랍을 위한 인터페이스
    // ITargetLost - 타겟이 사라졌을 경우를 위한 인터페이스
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
        // 몬스터의 스탯을 설정하는 부분 스탯 배열, 기본 스탯, 증가 비율, 현재 설정된 레벨을 매개변수로 공격력을 설정
        att_Point = StateSetting(att_Point, 2, 1.6f, GlobalData.user_BossNowLv);
        hp_Max = StateSetting(hp_Max, 100, 2, GlobalData.user_BossNowLv);
        gold_Point = StateSetting(gold_Point, (100 * GlobalData.gold_Value), 2, GlobalData.user_BossNowLv);
        hp_Now = (float[])hp_Max.Clone();
        //for (int ii = 0; ii < hp_Max.Length; ii++)  // 설정된 최대 체력과 현재 체력을 같은 값으로 설정
        //    this.hp_Now[ii] = hp_Max[ii];   // 배열을 그대로 대입하면 같은 배열로 인식함으로 인덱스 하나마다 대입

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

    void HitAttack()    // 공격을 맞는 타이밍에 호출되는 함수
    {
        if (target_Tr == null)      // 타겟이 없다면 함수를 빠져 나감
            return;

        IHitDamage target_Hit = target_Tr.GetComponent<IHitDamage>();       // 타겟의 IHitDamage 인터페이스 저장

        if (target_Hit == null)     // 저장한 인터페이스가 없다면 함수를 빠져 나감
            return;

        bool target_Bool = false;   // 타겟의 사망 여부를 확인할 변수
        target_Bool = target_Hit.HitDamage(att_Point, new Color(255, 0, 0));
        // 공격력 수치와 기본 색상(빨간색)을 매개변수로 HitDamage 함수 호출

        if (target_Bool == true)    // 타겟이 사망했을 경우
        {
            chaMode = ChaMode.Idle;     // 공격 대상을 찾는 모드로 전환
            target_Tr = null;       // 타겟을 비움
        }

        Sound_Ctrl.Inst.SfSoundPlay("Hit", "Monster");      // 피격 사운드 호출
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