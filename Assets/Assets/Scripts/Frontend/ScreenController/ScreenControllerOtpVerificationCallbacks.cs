using Com.BigWin.Frontend.Data;

namespace Com.BigWin.Frontend
{
    public partial class ScreenController
    {
        public void OnResumeLogin()
        {
            OnClickHome();
        }

        public void OnClickHome()
        {
            Show(ScreenID.HOME_SCREEN);
        }
    }
}