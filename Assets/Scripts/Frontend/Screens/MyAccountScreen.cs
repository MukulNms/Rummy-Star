using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Com.BigWin.Frontend.Data;

namespace Com.BigWin.Frontend
{
    public class MyAccountScreen : Screen
    {
        private Toggle pointTransferBtn;
        private Toggle receivablesBtn;
        private Toggle transferablesBtn;
        private Toggle changePasswordBtn;
        private Toggle changePinBtn;
        private Button closeBtn;

        public static TextMeshProUGUI mainBalance;

        public override void Initialize(Transform screenContainer, ScreenController screenController)
        {
            base.Initialize(screenContainer, screenController);

            FindUIReferences();
            AddListners();
        }

        private void FindUIReferences()
        {
            pointTransferBtn = screenObj.transform.FindRecursive("PointTransferButton").GetComponent<Toggle>();
            receivablesBtn = screenObj.transform.FindRecursive("ReceivablesButton").GetComponent<Toggle>();
            transferablesBtn = screenObj.transform.FindRecursive("TransferablesButton").GetComponent<Toggle>();
            changePasswordBtn = screenObj.transform.FindRecursive("ChangePasswordButton").GetComponent<Toggle>();
            //changePinBtn = screenObj.transform.FindRecursive("ChangePinButton").GetComponent<Toggle>();
            closeBtn = screenObj.transform.FindRecursive("CloseButton").GetComponent<Button>();

            mainBalance = screenObj.transform.FindRecursive("Chip").GetComponentInChildren<TextMeshProUGUI>();
        }

        private void AddListners()
        {
            pointTransferBtn.onValueChanged.AddListener((isOn) => { if (isOn) sc.OnClickPointTransferScreen(); });
            receivablesBtn.onValueChanged.AddListener((isOn) => { if (isOn) sc.OnClickReceivablesScreen(); });
            transferablesBtn.onValueChanged.AddListener((isOn) => { if (isOn) sc.OnClickTransferablesScreen(); });
            changePasswordBtn.onValueChanged.AddListener((isOn) => { if (isOn) sc.OnClickChangePasswordScreen(); });
            //changePinBtn.onValueChanged.AddListener((isOn) => { if (isOn) screenController.OnClickChangePinScreen(); });
            closeBtn.onClick.AddListener(sc.OnClickHome);
        }

        public override void Show(object data=null)
        {
            base.Show();
            sc.OnClickPointTransferScreen();
        }

        public override ScreenID ScreenID => ScreenID.MY_ACCOUNT_SCREEN;
        protected override string ScreenName => "MyAccountScreen";
    }
}