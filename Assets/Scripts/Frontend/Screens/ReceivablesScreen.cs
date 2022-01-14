using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Com.BigWin.Frontend.Data;
using System;
using Unity.Jobs;
using Unity.Collections;
using Receivable;
using System.Reflection;
using Newtonsoft.Json;
using m = UnityEngine.MonoBehaviour;
using System.Collections.Generic;

namespace Com.BigWin.Frontend
{
    public class ReceivablesScreen : Screen
    {
        private GameObject receivablePrefab;
        private GameObject content;
        private TextMeshProUGUI mainBalance;
        [ReadOnly] private bool isDataLoaded;
        private string user_id;
        [SerializeField] private Button backBtn;
        public override void Initialize(Transform screenContainer, ScreenController screenController)
        {
            base.Initialize(screenContainer, screenController);
            FindUIReferences();
            AddListners();
        }

        private void FindUIReferences()
        {
            receivablePrefab = Resources.Load($"prefab/ReceivablePrefab") as GameObject;
            content = screenObj.transform.FindRecursive("Content").transform.gameObject;
            mainBalance = screenObj.transform.parent.transform.FindRecursive("mainBalance").GetComponent<TextMeshProUGUI>();

        }

        private void AddListners()
        {

            backBtn.onClick.AddListener(() => { isDataLoaded = false; content.transform.DestroyAllChildren(); });
        }

        public override void Show(object data = null)
        {
            base.Show();
            if (isDataLoaded) return;
            user_id = sc.data.Email;
            object user = new { user_id  };
            SocketRequest.intance.SendEvent(Constant.OnNotification,user, (res) =>
            {
                BackEndData<ReceivableData> receivable = JsonConvert.DeserializeObject<BackEndData<ReceivableData>>(res);
                GetReceivableData(receivable.data,receivable.status);
            });

            foreach (var item in prefabs)
            {
                Destroy(item);
            }
            prefabs.Clear();
            UpdateBalane();   //webRequestHandler.Post(Constant.RECEIVABLES_URL, JsonConvert.SerializeObject(user), GetReceivableData);
        }
        List<GameObject> prefabs = new List<GameObject>();
        public override void Hide()
        {
            base.Hide();
            isDataLoaded = false;
            foreach (var item in prefabs)
            {
                Destroy(item);
            }
            prefabs.Clear();
        }
        public void GetReceivableData(ReceivableData data,string status)
        {
            try
            {

               
                if ( status != "200")
                {
                    AndroidToastMsg.ShowAndroidDefaltMessage();
                    m.print("something went wrong while receiving data");
                    return;
                }
                print("nof " + data.notification_count);

                if (data.notification_count == 0) return;
                isDataLoaded = true;
                if (status == Constant.IS_INVALID_USER)
                {
                    //OnInvalidUser(data.message);
                    return;
                }
                content.transform.DestroyAllChildren();
                for (int i = 0; i < data.notification_count; i++)
                {
                    GameObject clone = m.Instantiate(receivablePrefab, parent: content.transform);
                    ReceivablePrefab receivableclone = clone.GetComponent<ReceivablePrefab>();
                    string from = data.notification[i].sender;
                    string to = data.notification[i].reciever;
                    string amount = data.notification[i].points.ToString();
                    string date = data.notification[i].created_at.ToString();
                    string noti_id = data.notification[i].id.ToString();
                    receivableclone.SetData(from, to, amount, date);
                    receivableclone.Accept.onClick.AddListener(() =>
                    {
                        object accept = new  { notify_id = noti_id, user_id = user_id,};
                        SocketRequest.intance.SendEvent(Constant.OnAcceptPoints, accept, (res) =>
                        {
                            Debug.Log(res);
                            var repo = JsonConvert.DeserializeObject<Status>(res);
                            //AndroidToastMsg.ShowAndroidToastMessage(repo.message);
                            if (repo.status == 200)
                            {
                                UpdateBalane();
                                m.Destroy(clone, 1f);
                                //OnSuccessfullyAcceptedOrRejected();
                            }
                           
                            string msg = string.IsNullOrEmpty(repo.message) ? repo.error : repo.message;
                            if (Application.platform != RuntimePlatform.Android)
                            {
                                dialogue.Show(msg);
                            }
                            else
                            {
                                AndroidToastMsg.ShowAndroidToastMessage(msg);
                            }
                        });
                    });
                    receivableclone.Reject.onClick.AddListener(() =>
                    {
                        object accept = new  { notify_id = noti_id, user_id = user_id,};
                        SocketRequest.intance.SendEvent(Constant.OnRejectPoints, accept, (res) =>
                        {
                            Debug.Log(res);
                            var repo = JsonConvert.DeserializeObject<Status>(res);
                            if (repo.status == 200)
                            {
                                UpdateBalane();
                                m.Destroy(clone, 1f);
                               //OnSuccessfullyAcceptedOrRejected();
                            }
                            string msg = string.IsNullOrEmpty(repo.message) ? repo.error : repo.message;
                            if (Application.platform != RuntimePlatform.Android)
                            {
                                dialogue.Show(msg);
                            }
                            else
                            {
                                AndroidToastMsg.ShowAndroidToastMessage(msg);
                            }
                        });
                    });
                    clone.SetActive(true);
                }
            }
            catch
            {

            }
        }

        void UpdateBalane()
        {
            object user = new { user_id = sc.data.Email };
            SocketRequest.intance.SendEvent(Constant.OnUserProfile, user, (json) =>
            {
               
                BackEndData3<PlayerProfile> profile = JsonUtility.FromJson<BackEndData3<PlayerProfile>>(json);
                mainBalance.text = profile.data.coins.ToString();
            });
        }

        private void OnInvalidUser(string msg)
        {
            m.print("invalid user");
            dialogue.Show(msg, okButtonMsg: "Logout");
            dialogue.OnDialogHide = () =>
            {
                sc.OnLoginScreen();
            };
        }
        private void OnSuccessfullyAcceptedOrRejected()
        {
            string username = sc.data.Email;
            string password = sc.data.Password;
            LoginForm form = new LoginForm() { user_id = username, password = password, imei = "1321365464987", device = SystemInfo.deviceUniqueIdentifier };

            webRequestHandler.Post(Constant.PROFILE_URL, JsonUtility.ToJson(form), OnLoginRequestProcessed);
        }

        private void OnLoginRequestProcessed(string json, bool success)
        {
            Profile profliedata = JsonUtility.FromJson<Profile>(json);

            if (profliedata.user_data.status == "200")
            {
                mainBalance.text = profliedata.user_data.points.ToString();
                sc.data.LoginResponseData.user_data.coins = profliedata.user_data.points.ToString();

            }
            else
            {
                AndroidToastMsg.ShowAndroidDefaltMessage();
                m.print("something went wrong");
            }
        }
        public override ScreenID ScreenID => ScreenID.RECEIVABLES_SCREEN;
        protected override string ScreenName => "ReceivablesScreen";
    }
}

[Serializable]
public class Status
{
    public int status;
    public string message;
    public string error;
}

