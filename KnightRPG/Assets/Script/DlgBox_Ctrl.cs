using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DlgBox_Ctrl : MonoBehaviour
{
    public static DlgBox_Ctrl Inst = null;      // �̱��� ����

    public delegate void BtnFunc();     // ��������Ʈ �Լ� ����
    BtnFunc btn_Func;   // ��������Ʈ�� ������ ����
    public GameObject dlg_Root = null;      // ���̾�α� �ڽ� ������Ʈ
    public Text dlg_Txt = null;     // ���̾�α׾��� �ؽ�Ʈ
    public Button ok_Btn = null;    // ���� ������ ��ư
    public Button yes_Btn = null;   // 2�� ������ ��ư
    public Button no_Btn = null;    // 2�� ������ ��ư

    void Awake()
    {
        Inst = this;
    }

    // ���̾�α׿� ���� ���ڿ�, ��������Ʈ�� �߰��� �Լ�, �������� �������� 2������ Ȯ���� bool ����
    public void DlgBoxSetting(string a_Txt, BtnFunc func, bool yesOrno = false)
    {
        btn_Func = func;    // �Ű������� �Ѿ�� �Լ��� ����
        dlg_Root.SetActive(true);   // ���̾�α� �ڽ��� ����
        dlg_Txt.text = a_Txt;   // �Ű������� �Ѿ�� ���ڿ��� �ؽ�Ʈ ��ȯ

        if(yesOrno != true)     // ���� ���������� 2�� ���������� üũ
        {
            ok_Btn.gameObject.SetActive(true);      // ���� ������ ��ư ����

            if (ok_Btn != null)     // ���� �������� ��ư�� ������ ����Ǵ� ����
                ok_Btn.onClick.AddListener(() =>
                {
                    if (btn_Func != null)   // ��������Ʈ�� �ִٸ� ����
                        btn_Func();

                    btn_Func = null;    // ��������Ʈ �� �ʱ�ȭ / �ʱ�ȭ�� �����ָ� ������ �߰��� �Լ��� �״�� �����ְ� �ȴ�.
                    ok_Btn.gameObject.SetActive(false);     // ���� ������ ��ư �����
                    dlg_Root.SetActive(false);      // ���̾�α� �����
                });
        }
        else
        {
            yes_Btn.gameObject.SetActive(true);     // 2�� ������ ��ư �ѱ�
            no_Btn.gameObject.SetActive(true);     // 2�� ������ ��ư �ѱ�

            if (yes_Btn != null)     // 2�� �������� ��ư�� ������ ����Ǵ� ����
                yes_Btn.onClick.AddListener(() =>
                {
                    if (btn_Func != null)   // ��������Ʈ�� �ִٸ� ����
                        btn_Func();

                    btn_Func = null;    // ��������Ʈ �� �ʱ�ȭ / �ʱ�ȭ�� �����ָ� ������ �߰��� �Լ��� �״�� �����ְ� �ȴ�.
                    yes_Btn.gameObject.SetActive(false);    // ��ư �����
                    no_Btn.gameObject.SetActive(false);    // ��ư �����
                    dlg_Root.SetActive(false);      // ���̾�α� �����
                });

            if (no_Btn != null)     // 2�� �������� ��ư�� ������ ����Ǵ� ����
                no_Btn.onClick.AddListener(() =>
                {
                    btn_Func = null;
                    yes_Btn.gameObject.SetActive(false);    // ��ư �����
                    no_Btn.gameObject.SetActive(false);    // ��ư �����
                    dlg_Root.SetActive(false);      // ���̾�α� �����
                });
        }

    }
}