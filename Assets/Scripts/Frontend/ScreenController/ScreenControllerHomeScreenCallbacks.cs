using Com.BigWin.Frontend.Data;

namespace Com.BigWin.Frontend
{
    public partial class ScreenController
    {
        // public void OnClickReferAndEarn()
        // {
        //      Show(new ReferAndEarnScreenData(dataStorageController.ProfileResponseData.response.data));
        // }

        // public void OnClickNotification()
        // {
        //      Show(new NotificationScreenData(dataStorageController.LoginResponseData.response.data));
        // }

        // public void OnClickProfile()
        // {
        //     Show(new ProfileScreenData());
        // }

        public void OnClickMyAccount()
        {
            Show(ScreenID.MY_ACCOUNT_SCREEN);
        }

        public void OnClickFunTargetTimerGame()
        {
            Show(ScreenID.FUN_TARGET_TIMER_GAME_SCREEN);
        }
    }
}