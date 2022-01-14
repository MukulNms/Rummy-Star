using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Com.BigWin.Frontend.Data;
using m = UnityEngine.MonoBehaviour;
using System.Linq;
using Newtonsoft.Json;
using System;

namespace Com.BigWin.Frontend
{
    public class PointTransferScreen : Screen
    {
        private TMP_InputField toAccountInputField;
        private TMP_InputField pwdInputField    ;
        private TMP_InputField amountInputField;
        private Button okBtn;
        [SerializeField] private Button backBtn;
        private TextMeshProUGUI mainBalance;
        private bool isTransationFinished;
        bool isDataLoaded;
        public override void Initialize(Transform screenContainer, ScreenController screenController)
        {
            base.Initialize(screenContainer, screenController);

            FindUIReferences();
            AddListners();
        }

        private void FindUIReferences()
        {
            toAccountInputField = screenObj.transform.FindRecursive("ToAccount").GetComponent<TMP_InputField>();
            pwdInputField = screenObj.transform.FindRecursive("Pin").GetComponent<TMP_InputField>();
            amountInputField = screenObj.transform.FindRecursive("Amount").GetComponent<TMP_InputField>();
            mainBalance= screenObj.transform.parent.FindRecursive("mainBalance").GetComponent<TextMeshProUGUI>();
            okBtn = screenObj.transform.FindRecursive("Ok").GetComponent<Button>();
        }

        private void AddListners()
        {
            okBtn.onClick.AddListener(OnClickOKButton);
        }

        private void OnClickOKButton()
        {
            
            if (!isTransationFinished)
            {
                m.print("please wait");
                AndroidToastMsg.ShowAndroidToastMessage("please wait");
                return;
            }
            if (string.IsNullOrEmpty(amountInputField.text))
            {
                m.print("invalid amount");
                AndroidToastMsg.ShowAndroidToastMessage("invalid amount");
                return;
            }
            if (string.IsNullOrEmpty(pwdInputField.text))
            {
                m.print("invalid password");
                AndroidToastMsg.ShowAndroidToastMessage("invalid amount");
                return;
            }
            if (!ValidateNumber(amountInputField.text, "account")) return;
            AndroidToastMsg.ShowAndroidToastMessage("Please wait");
            string amount = amountInputField.text.Trim();
            string accountNumber =  toAccountInputField.text.Trim();
            string pwd = pwdInputField.text.Trim();
            isTransationFinished = false;
            userData userData = new userData() 
            { password = pwd, points = amount, reciever = accountNumber,
                sender = sc.data.Email };

            SocketRequest.intance.SendEvent(Constant.OnSendPoints, userData, (res) =>
            {
                BackEndData3<Status> filterResponse = JsonConvert.DeserializeObject<BackEndData3<Status>>(res);

                var status = filterResponse.status;
                Status status1 = filterResponse.data;
                if (status1.status == 401)//invalid user
                {
                    OnInvalidUser(status1.message); return;
                }
                m.print(status1.message);
                AndroidToastMsg.ShowAndroidToastMessage(status1.message);
                if (status1.status == 401)//invalid user
                {
                    OnInvalidUser(status1.message);return;
                }if (status == 200)
                {
                    UpdateBalane();
                    ResetUi();
                }
                    if (Application.platform != RuntimePlatform.Android)
                    {
                        dialogue.Show(filterResponse.message);
                    }
                    else
                    {
                        AndroidToastMsg.ShowAndroidToastMessage(filterResponse.message);
                    }
                isTransationFinished = true;
            });
        }

        void UpdateBalane()
        {
            object user = new { user_id = sc.data.Email };
            SocketRequest.intance.SendEvent(Constant.OnUserProfile, user, (json) =>
            {
                BackEndData3<PlayerProfile> profile = JsonUtility.FromJson<BackEndData3<PlayerProfile>>(json);
                mainBalance.text = profile.data.coins.ToString();
            });
        }
        private void OnInvalidUser(string msg)
        {
            m.print("invalid user");
            dialogue.Show(msg, okButtonMsg: "Logout");
            dialogue.OnDialogHide = () =>
            {
                SocketRequest.intance.SendEvent(Constant.OnLogout);
                sc.OnLoginScreen();
            };
        }
        public override void Show(object data = null)
        {
            base.Show();
            isTransationFinished = true;
            UpdateBalane();
        }
        private bool ValidateNumber(string sample, string nam)
        {
            sample=sample.Trim();
            if (string.IsNullOrEmpty(sample))
            {
                m.print(nam + " is empty");
                AndroidToastMsg.ShowAndroidToastMessage(nam + " is empty");
                return false;
            }

            m.print(sample);
            if (!sample.All(char.IsDigit))
            {
                m.print(nam + " is invalid");
                AndroidToastMsg.ShowAndroidToastMessage(nam + " is invalid");
                return false;
            }
            return true;
        }
        
        private void OnSuccessfullPointTrasfer()
        {
            string username = sc.data.Email;
            string password = sc.data.Password;
            isDataLoaded = true;
            LoginForm form = new LoginForm() { user_id = username, password = password, imei = "1321365464987", device = SystemInfo.deviceUniqueIdentifier };

            webRequestHandler.Post(Constant.PROFILE_URL, JsonUtility.ToJson(form), OnLoginRequestProcessed);
            ResetUi();
        }
        private void ResetUi()
        {
            toAccountInputField.text = string.Empty;
            pwdInputField.text = string.Empty;
            amountInputField.text = string.Empty;
        }
        private void OnLoginRequestProcessed(string json, bool success)
        {
            Profile profliedata = JsonUtility.FromJson<Profile>(json);

            if (profliedata.user_data.status == "200")
            {
                mainBalance.text = profliedata.user_data.points.ToString();
                sc.data.LoginResponseData.user_data.coins= profliedata.user_data.points.ToString();
            }
            else
            {
                AndroidToastMsg.ShowAndroidDefaltMessage();
                m.print("something went wrong");
            }
        }

        public override ScreenID ScreenID => ScreenID.POINT_TRANSFER_SCREEN;
        protected override string ScreenName => "PointTransferScreen";
    }
}

[Serializable]
public class userData
{
    public string sender;
    public string reciever;
    public string points;
    public string password;
}

[Serializable]
public class ProfileData
{
    public string status ;
    public string message;
    public string points ;
    public string device ;
    public string user_id;
}


public class Profile
{
    public ProfileData user_data;
}

[Serializable]
public class errorMsg
{
   public string status ;
   public string error ;
}
