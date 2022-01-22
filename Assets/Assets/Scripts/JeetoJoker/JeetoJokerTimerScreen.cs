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
        [SerializeField] Button betOkBtn;
        [SerializeField] Button clearBtn;
        [SerializeField] Button doubleBtn;

        private int currentlySectedChip = 10;
        private bool isUserPlacedBets;
        private int[] betHolder = new int[10];
        private bool isdataLoaded;
        private bool isPreviousBetPlaced;
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

            betOkBtn.onClick.AddListener(() =>
            {
                
                OnBetCalculation();
            });

            clearBtn.onClick.AddListener(() =>
            {
                ResetAllBets();
            });
            doubleBtn.onClick.AddListener(OnClickOnDoubleBetBtn());

            heartJackBtn.onClick.AddListener(() => { AddBet(1, heartJackBtn.gameObject); });
            spadeJackBtn.onClick.AddListener(() => { AddBet(2, spadeJackBtn.gameObject); });
            diamondJackBtn.onClick.AddListener(() => { AddBet(3,diamondJackBtn.gameObject); });
            clubJackBtn.onClick.AddListener(() =>  { AddBet(4,  clubJackBtn.gameObject); });
            heartQueenBtn.onClick.AddListener(() => { AddBet(5, heartQueenBtn.gameObject); });
            spadeQueenBtn.onClick.AddListener(() => { AddBet(6, spadeQueenBtn.gameObject); });
            diamondQueenBtn.onClick.AddListener(() => { AddBet(7,diamondQueenBtn.gameObject); });
            clubQueenBtn.onClick.AddListener(() => { AddBet(8, clubQueenBtn.gameObject); });
            heartKingBtn.onClick.AddListener(() => { AddBet(9, heartKingBtn.gameObject); });
            spadeKingBtn.onClick.AddListener(() => { AddBet(10, spadeKingBtn.gameObject); });
            diamondKingBtn.onClick.AddListener(() => { AddBet(11, diamondKingBtn.gameObject); });
            clubKingBtn.onClick.AddListener(() => { AddBet(12, clubKingBtn.gameObject); });

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

            if (currentlySectedChip == -1)//this is for delete chip btn
            {
                if (betHolder[betIndex] == 0) return;
                betHolder[betIndex] = 0;
            }
            else
            {
                betHolder[betIndex] += currentlySectedChip;
            }
            betOkBtn.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "BET OK";
            clearBtn.interactable = true;
            isPreviousBetPlaced = true;
            totalBet = betHolder.Sum();
           // btnREf.transform.GetChild(0).GetComponent<Text>().text = betHolder[betIndex].ToString() == "0" ? string.Empty : betHolder[betIndex].ToString();
           // currentComment = commenstArray[1];
            SoundManager.instance.PlayClip("addbet");
            
        }

        public void OnBetBtnClick()
        {
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

        private UnityAction OnClickOnDoubleBetBtn()
        {
            return () =>
            {
                if (!isdataLoaded)
                {
                    m.print("please wait data to load");
                    AndroidToastMsg.ShowAndroidToastMessage("please wait data to load");
                    return;
                }
                if (betHolder.Sum() == 0)
                {

                    m.print("no bet placed yet");
                    AndroidToastMsg.ShowAndroidToastMessage("no bet placed yet");
                    return;
                }
                bool isEnoughBalance = balance > betHolder.Sum() * 2;

                if (!isEnoughBalance)
                {
                    m.print("not enough balance");
                    AndroidToastMsg.ShowAndroidToastMessage("not enough balance");
                    return;
                }

                bool isRichedTheLimit = betHolder.Sum() * 2 > SINGLE_BET_LIMIT;

                if (isRichedTheLimit)
                {
                    m.print("reached the limit");
                    AndroidToastMsg.ShowAndroidToastMessage("reached the limit");
                    return;
                }

                for (int i = 0; i < betHolder.Length; i++)
                {
                    betHolder[i] *= 2;
                }
                heartJackBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[0].ToString() == "0" ? string.Empty : betHolder[0].ToString();
                spadeJackBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[1].ToString() == "0" ? string.Empty : betHolder[1].ToString();
                diamondJackBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[2].ToString() == "0" ? string.Empty : betHolder[2].ToString();
                clubJackBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[3].ToString() == "0" ? string.Empty : betHolder[3].ToString();
                heartQueenBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[4].ToString() == "0" ? string.Empty : betHolder[4].ToString();
                spadeQueenBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[5].ToString() == "0" ? string.Empty : betHolder[5].ToString();
                diamondQueenBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[6].ToString() == "0" ? string.Empty : betHolder[6].ToString();
                clubQueenBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[7].ToString() == "0" ? string.Empty : betHolder[7].ToString();
                heartKingBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[8].ToString() == "0" ? string.Empty : betHolder[8].ToString();
                spadeKingBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[9].ToString() == "0" ? string.Empty : betHolder[9].ToString();
                diamondKingBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[10].ToString() == "0" ? string.Empty : betHolder[10].ToString();
                clubKingBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[11].ToString() == "0" ? string.Empty : betHolder[11].ToString();
                totalBet = betHolder.Sum();
                SoundManager.instance.PlayClip("addbet");
                
            };
        }
        private void ResetAllBets()
        {
            for (int i = 0; i < betHolder.Length; i++)
            {
                betHolder[i] = 0;
            }
            heartJackBtn.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            spadeJackBtn.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            diamondJackBtn.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            clubJackBtn.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            heartQueenBtn.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            spadeQueenBtn.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            diamondQueenBtn.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            clubQueenBtn.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            heartKingBtn.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            spadeKingBtn.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            diamondKingBtn.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            clubKingBtn.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            totalBet = 0;
            isUserPlacedBets = false;
            
        }

        private void OnBetCalculation()
        {
            if (!isdataLoaded)
            {
                m.print("please wait data to load");
                AndroidToastMsg.ShowAndroidToastMessage("please wait data to load");
                return;
            }

           
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
            betOkBtn.interactable = clearBtn.interactable = doubleBtn.interactable = true;
            isUserPlacedBets = false;
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
