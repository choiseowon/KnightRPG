using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using UnityEngine.Networking;

enum TabMode
{
    Login,
    Create
}

public class Login_Mgr : MonoBehaviour
{
    public InputField id_Field = null;
    public InputField pw_Field = null;

    public InputField create_IdField = null;
    public InputField create_PwField = null;
    public InputField create_NickField = null;

    public GameObject login_Root = null;
    public GameObject create_Root = null;
    public GameObject loading_Root = null;

    //public DlgBox_Ctrl dlgBox_Root = null;

    public Button create_Btn = null;
    public Button login_Btn = null;

    public Button return_Btn = null;
    public Button ok_Btn = null;

    TabMode tabMode = TabMode.Login;
    //int tabMode_Check = 0;

    string LoginUrl = "";
    string CreateUrl = "";
    string NewDataUrl = "";
    string NewItemUrl = "";

    void Awake()
    {
        LoginUrl = "http://seowonserver.dothome.co.kr/KnightDB/LoginUrl.php";
        CreateUrl = "http://seowonserver.dothome.co.kr/KnightDB/CreateUrl.php";
        NewDataUrl = "http://seowonserver.dothome.co.kr/KnightDB/NewDataUrl.php";
        NewItemUrl = "http://seowonserver.dothome.co.kr/KnightDB/NewItemUrl.php";
    }

    void Start()
    {
        if (create_Btn != null)
            create_Btn.onClick.AddListener(() =>
            {
                id_Field.text = "";
                pw_Field.text = "";

                tabMode = TabMode.Create;
                login_Root.SetActive(false);
                create_Root.SetActive(true);
                create_IdField.Select();
            });

        if (login_Btn != null)
            login_Btn.onClick.AddListener(() =>
            {
                LoginBtn();
            });

        if (return_Btn != null)
            return_Btn.onClick.AddListener(() =>
            {
                create_IdField.text = "";
                create_PwField.text = "";
                create_NickField.text = "";

                tabMode = TabMode.Login;
                login_Root.SetActive(true);
                create_Root.SetActive(false);
                id_Field.Select();
            });

        if (ok_Btn != null)
            ok_Btn.onClick.AddListener(() =>
            {
                CreateBtn();
            });

        Sound_Ctrl.Inst.BgmSoundPlay("TitleBGM");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(tabMode == TabMode.Login)
            {
                if (id_Field.isFocused == true)
                    pw_Field.Select();
                else if (pw_Field.isFocused == true)
                    id_Field.Select();
            }
            else if(tabMode == TabMode.Create)
            {
                if (create_IdField.isFocused == true)
                    create_PwField.Select();
                else if (create_PwField.isFocused == true)
                    create_NickField.Select();
                else if (create_NickField.isFocused == true)
                    create_IdField.Select();
            }
        }

        if(Input.GetKeyDown(KeyCode.Return) && DlgBox_Ctrl.Inst.dlg_Root.activeSelf == false)
        {
            if (tabMode == TabMode.Login)
                LoginBtn();
        }
    }

    public void LoginBtn()
    {
        string a_IdStr = id_Field.text;
        string a_PwStr = pw_Field.text;

        if (a_IdStr.Trim() == "" || a_PwStr.Trim() == "")
        {
            DlgBox_Ctrl.Inst.DlgBoxSetting("빈칸 에러", null);
            return;
        }

        if (!(3 <= a_IdStr.Length && a_IdStr.Length < 15))  //3~15
        {
            DlgBox_Ctrl.Inst.DlgBoxSetting("아이디 글자 수 에러", null);
            return;
        }
        if (!(3 <= a_PwStr.Length && a_PwStr.Length < 15))  //6~15
        {
            DlgBox_Ctrl.Inst.DlgBoxSetting("비밀번호 글자 수 에러", null);
            return;
        }

        StartCoroutine(LoginCo(a_IdStr, a_PwStr));

    }

    IEnumerator LoginCo(string a_IdStr, string a_PwStr)
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_user", a_IdStr, System.Text.Encoding.UTF8);
        form.AddField("Input_pass", a_PwStr, System.Text.Encoding.UTF8);
        loading_Root.SetActive(true);

        UnityWebRequest a_www = UnityWebRequest.Post(LoginUrl, form);
        yield return a_www.SendWebRequest();

        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);

            if (sz.Contains("Login-Success!!") == false)
            {
                if(sz.Contains("ID Error.") == true)
                    DlgBox_Ctrl.Inst.DlgBoxSetting("아이디 오류", null);
                else if (sz.Contains("Pass Error.") == true)
                    DlgBox_Ctrl.Inst.DlgBoxSetting("비밀번호 오류", null);
                else
                    DlgBox_Ctrl.Inst.DlgBoxSetting("로그인 실패...", null);

                loading_Root.SetActive(false);
                yield break;
            }

            if (sz.Contains("user_nick") == false)
                yield break;

            var N = JSON.Parse(sz);

            if (N == null)
                yield break;

            if (N["user_nick"] != null)
                GlobalData.user_Nick = N["user_nick"];

            if (N["user_no"] != null)
                GlobalData.user_Number = N["user_no"];

            StartCoroutine(GlobalData.LoadDataCo(GlobalData.user_Number));
            StartCoroutine(GlobalData.LoadItemCo(GlobalData.user_Number));
            loading_Root.SetActive(false);
            PlayerPrefs.SetFloat("BgmVolume", GlobalData.bgm_Volume);   // 게임 종료 시 볼륨값 로컬로 저장
            PlayerPrefs.SetFloat("SfVolume", GlobalData.sf_Volume);   // 게임 종료 시 볼륨값 로컬로 저장
            UnityEngine.SceneManagement.SceneManager.LoadScene("InGameScene");
        }
        else
        {
            loading_Root.SetActive(false);
            Debug.Log(a_www.error);
        }
    }

    public void CreateBtn()
    {
        string a_IdStr = create_IdField.text;
        string a_PwStr = create_PwField.text;
        string a_NickStr = create_NickField.text;

        if (a_IdStr.Trim() == "" || a_PwStr.Trim() == "")
        {
            DlgBox_Ctrl.Inst.DlgBoxSetting("빈칸 에러", null);
            return;
        }

        if (!(3 <= a_IdStr.Length && a_IdStr.Length < 15))  //3~15
        {
            DlgBox_Ctrl.Inst.DlgBoxSetting("아이디 글자 수 에러", null);
            return;
        }

        if (!(3 <= a_PwStr.Length && a_PwStr.Length < 15))  //6~15
        {
            DlgBox_Ctrl.Inst.DlgBoxSetting("비밀번호 글자 수 에러", null);
            return;
        }

        if (!(3 <= a_NickStr.Length && a_NickStr.Length < 15))  //6~15
        {
            DlgBox_Ctrl.Inst.DlgBoxSetting("닉네임 글자 수 에러", null);
            return;
        }

        StartCoroutine(CreateCo(a_IdStr, a_PwStr, a_NickStr));
        loading_Root.SetActive(false);

    }

    IEnumerator CreateCo(string a_IdStr, string a_PwStr, string a_NickStr)
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_user", a_IdStr, System.Text.Encoding.UTF8);
        form.AddField("Input_pass", a_PwStr, System.Text.Encoding.UTF8);
        form.AddField("Input_nick", a_NickStr, System.Text.Encoding.UTF8);
        loading_Root.SetActive(true);

        UnityWebRequest a_www = UnityWebRequest.Post(CreateUrl, form);
        yield return a_www.SendWebRequest();

        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);

            if (sz.Contains("ID does exist.") == true)
            {
                DlgBox_Ctrl.Inst.DlgBoxSetting("중복되는 아이디가 있습니다.", null);
                yield break;
            }

            if (sz.Contains("Nickname does exist.") == true)
            {
                DlgBox_Ctrl.Inst.DlgBoxSetting("중복되는 닉네임이 있습니다.", null);
                yield break;
            }

            if (sz.Contains("Create OK!") == false)
                yield break;

            var N = JSON.Parse(sz);

            if (sz.Contains("Number OK!") == false)
                yield break;

            string user_Num = "0";

            if (N["user_no"] != null)
                user_Num = N["user_no"];

            UserDataCreate(user_Num);
        }
        else
        {
            Debug.Log(a_www.error);
            loading_Root.SetActive(false);
        }
    }

    public void UserDataCreate(string user_Num)
    {
        StartCoroutine(UserDataCo(user_Num));
    }

    IEnumerator UserDataCo(string user_Num)
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_number", user_Num, System.Text.Encoding.UTF8);

        UnityWebRequest a_www = UnityWebRequest.Post(NewDataUrl, form);
        yield return a_www.SendWebRequest();

        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);

            if (sz.Contains("Data OK!") == false)
            {
                DlgBox_Ctrl.Inst.DlgBoxSetting("아이디 생성 실패..", null);
                Debug.Log("data error");
                loading_Root.SetActive(false);
                yield break;
            }

            Debug.Log("Data OK!");

            StartCoroutine(UserItemCo(user_Num));
        }
        else
        {
            Debug.Log(a_www.error);
            loading_Root.SetActive(false);
        }
    }

    IEnumerator UserItemCo(string user_Num)
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_number", user_Num, System.Text.Encoding.UTF8);

        UnityWebRequest a_www = UnityWebRequest.Post(NewItemUrl, form);
        yield return a_www.SendWebRequest();

        if (a_www.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);

            if (sz.Contains("Item OK!") == false)
            {
                DlgBox_Ctrl.Inst.DlgBoxSetting("아이디 생성 실패..", null);
                Debug.Log("item error");
                loading_Root.SetActive(false);
                yield break;
            }

            create_IdField.text = "";
            create_PwField.text = "";
            create_NickField.text = "";

            DlgBox_Ctrl.Inst.DlgBoxSetting("아이디 생성 성공 !", null);
            tabMode = TabMode.Login;
            login_Root.SetActive(true);
            create_Root.SetActive(false);
            loading_Root.SetActive(false);

            Debug.Log("Item OK!");
        }
        else
        {
            Debug.Log(a_www.error);
            loading_Root.SetActive(false);
        }
    }
}
