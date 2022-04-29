using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Player_Ctrl : Charactor, ISerch, IMove, IAttack, IHitDamage, IDeath, IRecover
{
    // Charactor - 캐릭터들의 동일하게 필요한 변수들 모아놓은 클래스
    // ISerch - 공격 타겟을 찾는 인터페이스 , IMove 이동을 위한 인터페이스
    // IAttack - 공격을 위한 인터페이스, IHitDamage - 피해를 받는 인터페이스
    // IDeath - 캐릭터 사망처리를 위한 인터페이스, IRecover - 캐릭터 부활을 위한 인터페이스
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
                                    float state_Value, int state_Lv)    // 능력치를 설정해주는 함수
    {
        state_Array = new float[18];        // 능력치를 저장할 배열 초기화
        state_Array[0] = value * state_Value;       // 배열의 첫번째 인덱스의 값을 설정
        state_Array = GlobalData.Theorem(state_Array);      // 배열 정리용 변수

        for (int ii = 0; ii < state_Lv; ii++)       // 능력치의 레벨 만큼 반복
        {
            state_Array[0] += value * state_Value;      // 첫번째 인덱스에 추가되는 능력치 값을 계속 더해줌
            state_Array = GlobalData.Theorem(state_Array);      // 배열을 정리하는 걸로 자리수가 초과되면 다음 인덱스 값을 올려준다
        }

        return state_Array;     // 계산이 완료된 배열을 반환
    }

    public void Serch()     // 타겟을 찾는 함수
    {
        if (attack_Co != null)      // 공격을 하고 있으면 함수를 빠져나감
            return;

        string tag_Str = "Monster";     // 공격 대상은 몬스터로 지정

        float dis = 0.0f;       // 거리 값을 저장한 변수
        target_Coll = Physics.OverlapSphere(this.transform.position, this.serch_Size);      // 특정 좌표에서 일정한 크기를 가진 구형 콜라이더랑 충돌되는 콜라이더를 배열로 저장
        target_List.Clear();    // 딕셔너리 값을 비움

        foreach (Collider coll in target_Coll)      // 충돌된 콜라이더들 만큼 반복
        {
            if (coll.tag.Contains(tag_Str) != true)     // 콜라이더의 태그가 대상으로 정한것과 같은지 체크
                continue;   // 대상과 다르다면 다음 콜라이더로 넘어감

            dis = Vector3.Distance(this.transform.position, coll.transform.position);   // 해당 좌표와 콜라이더의 거리값을 float로 저장
            target_List.Add(coll.gameObject, dis);      // 콜라이더의 오브젝트를 키값, 거리를 밸류로 딕셔너리에 추가
        }

        if (target_List.Count <= 0)     // 딕셔너리에 추가된 값이 0이면 함수를 빠져나감
            return;

        float values = target_List.Values.Min();    // 딕셔너리의 밸류값 중에서 가장 낮은 값을 구함
        GameObject obj = target_List.FirstOrDefault(x => x.Value == values).Key;    // 해당 밸류값과 같은 키값을 오브젝트 변수로 저장
        target_Tr = obj.transform;      // 해당 오브젝트를 타겟으로 설정

        if (target_Tr != null)      // 타겟이 있는지 체크
            Move();     // 타겟을 향해 이동하는 함수 호출
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

    IEnumerator attack_Co = null;   // 코루틴을 저장할 객체 선언

    public void Attack()    // 공격을 위한 함수
    {
        if (attack_Co != null)      // 공격 딜레이가 있으면 함수를 빠져나감
            return;

        float dis = Vector3.Distance(this.transform.position, target_Tr.position);      // 공격 대상과의 거리 체크

        if (dis > 2.0f)     // 공격 대상과의 거리가 일정거리 이상이 되면 실행
        {
            chaMode = ChaMode.Idle;     // 이동을 위한 모드로 변경 후 함수를 빠져나감
            return;
        }

        Charactor target_Cha = target_Tr.GetComponent<Charactor>();     // 공격 타겟의 Charactor 클래스를 받아옴

        if (target_Cha == null || target_Cha.chaMode == ChaMode.Death)  // 공격 타겟이 사망 상태이면 함수를 빠져나감
            return;

        float time = 0.0f;      // 공격 애니메이션의 시간을 저장할 변수

        for (int ii = 0; ii < cha_Runtime.animationClips.Length; ii++)      // 캐릭터의 애니메이션의 클립들 만큼 반복
            if (cha_Runtime.animationClips[ii].name.Contains("NormalAttack01_SwordShield"))     // 해당 애니메이션의 이름에 들어간 문자열 비교
            {
                time = cha_Runtime.animationClips[ii].length;   // 해당 애니메이션의 시간 길이를 저장
            }

        time = time / cha_Anim.GetFloat("attSpeed");    // 애니메이션의 시간을 애니메이션 속도로 나누어 저장

        cha_Anim.SetTrigger("attack");      // 공격 애니메이션 재생
        this.cha_Model.transform.LookAt(target_Tr);     // 캐릭터 오브젝트가 공격대상을 바라보도록 설정

        attack_Co = AttackCo(time);     // 애니메이션 시간 값을 매개변수로 하는 코루틴 저장
        StartCoroutine(attack_Co);      // 저장된 코루틴 호출 / 나중에 코루틴을 멈추기위해 객체러 생성하여 저장해둔다


        Sound_Ctrl.Inst.SfSoundPlay("SwordSwing", "Player");    // 사운드 호출
    }

    IEnumerator AttackCo(float time)
    {
        yield return new WaitForSeconds(time * 0.5f);
        HitAttack();
        yield return new WaitForSeconds(time * 0.5f);
        attack_Co = null;
    }

    void HitAttack()    // 공격을 맞는 타이밍에 호출되는 함수
    {
        if (target_Tr == null)      // 타겟이 없다면 함수를 빠져 나감
            return;

        IHitDamage target_Hit = target_Tr.GetComponent<IHitDamage>();       // 타겟의 IHitDamage 인터페이스 저장

        if (target_Hit == null)     // 저장한 인터페이스가 없다면 함수를 빠져 나감
            return;

        int rand = Random.Range(0, 1000);       // 치명타 확률을 위한 랜덤 값 저장
        bool target_Bool = false;   // 타겟의 사망 여부를 확인할 변수

        if (rand < critical_Rand)       // 치명타가 발생했을 경우
            target_Bool = target_Hit.HitDamage(critical_Point, new Color(255, 255, 0));     
            // 치명타 수치와 치명타 표시를 위한 색상(노란색) 을 매개변수로 HitDamage 함수 호출
        else
            target_Bool = target_Hit.HitDamage(att_Point, new Color(255, 255, 255));
            // 공격력 수치와 기본 색상(빨간색)을 매개변수로 HitDamage 함수 호출

        if (target_Bool == true)    // 타겟이 사망했을 경우
        {
            chaMode = ChaMode.Idle;     // 공격 대상을 찾는 모드로 전환
            target_Tr = null;       // 타겟을 비움
        }
    }

    public bool HitDamage(float[] damage_Point, Color a_Color)      // 공격을 받았을 때 호출되는 함수
    {
        for (int ii = damage_Point.Length - 1; ii >= 0; ii--)       // 공격력 배열을 가장 마지막 인덱스 부터 반복해서 내려온다
        {
            hp_Now[ii] -= damage_Point[ii];     // 서로 같은 자리수이 값만 빼도록 계산
            GlobalData.Theorem(hp_Now);     // 배열 정리용 변수
        }   // 가장 낮은 수부터 계산할 경우 정리 과정에서 앞 자리수의 값을 가지고와 채우기 때문에 한번 더 체크를 해줘야 한다

        bool hp_Bool = cha_Ui.HpUpdate(hp_Max, hp_Now);       // 대미지의 수치를 텍스트로 표시하는 함수 컬러의 경우 치명타가 발생하면 색을 바꾸어줌

        if (hp_Bool == true)    // 체력의 비율이 0일 경우
            Death();        // 사망 함수 호출

        return hp_Bool;     // 공격을 한 상대에게 사망여부를 반환해줌
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
            MainUI_Ctrl.Inst.recover_Txt.text = ii + "초 후 부활합니다";
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
