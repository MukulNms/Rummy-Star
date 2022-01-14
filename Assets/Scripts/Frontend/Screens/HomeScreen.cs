

using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Com.BigWin.Frontend.Data;
using System;
using Newtonsoft.Json;
namespace Com.BigWin.Frontend
{
    public class HomeScreen : Screen
    {
        [SerializeField] TextMeshProUGUI chipTxt;
        [SerializeField] TextMeshProUGUI userIdTxt;
        [SerializeField] Button myAccountBtn;
        [SerializeField] Button logoutBtn;
        [SerializeField] Button funTargetTimerGameBtn;
        [SerializeField] Button jeetoJokerTimerGameBtn;

        public override void Initialize(Transform screenContainer, ScreenController screenController)
        {
            base.Initialize(screenContainer, screenController);
            AddListners();
        }

        private void AddListners()
        {
            myAccountBtn.onClick.AddListener(sc.OnClickMyAccount);
            logoutBtn.onClick.AddListener(
                () =>
                {
                    SocketRequest.intance.SendEvent(Constant.OnLogout,
                        (res) =>
                        {
                            Debug.Log(res);
                        });
                    sc.OnLoginScreen();
                }
                );
            funTargetTimerGameBtn.onClick.AddListener(
                () =>
                {
                    RegistertionInfo reg = new RegistertionInfo() { gameId = Games.funWheel, playerId = sc.data.Email };
                    Debug.Log(reg);
                    Action<string> OnCurrentRoundInfo = null;
                    if (!isRequestCompleted)
                        SocketRequest.intance.SendEvent(Constant.RegisterPlayer, reg, (json) =>
                        {
                            isRequestCompleted = false;
                            sc.Show(ScreenID.FUN_TARGET_TIMER_GAME_SCREEN, data: (object)json);
                        }, Constant.OnCurrentTimer);
                    isRequestCompleted = true;
                }
                ); 
            
                jeetoJokerTimerGameBtn.onClick.AddListener(
                   () =>
                   {
                       RegistertionInfo reg = new RegistertionInfo() { gameId = Games.funWheel, playerId = sc.data.Email };
                       Debug.Log(reg);
                       Action<string> OnCurrentRoundInfo = null;
                       if (!isRequestCompleted)
                           SocketRequest.intance.SendEvent(Constant.RegisterPlayer, reg, (json) =>
                           {
                               isRequestCompleted = false;
                               sc.Show(ScreenID.JEETO_JOKER_TIMER_GAME_SCREEN, data: (object)json);
                           }, Constant.OnCurrentTimer);
                       isRequestCompleted = true;
                   }
                   );
            
        }

        string balance;
        string userId;
        void UpdateUi()
        {
            chipTxt.text = balance;
            userIdTxt.text = userId;
        }
        bool isRequestCompleted = false;
        public override void Show(object data = null)
        {
            base.Show();
            isRequestCompleted = false;
            object user = new { user_id = sc.data.Email };
            SocketRequest.intance.SendEvent(Constant.OnUserProfile, user, (json) =>
            {
                BackEndData3<PlayerProfile> profile = JsonUtility.FromJson<BackEndData3<PlayerProfile>>(json);
                balance = profile.data.coins.ToString();
                userId = profile.data.user_id.ToUpperInvariant().ToString();
                UpdateUi();
            });


        }

        public override ScreenID ScreenID => ScreenID.HOME_SCREEN;
        protected override string ScreenName => "HomeScreen";
    }
}

public class RegistertionInfo
{
    public Games gameId;
    public string playerId;
}

public class LoginData
{
    public int id;
    public string distributor_id;
    public string user_id;
    public string username;
    public object IMEI_no;
    public string device;
    public string last_logged_in;
    public string last_logged_out;
    public int IsBlocked;
    public string password;
    public string created_at;
    public string updated_at;
    public int active;
    public int coins;
}

[Serializable]
public class PlayerProfile
{
    public int id;
    public string distributor_id;
    public string user_id;
    public string username;
    public object IMEI_no;
    public string device;
    public DateTime last_logged_in;
    public DateTime last_logged_out;
    public int IsBlocked;
    public string password;
    public DateTime created_at;
    public DateTime updated_at;
    public int active;
    public int coins;
}
