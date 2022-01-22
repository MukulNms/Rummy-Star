using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Com.BigWin.Frontend.Data;
using Newtonsoft.Json;
using Receivable;
using m = UnityEngine.MonoBehaviour;
using Assets.Scripts.Frontend.Utils;
using System.Collections.Generic;

namespace Com.BigWin.Frontend
{
    public class TransferablesScreen : Screen
    {
        private GameObject receivablePrefab;
        private GameObject content;
        private TextMeshProUGUI mainBalance;
        private string user_id;
        [ReadOnly] public bool isDataLoaded;
        [SerializeField] private Button backBtn;
        public override void Initialize(Transform screenContainer, ScreenController screenController)
        {
            base.Initialize(screenContainer, screenController);
            FindUIReferences();
            AddListners();
        }

        private void FindUIReferences()
        {

            mainBalance = screenObj.transform.parent.transform.FindRecursive("mainBalance").GetComponent<TextMeshProUGUI>();
            receivablePrefab = Resources.Load($"prefab/TransferablePrefab") as GameObject;
            content = screenObj.transform.FindRecursive("Content").transform.gameObject;
            backBtn.onClick.AddListener(() => { isDataLoaded = false; content.transform.DestroyAllChildren(); });
        }

        private void AddListners()
        {
        }
        public override void Show(object data = null)
        {
            base.Show();
            user_id = sc.data.Email;
            if (isDataLoaded) return;
            object user = new { user_id = sc.data.Email };

            SocketRequest.intance.SendEvent(Constant.OnsenderNotification, user, (res) =>
             {
                 BackEndData<ReceivableData> receivable = JsonConvert.DeserializeObject<BackEndData<ReceivableData>>(res);
                 GetReceivableData(receivable.data, receivable.status);
             });
            foreach (var item in prefabs)
            {
                Destroy(item);
            }
            prefabs.Clear();
            UpdateBalane();
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
        public void GetReceivableData(ReceivableData data, string status)
        {

            isDataLoaded = true;
            content.transform.DestroyAllChildren();
            for (int i = 0; i < data.notification_count; i++)
            {
                GameObject clone = m.Instantiate(receivablePrefab, parent: content.transform);
                prefabs.Add(clone);
                ReceivablePrefab trasferableClone = clone.GetComponent<ReceivablePrefab>();
                string from = data.notification[i].sender;
                string to = data.notification[i].reciever;
                string amount = data.notification[i].points.ToString();
                string date = data.notification[i].created_at.ToString();
                string noti_id = data.notification[i].id.ToString();
                trasferableClone.SetData(from, to, amount, date);

                trasferableClone.Reject.onClick.AddListener(() =>
                {
                    object accept = new { notify_id = noti_id, user_id = sc.data.Email };

                    SocketRequest.intance.SendEvent(Constant.OnRejectPoints, accept, (res) =>
                    {
                        Debug.Log(res);
                        var repo = JsonConvert.DeserializeObject<Status>(res);
                        if (repo.status == 200)
                        {
                            UpdateBalane();
                            m.Destroy(clone, 1f);
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
        void UpdateBalane()
        {
            object user = new { user_id = sc.data.Email };
            SocketRequest.intance.SendEvent(Constant.OnUserProfile, user, (json) =>
            {
                Debug.Log(json);
                BackEndData3<PlayerProfile> profile = JsonUtility.FromJson<BackEndData3<PlayerProfile>>(json);
                mainBalance.text = profile.data.coins.ToString();
            });
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
        public override ScreenID ScreenID => ScreenID.TRANSFERABLES_SCREEN;
        protected override string ScreenName => "TransferablesScreen";
    }
}
[SerializeField]
public class TReject
{
    public string reciever;
    public string sender;
    public string user_id;
    public string notify_id;
}