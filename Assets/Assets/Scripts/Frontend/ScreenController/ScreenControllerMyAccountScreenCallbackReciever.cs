using Com.BigWin.Frontend.Data;

namespace Com.BigWin.Frontend
{
    public partial class ScreenController
    {
        public void OnClickPointTransferScreen()
        {

            Show(ScreenID.POINT_TRANSFER_SCREEN, ShowAsSubScreen: true);
        }

        public void OnClickReceivablesScreen()
        {

            Show(ScreenID.RECEIVABLES_SCREEN, ShowAsSubScreen: true);
        }

        public void OnClickTransferablesScreen()
        {

            Show(ScreenID.TRANSFERABLES_SCREEN, ShowAsSubScreen: true);
        }

        public void OnClickChangePasswordScreen()
        {
            Show(ScreenID.CHANGE_PASSWORD_SCREEN, ShowAsSubScreen: true);
        }

        //public void OnClickChangePinScreen()
        //{
        //    Show(ScreenID.CHANGE_PIN_SCREEN, ShowAsSubScreen: true);
        //}
    }
}