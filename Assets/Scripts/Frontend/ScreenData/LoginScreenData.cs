using System;

namespace Com.BigWin.Frontend.Data
{
    public class LoginScreenData : ScreenData
    {
        public override ScreenID ScreenID => ScreenID.LOGIN_SCREEN;
    }

    [Serializable]
    public class LoginForm
    {
        public string user_id;
        public string password;
        public string device;
        public string imei;
    }

    [Serializable]
    public class LoginResponseData
    {
        public LoginResponce user_data;
    }

    [Serializable]
    public class LoginResponce
    {
        public string message;
        public string status;
        public string coins;
        public long round_count;
        public string device;
        public string user_id;
    }

    [Serializable]
    public class LogoutForm
    {
        public string user_id;
        public LogoutForm(string user_id)
        {
            this.user_id = user_id;
        }
    }
}