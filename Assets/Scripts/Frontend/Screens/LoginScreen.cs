using TMPro;

using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Com.BigWin.Frontend.Data;
using System.Text;
using System.CodeDom;
using System.Collections;
using System;
using System.Linq;
using UnityEditor;
using System.Collections.Generic;

namespace Com.BigWin.Frontend
{
    public class LoginScreen : Screen
    {
        private TMP_InputField userNameInput;
        private TMP_InputField passwordInput;
        private Toggle rememberPasswordToggle;
        private Button loginBtn;
        private Button exitBtn;
        public override void Initialize(Transform screenContainer, ScreenController screenController)
        {
            base.Initialize(screenContainer, screenController);

            FindAllReferences();
            AddListeners();
        }

        private void FindAllReferences()
        {
            userNameInput = screenObj.transform.FindRecursive("UserName").GetComponentInChildren<TMP_InputField>();
            passwordInput = screenObj.transform.FindRecursive("Password").GetComponentInChildren<TMP_InputField>();
            rememberPasswordToggle = screenObj.transform.FindRecursive("RememberPassword").GetComponentInChildren<Toggle>();
            loginBtn = screenObj.transform.FindRecursive("LoginButton").GetComponent<Button>();
            exitBtn = screenObj.transform.FindRecursive("ExitButton").GetComponent<Button>();
           
        }

        private void AddListeners()
        {
            loginBtn.onClick.AddListener(OnClickLoginButton);
            exitBtn.onClick.AddListener(() =>
            {
                SoundManager.instance.PlayClip("quit");
                Application.Quit();
            });
        }

        bool isServerVersionGreater = false;
        string newApkLink = string.Empty;
        bool isDataLoaded = false;
        public override void Show(object data = null)
        {
            base.Show();
            isServerVersionGreater = false;
            isLoginRequestSent = false;

            newApkLink = "http://13.233.60.158:5000";

            isDataLoaded = false;


            userNameInput.text = sc.data.Email?.Replace("FUN", string.Empty);
            passwordInput.text = sc.data.Password;
            // Comment For ByPass APK Version
           // if (Application.platform == RuntimePlatform.Android)
             //   CheckAndroidVrsion();
        }
        public override void Hide()
        {
            base.Hide();
            isLoginRequestSent = false;
        }

        void CheckAndroidVrsion()
        {

            webRequestHandler.Get(Constant.GET_APK_VERSION_URL, (json, status) =>
             {
                 ApkVersion apkVersion = JsonConvert.DeserializeObject<ApkVersion>(json);
                
                 isDataLoaded = true;
                 print($"server version {apkVersion.data.version_code}  and app version {Application.version}");
                 isServerVersionGreater = compareVersionCode(Application.version, apkVersion.data.version_code) == -1;
                 if (!isServerVersionGreater) return;
                 dialogue.Show("Please download the lastest version\npress ok to download");
                 dialogue.OnDialogHide = () => Application.OpenURL(apkVersion.data.ApkUrl);
             });
        }

        //return 0 if same, return 1 if version1 is greater and return -1 if version1 is lower
        public int compareVersionCode(string version1, string version2, char spliteType = '.')
        {

            string[] version1A = version1.Split(spliteType);
            string[] version2A = version2.Split(spliteType);



            // To avoid IndexOutOfBounds
            int maxIndex = Math.Min(version1A.Length, version2A.Length);

            for (int i = 0; i < maxIndex; i++)
            {
                int v1 = int.Parse(version1A[i]);
                int v2 = int.Parse(version2A[i]);

                if (v1 < v2)
                {
                    return -1;
                }
                else if (v1 > v2)
                {
                    return 1;
                }

            }
            return 0;
        }
        public override ScreenID ScreenID => ScreenID.LOGIN_SCREEN;

        protected override string ScreenName => "LoginScreen";

        bool isLoginRequestSent;
        private void OnClickLoginButton()
        {
            SoundManager.instance.PlayClip("login");

            if (Application.platform == RuntimePlatform.Android)
                if (isLoginRequestSent)
                {
                    dialogue.Show("Please wait");
                    return;
                }

            if (Application.platform == RuntimePlatform.Android)
                if (isServerVersionGreater)
                {
                    dialogue.Show("Please download the lastest version\npress ok to download");
                    dialogue.OnDialogHide = () => Application.OpenURL(newApkLink);
                    return;
                }


            if (userNameInput.text == "" && passwordInput.text == "")
            {
                AndroidToastMsg.ShowAndroidToastMessage("Emplty Field");
                print("Emplty Field");
                return;
            }
            LoginForm form = new LoginForm() { user_id = "FUN" + userNameInput.text, password = passwordInput.text, imei = "1321365464987", device = SystemInfo.deviceUniqueIdentifier };


            if (!SocketRequest.intance.isConnected)
            {
                dialogue.Show("Please check your internet connection");
                return;
            }
            if (!isLoginRequestSent)
                SocketRequest.intance.SendEvent(Constant.OnLogin, form, (res) =>
            {
                BackEndData3<LoginData> back = Constant.GetObjectOfType<BackEndData3<LoginData>>(res);
                print("Login resposne " + back.status);
                isLoginRequestSent = false;
                OnLoginRequestProcessed(back.status, back.message,res);
            });
            isLoginRequestSent = true;
            //webRequestHandler.Post(Constant.LOGIN_URL, JsonUtility.ToJson(form), OnLoginRequestProcessed);
        }

        private void OnLoginRequestProcessed(int status, string message,object data=null)
        {
            if (status == 401)
            {
                dialogue.Show(message);
                return;
            } 
            if (status == 404)
            {
                Debug.Log("incorrect password "+message);
                dialogue.Show(message, okButtonMsg: "Try again");
                return;
            }
            if (status == 200)
            {
                SaveData();
                sc.Show(ScreenID.HOME_SCREEN, false,data); ;
                return;
            }
            else
            {
                if (status == 202)
                {
                    dialogue.Show(message, okButtonMsg: "Force Login");
                    ForceLogin form = new ForceLogin { user_id = "FUN" + userNameInput.text, password = passwordInput.text, device = SystemInfo.deviceUniqueIdentifier };
                    dialogue.OnDialogHide = () =>
                    {
                        SocketRequest.intance.SendEvent(Constant.OnForceLogin, form, (res) =>
                        {
                            BackEndData3<LoginResponseData> back = Constant.GetObjectOfType<BackEndData3<LoginResponseData>>(res);
                            print("Login resposne " + res);
                            OnLoginRequestProcessed(back.status, back.message);
                        },Constant.OnLogin);
                        SaveData();
                    };
                    return;
                }
            }

            dialogue.Show(message);
        }

        private void SaveData()
        {
            if (rememberPasswordToggle.isOn)
            {
                sc.data.SaveCredentials("FUN" + userNameInput.text, passwordInput.text);
            }
            else
            {
                sc.data.DeleteCredentials();
            }
        }

        private void OnForeceLoginRequestProcessed(string json)
        {

            LoginResponseData loginResponseData = JsonUtility.FromJson<LoginResponseData>(json);

            if (loginResponseData.user_data.status == "200")
            {
                if (rememberPasswordToggle.isOn)
                {
                    sc.data.SaveCredentials("FUN" + userNameInput.text, passwordInput.text);
                }
                else
                {
                    sc.data.DeleteCredentials();
                }

                sc.data.LoginResponseData = loginResponseData;
                sc.OnResumeLogin();
            }
            else
            {
                dialogue.Show(loginResponseData.user_data.message);
                if (loginResponseData.user_data.status == "202")
                {
                    LogoutForm form = new LogoutForm("FUN" + userNameInput.text);
                    dialogue.OnDialogHide = () => webRequestHandler.Post(Constant.LOGOUT_URL, JsonUtility.ToJson(form), (x, y) => OnClickLoginButton());
                }

            }
        }
    }
}


public class ApkData
{
    public string status;
    public string message;
    public string version_code;
    public string ApkUrl;
}

[Serializable]
public class ApkVersion
{
    public ApkData data;
}
[Serializable]
public class ForceLogin
{
    public string user_id;
    public string password;
    public string device;
}
public class UserData
{
    public string message;
    public string status;
    public string coins;
    public string user_id;
    public string device;
}
[Serializable]
public class ForceLoginResponse
{
    public UserData user_data;
}

public class e
{
    public string message { get; set; }
    public string status { get; set; }
}
[Serializable]
public class Invalide
{
    public e user_data { get; set; }
}
