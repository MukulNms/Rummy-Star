using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Com.BigWin.Frontend.Data;

namespace Com.BigWin.Frontend
{
    public class ChangePinScreen : Screen
    {
        private TMP_InputField pinInputField;
        private TMP_InputField newPinInputField;
        private TMP_InputField confirmPinInputField;
        private Button okBtn;

        public override void Initialize(Transform screenContainer, ScreenController screenController)
        {
            base.Initialize(screenContainer, screenController);

            FindUIReferences();
            AddListners();
        }

        private void FindUIReferences()
        {
            pinInputField = screenObj.transform.FindRecursive("Pin").GetComponent<TMP_InputField>();
            newPinInputField = screenObj.transform.FindRecursive("NewPin").GetComponent<TMP_InputField>();
            confirmPinInputField = screenObj.transform.FindRecursive("ConfirmPin").GetComponent<TMP_InputField>();
            okBtn = screenObj.transform.FindRecursive("Ok").GetComponent<Button>();
        }

        private void AddListners()
        {
            okBtn.onClick.AddListener(OnClickOKButton);
        }

        private void OnClickOKButton()
        {
            // LoginForm form = new LoginForm(userNameInput.text, passwordInput.text, imei: "1321365464987");
            // webRequestHandler.Post(Constant.LOGIN_URL, JsonUtility.ToJson(form), OnChangePinRequestProcessed);
        }

        private void OnChangePinRequestProcessed(string json, bool success)
        {
        }


        public override ScreenID ScreenID => ScreenID.CHANGE_PIN_SCREEN;
        protected override string ScreenName => "ChangePinScreen";
    }
}