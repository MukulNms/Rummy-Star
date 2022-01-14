using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Com.BigWin.Frontend.Data;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using Assets.Scripts.Games.FunTarget.ApiData;
using CurrentRoundNameSpace;
using BetNameSpace;
using LastBetNameSpace;
using m = UnityEngine.MonoBehaviour;
using UnityEngine.Events;
using WinNampSpcae;
using FunTargetSocketClasses;



namespace Com.BigWin.Frontend
{
    
    public class JeetoJokerTimerScreen : Screen
    {


        [SerializeField] Toggle chipNo10Btn;
        [SerializeField] Toggle chipNo50Btn;
        [SerializeField] Toggle chipNo100Btn;
        [SerializeField] Toggle chipNo200Btn;
        [SerializeField] Toggle chipNo500Btn;
        


        [SerializeField] Button heartJackBtn;
        [SerializeField] Button spadeJackBtn;
        [SerializeField] Button diamondJackBtn;
        [SerializeField] Button clubJackBtn;
        [SerializeField] Button heartQueenBtn;
        [SerializeField] Button spadeQueenBtn;
        [SerializeField] Button diamondQueenBtn;
        [SerializeField] Button clubQueenBtn;
        [SerializeField] Button heartKingBtn;
        [SerializeField] Button spadeKingBtn;
        [SerializeField] Button diamondKingBtn;
        [SerializeField] Button clubKingBtn;

        [SerializeField] Button exitBtn;
        [SerializeField] Text timerText;



        private int[] betHolder = new int[10];
        
        private float totalBet;
        private float balance;
        private bool isDataLoaded;
        private bool canPlaceBet;
        private bool isTimeUp;
        private int currentlySelectedChip = 10;

        const int SINGLE_BET_LIMIT = 500;

       
        public override void Initialize(Transform screenContainer, ScreenController screenController)
        {
            base.Initialize(screenContainer, screenController);
            AddListners();
        }

        private void AddListners()
        {
            exitBtn.onClick.AddListener(() =>
            {
                SocketRequest.intance.SendEvent(Constant.onleaveRoom);
                sc.OnClickHome();
            });

            heartJackBtn.onClick.AddListener(() => { AddBet(1, heartJackBtn.gameObject); });
            spadeJackBtn.onClick.AddListener(() => { AddBet(1, spadeJackBtn.gameObject); });
            diamondJackBtn.onClick.AddListener(() => { AddBet(1,diamondJackBtn.gameObject); });
            clubJackBtn.onClick.AddListener(() =>  { AddBet(1,  clubJackBtn.gameObject); });
            heartQueenBtn.onClick.AddListener(() => { AddBet(1, heartQueenBtn.gameObject); });
            spadeQueenBtn.onClick.AddListener(() => { AddBet(1, spadeQueenBtn.gameObject); });
            diamondQueenBtn.onClick.AddListener(() => { AddBet(1,diamondQueenBtn.gameObject); });
            clubQueenBtn.onClick.AddListener(() => { AddBet(1, clubQueenBtn.gameObject); });
            heartKingBtn.onClick.AddListener(() => { AddBet(1, heartKingBtn.gameObject); });
            spadeKingBtn.onClick.AddListener(() => { AddBet(1, spadeKingBtn.gameObject); });
            diamondKingBtn.onClick.AddListener(() => { AddBet(1, diamondKingBtn.gameObject); });
            clubKingBtn.onClick.AddListener(() => { AddBet(1, clubKingBtn.gameObject); });

            chipNo10Btn.onValueChanged.AddListener((i) => { if (!i) return; currentlySelectedChip = 10; DisableToggleBgImage(chipNo10Btn.gameObject); });
            chipNo50Btn.onValueChanged.AddListener((i) => { if (!i) return; currentlySelectedChip = 50; DisableToggleBgImage(chipNo50Btn.gameObject); });
            chipNo100Btn.onValueChanged.AddListener((i) => { if (!i) return; currentlySelectedChip = 100; DisableToggleBgImage(chipNo100Btn.gameObject); });
            chipNo200Btn.onValueChanged.AddListener((i) => { if (!i) return; currentlySelectedChip = 200; DisableToggleBgImage(chipNo200Btn.gameObject); });
            chipNo500Btn.onValueChanged.AddListener((i) => { if (!i) return; currentlySelectedChip = 500; DisableToggleBgImage(chipNo500Btn.gameObject); });

            
            
        }

        private void AddBet(int betIndex, GameObject btnRef)
        {
            Debug.Log("isdataLoaded " + isDataLoaded);
            if (!isDataLoaded)
            {
                m.print("please wait data to load");
                AndroidToastMsg.ShowAndroidToastMessage("please wait");
                return;
            }
            Debug.Log("canPlaceBet " + canPlaceBet);
            if (!canPlaceBet || isTimeUp) return;
            if (currentlySelectedChip == 0)
            {
                m.print("please select a chip first");
                AndroidToastMsg.ShowAndroidToastMessage("please select a chip first");
                return;
            }

            if (balance < currentlySelectedChip || balance < currentlySelectedChip + betHolder.Sum())
            {
                m.print("not enough balanc");
                AndroidToastMsg.ShowAndroidToastMessage("not enough balance");
                Debug.Log("return here ");
                return;
            }
            if (betHolder[betIndex] + currentlySelectedChip> SINGLE_BET_LIMIT)
            {
                m.print("reached the limit");
                AndroidToastMsg.ShowAndroidToastMessage("reached the limit");
                return;
            }
            

            WheelRotation.instance.spin_button();
        }

        private void DisableToggleBgImage(GameObject target)
        {
            Debug.Log(">>>>>>>>>> Current Selected Chip >>>> " + currentlySelectedChip);
            // chipNo1Btn.transform.GetChild(0).GetComponent<Image>().enabled = true;
            // chipNo5Btn.transform.GetChild(0).GetComponent<Image>().enabled = true;
            chipNo10Btn.transform.GetChild(0).GetComponent<Image>().enabled = true;
            chipNo50Btn.transform.GetChild(0).GetComponent<Image>().enabled = true;
            chipNo100Btn.transform.GetChild(0).GetComponent<Image>().enabled = true;
            chipNo200Btn.transform.GetChild(0).GetComponent<Image>().enabled = true;
            chipNo500Btn.transform.GetChild(0).GetComponent<Image>().enabled = true;
            
            
            
            target.transform.GetChild(0).GetComponent<Image>().enabled = false;
        }

        public override ScreenID ScreenID => ScreenID.JEETO_JOKER_TIMER_GAME_SCREEN;

        protected override string ScreenName => "JeetoJokerTimerScreen";

        private void Start()
        {
            StartCoroutine(StartCountdown());
        }

        int currentTime = 0;
        /// <summary>
        /// This is the 60 sec timer 
        /// </summary>
        /// <param name="counter"></param>
        /// <returns></returns>
        public IEnumerator StartCountdown(int counter= 60)
        {
          
            while (counter > 0)
            {
                yield return new WaitForSeconds(1f);
                counter--;
                currentTime = counter;
                timerText.text = counter.ToString();

            }
        }

    }


}
