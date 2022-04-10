using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DlgBox_Ctrl : MonoBehaviour
{
    public static DlgBox_Ctrl Inst = null;      // 싱글톤 선언

    public delegate void BtnFunc();     // 델리게이트 함수 생성
    BtnFunc btn_Func;   // 델리게이트를 저장한 변수
    public GameObject dlg_Root = null;      // 다이얼로그 박스 오브젝트
    public Text dlg_Txt = null;     // 다이얼로그안의 텍스트
    public Button ok_Btn = null;    // 단일 선택지 버튼
    public Button yes_Btn = null;   // 2중 선택지 버튼
    public Button no_Btn = null;    // 2증 선택지 버튼

    void Awake()
    {
        Inst = this;
    }

    // 다이얼로그에 써질 문자열, 델리게이트에 추가할 함수, 선택지가 단일인지 2중인지 확인할 bool 변수
    public void DlgBoxSetting(string a_Txt, BtnFunc func, bool yesOrno = false)
    {
        btn_Func = func;    // 매개변수로 넘어온 함수를 저장
        dlg_Root.SetActive(true);   // 다이얼로그 박스를 켜줌
        dlg_Txt.text = a_Txt;   // 매개변수로 넘어온 문자열로 텍스트 변환

        if(yesOrno != true)     // 단일 선택지인지 2중 선택지인지 체크
        {
            ok_Btn.gameObject.SetActive(true);      // 단일 선택지 버튼 켜줌

            if (ok_Btn != null)     // 단일 선택지의 버튼을 누르면 실행되는 내용
                ok_Btn.onClick.AddListener(() =>
                {
                    if (btn_Func != null)   // 델리게이트가 있다면 실행
                        btn_Func();

                    btn_Func = null;    // 델리게이트 값 초기화 / 초기화를 안해주면 이전에 추가한 함수가 그대로 남아있게 된다.
                    ok_Btn.gameObject.SetActive(false);     // 단일 선택지 버튼 숨기기
                    dlg_Root.SetActive(false);      // 다이얼로그 숨기기
                });
        }
        else
        {
            yes_Btn.gameObject.SetActive(true);     // 2중 선택지 버튼 켜기
            no_Btn.gameObject.SetActive(true);     // 2중 선택지 버튼 켜기

            if (yes_Btn != null)     // 2중 선택지의 버튼을 누르면 실행되는 내용
                yes_Btn.onClick.AddListener(() =>
                {
                    if (btn_Func != null)   // 델리게이트가 있다면 실행
                        btn_Func();

                    btn_Func = null;    // 델리게이트 값 초기화 / 초기화를 안해주면 이전에 추가한 함수가 그대로 남아있게 된다.
                    yes_Btn.gameObject.SetActive(false);    // 버튼 숨기기
                    no_Btn.gameObject.SetActive(false);    // 버튼 숨기기
                    dlg_Root.SetActive(false);      // 다이얼로그 숨기기
                });

            if (no_Btn != null)     // 2중 선택지의 버튼을 누르면 실행되는 내용
                no_Btn.onClick.AddListener(() =>
                {
                    btn_Func = null;
                    yes_Btn.gameObject.SetActive(false);    // 버튼 숨기기
                    no_Btn.gameObject.SetActive(false);    // 버튼 숨기기
                    dlg_Root.SetActive(false);      // 다이얼로그 숨기기
                });
        }

    }
}