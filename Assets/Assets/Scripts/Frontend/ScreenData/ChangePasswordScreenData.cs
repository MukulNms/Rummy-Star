using System;

namespace Com.BigWin.Frontend.Data
{
    public class ChangePasswordScreenData : ScreenData
    {
        public override ScreenID ScreenID => ScreenID.CHANGE_PASSWORD_SCREEN;
    }

    [Serializable]
    public class ChangePasswordForm
    {
        public string user_id;
        public string password;
        public string device;

        public ChangePasswordForm(string user_id, string password, string device)
        {
            this.user_id = user_id;
            this.password = password;
            this.device = device;
        }
    }

    [Serializable]
    public class ChangePasswordResponce
    {
        public string message;
        public string status;
    }
}