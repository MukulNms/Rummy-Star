using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Com.BigWin.Frontend.Data;
using Newtonsoft.Json;
namespace Com.BigWin.Frontend
{
    public class ChangePasswordScreen : Screen
    {
        private TMP_InputField passwordInputField;
        private TMP_InputField newPasswordInputField;
        private TMP_InputField confirmPasswordInputField;
        private Button okBtn;

        public override void Initialize(Transform screenContainer, ScreenController screenController)
        {
            base.Initialize(screenContainer, screenController);

            FindUIReferences();
            AddListners();
        }

        private void FindUIReferences()
        {
            passwordInputField = screenObj.transform.FindRecursive("Password").GetComponent<TMP_InputField>();
            newPasswordInputField = screenObj.transform.FindRecursive("NewPassword").GetComponent<TMP_InputField>();
            confirmPasswordInputField = screenObj.transform.FindRecursive("ConfirmPassword").GetComponent<TMP_InputField>();
            okBtn = screenObj.transform.FindRecursive("Ok").GetComponent<Button>();
        }

        private void AddListners()
        {

            okBtn.onClick.AddListener(OnClickOKButton);
        }

        private void OnClickOKButton()
        {
            if (newPasswordInputField.text != confirmPasswordInputField.text)
            {

                if (Application.platform != RuntimePlatform.Android)
                {
                    dialogue.Show("password does not matched");
                }
                else
                {
                    AndroidToastMsg.ShowAndroidToastMessage("password does not matched");
                }
                return;
            }
            if (newPasswordInputField.text == confirmPasswordInputField.text)
            {
                object o = new { user_id = sc.data.Email, old_password = passwordInputField.text.ToString(), new_password = newPasswordInputField.text };
                SocketRequest.intance.SendEvent(Constant.OnChangePassword, o, (res) =>
                    {
                        BackEndData3<ChangePasswordResponce> filterResponse = JsonConvert.DeserializeObject<BackEndData3<ChangePasswordResponce>>(res);
                        if (Application.platform != RuntimePlatform.Android)
                        {
                            dialogue.Show(filterResponse.message);
                        }
                        else
                        {
                            AndroidToastMsg.ShowAndroidToastMessage(filterResponse.message);
                        }
                        ResetUi();
                    });
                //webRequestHandler.Post(Constant.CHANGE_PASSWORD_URL, JsonUtility.ToJson(form), OnChangePasswordRequestProcessed);
            }
            else
                dialogue.Show(Constant.PASSWORD_NOT_MATCHED);
        }
        void ResetUi()
        {
            passwordInputField.text = string.Empty;
            newPasswordInputField.text = string.Empty;
            confirmPasswordInputField.text = string.Empty;
        }

        private void OnChangePasswordRequestProcessed(ChangePasswordResponce responce, bool success)
        {
            //ChangePasswordResponce responce = JsonUtility.FromJson<ChangePasswordResponce>(json);

            if (responce.status == "200")
            {
                dialogue.Show(responce.message);
                passwordInputField.text = string.Empty;
                newPasswordInputField.text = string.Empty;
                confirmPasswordInputField.text = string.Empty;
            }
        }


        public override ScreenID ScreenID => ScreenID.CHANGE_PASSWORD_SCREEN;
        protected override string ScreenName => "ChangePasswordScreen";
    }
}