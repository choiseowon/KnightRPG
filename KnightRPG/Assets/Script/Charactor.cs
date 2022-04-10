using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Charactor : MonoBehaviour
{
    public ChaMode chaMode = ChaMode.Idle;

    protected float[] hp_Now = new float[18];
    protected float[] hp_Max = new float[18];
    protected float[] att_Point = new float[18];
    protected float att_Speed = 1.0f;
    protected float move_Speed = 0.02f;

    public GameObject cha_Model = null;
    public Animator cha_Anim = null;
    public WorldUI_Ctrl cha_Ui = null;
    protected RuntimeAnimatorController cha_Runtime = null;

    protected Collider[] target_Coll;       // 충될 체크된 콜라이더들을 저장할 배열
    protected NavMeshAgent navMeshAgent;

    protected float[] critical_Point = new float[18];
    protected int critical_Rand = 0;
    protected float critical_Power = 2.0f;

    protected float serch_Size = 0.0f;
    protected Transform target_Tr = null;
    protected Dictionary<GameObject, float> target_List = new Dictionary<GameObject, float>();
    // 타겟을 키값(GameObject) 타겟과의 거리(float)를 딕셔너리로 저장하기 위한 선언
}
