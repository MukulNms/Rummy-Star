
namespace Com.BigWin.Frontend.Data
{
    public abstract class ScreenData
    {
        public abstract ScreenID ScreenID
        {
            get;
        }
    }

    public enum ScreenID
    {
        SPLASH_SCREEN,
        LOGIN_SCREEN,
        // OTP_VERIFICATION_SCREEN,
        // REGISTRATION_SCREEN,
        HOME_SCREEN,
        FUN_TARGET_TIMER_GAME_SCREEN,
        JEETO_JOKER_TIMER_GAME_SCREEN,
        MY_ACCOUNT_SCREEN,
        POINT_TRANSFER_SCREEN,
        RECEIVABLES_SCREEN,
        TRANSFERABLES_SCREEN,
        CHANGE_PASSWORD_SCREEN,
        CHANGE_PIN_SCREEN,

        // REFER_AND_EARN_SCREEN,
        // NOTIFICATION_SCREEN,
        // ADD_CASH_SCREEN,
        // WITHDRAW_SCREEN,
        // PROFILE_SCREEN,
        // TRANSACTION_SCREEN,
        NONE
    }
}