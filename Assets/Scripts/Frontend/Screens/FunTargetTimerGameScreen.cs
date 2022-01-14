﻿using UnityEngine.UI;
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
using Newtonsoft.Json;
using SocketIO;

namespace Com.BigWin.Frontend
{
    public class FunTargetTimerGameScreen : Screen
    {
        #region Idenfiers
       // [SerializeField] Image headerBulbs;
        [SerializeField] Image timerRingImg;
        [SerializeField] Image baseWheelBulbs;
        [SerializeField] Image LastWinImg;
        [SerializeField] Sprite[] winImages;
        [SerializeField] TextMeshProUGUI winNumber;
        [SerializeField] GameObject Lastbets;

        [SerializeField] Button exitBtn;
        [SerializeField] Button betOkBtn;
        [SerializeField] Button clearBtn;
        [SerializeField] Button doubleBtn;
        //--------------------------------------------
       // [SerializeField] Toggle chipNo1Btn;
       // [SerializeField] Toggle chipNo5Btn;
        [SerializeField] Toggle chipNo10Btn;
        [SerializeField] Toggle chipNo50Btn;
        [SerializeField] Toggle chipNo100Btn;
        [SerializeField] Toggle chipNo200Btn;
        [SerializeField] Toggle chipNo500Btn;
        [SerializeField] Toggle chipNo1000Btn;
        [SerializeField] Toggle chipDeleteBtn;
        //-------------------------------------------------
        [SerializeField] Button zeroBetBtn;
        [SerializeField] Button oneBetBtn;
        [SerializeField] Button twoBetBtn;
        [SerializeField] Button threeBetBtn;
        [SerializeField] Button fourBetBtn;
        [SerializeField] Button fiveBetBtn;
        [SerializeField] Button sixBetBtn;
        [SerializeField] Button sevenBetBtn;
        [SerializeField] Button eightBetBtn;
        [SerializeField] Button nineBetBtn;
        //---------------------------------------------------
        [SerializeField] Text timerText;
        [SerializeField] TextMeshProUGUI comments;
        [SerializeField] TextMeshProUGUI balanceText;
        [SerializeField] TextMeshProUGUI totalBetText;
        [SerializeField] TextMeshProUGUI winnigText;
        [SerializeField] TextMeshProUGUI[] previousWinNOsTxt;//this represent the red boxes
        [SerializeField] TextMeshProUGUI[] previousWinXTxt;//this represent the red boxes
        [SerializeField] TextMeshProUGUI count;

        private float balance;
        private float totalBet;
        private int currentlySectedChip = 10;
        private int previousWinAmount;
        private int section;
        private int lastWinNo;

        private int[] previousBet = new int[10];
        private int[] betHolder = new int[10];
        private int[] previousWins = new int[10];


        private string roundcount;
        private string lastroundcount;
        private string lastWinRoundcount;
        private string[] PrizeName;
        private string isPreviousWinsRecivied;
        private string winningAmount;
        private string currentComment;
        private string userId;
        private string[] commenstArray = {"Bets are Empty!!" ,"For Amusement Only","Bet Accepted!! your bet amount is :"
        ,"Please click on Take","Bets Confirmed"};

        private bool isUserPlacedBets;
        private bool isBetConfirmed;
        private bool canPlaceBet;
        private bool isLastGameWinAmountReceived;
        private bool canPlacedBet;
        private bool isthisisAFirstRound;
        private bool isPreviousBetPlaced;
        private bool isdataLoaded;
        private bool isTimeUp;
        private Color timerRingColor;
        private User user;
        LastRoundWins[] lastRoundWins;

        private int lastWinNumber;

        const int SINGLE_BET_LIMIT = 5000;
        #endregion
        public override void Initialize(Transform screenContainer, ScreenController screenController)
        {
            base.Initialize(screenContainer, screenController);
            roundcount = string.Empty;
            isthisisAFirstRound = true;
            lastRoundWins = new LastRoundWins[10];
            //FindUIReferences();
            AddListners();
            timerRingColor = timerRingImg.color;

            AddSocketListners();
        }
        private void AddListners()
        {
            exitBtn.onClick.AddListener(() =>
            {
                SocketRequest.intance.SendEvent(Constant.onleaveRoom);
                sc.OnClickHome();
            });
            exitBtn.onClick.AddListener(() => ResetAllBets());

            betOkBtn.onClick.AddListener(() =>
            {
                OnBetCalculation();
            });
            clearBtn.onClick.AddListener(() =>
            {
                ResetAllBets();
            });
            doubleBtn.onClick.AddListener(OnClickOnDoubleBetBtn());

            //------------betting no---------------
            oneBetBtn.onClick.AddListener(() => { AddBet(1, oneBetBtn.gameObject); });
            twoBetBtn.onClick.AddListener(() => { AddBet(2, twoBetBtn.gameObject); });
            threeBetBtn.onClick.AddListener(() => { AddBet(3, threeBetBtn.gameObject); });
            fourBetBtn.onClick.AddListener(() => { AddBet(4, fourBetBtn.gameObject); });
            fiveBetBtn.onClick.AddListener(() => { AddBet(5, fiveBetBtn.gameObject); });
            sixBetBtn.onClick.AddListener(() => { AddBet(6, sixBetBtn.gameObject); });
            sevenBetBtn.onClick.AddListener(() => { AddBet(7, sevenBetBtn.gameObject); });
            eightBetBtn.onClick.AddListener(() => { AddBet(8, eightBetBtn.gameObject); });
            nineBetBtn.onClick.AddListener(() => { AddBet(9, nineBetBtn.gameObject); });
            zeroBetBtn.onClick.AddListener(() => { AddBet(0, zeroBetBtn.gameObject); });

            //------------current betting cheap---------------
           // chipNo1Btn.onValueChanged.AddListener((i) => { if (!i) return; currentlySectedChip = 1; DisableToggleBgImage(chipNo1Btn.gameObject); });
          //  chipNo5Btn.onValueChanged.AddListener((i) => { if (!i) return; currentlySectedChip = 5; DisableToggleBgImage(chipNo5Btn.gameObject); });
            chipNo10Btn.onValueChanged.AddListener((i) => { if (!i) return; currentlySectedChip = 10;  DisableToggleBgImage(chipNo10Btn.gameObject); });
            chipNo50Btn.onValueChanged.AddListener((i) => { if (!i) return; currentlySectedChip = 50; DisableToggleBgImage(chipNo50Btn.gameObject); });
            chipNo100Btn.onValueChanged.AddListener((i) => { if (!i) return; currentlySectedChip = 100; DisableToggleBgImage(chipNo100Btn.gameObject); });
            chipNo200Btn.onValueChanged.AddListener((i) => { if (!i) return; currentlySectedChip = 200; DisableToggleBgImage(chipNo200Btn.gameObject); });
            chipNo500Btn.onValueChanged.AddListener((i) => { if (!i) return; currentlySectedChip = 500; DisableToggleBgImage(chipNo500Btn.gameObject); });
            chipNo1000Btn.onValueChanged.AddListener((i) => { if (!i) return; currentlySectedChip = 1000; DisableToggleBgImage(chipNo1000Btn.gameObject); });
            chipDeleteBtn.onValueChanged.AddListener((i) => { if (!i) return; currentlySectedChip = -1; DisableToggleBgImage(chipDeleteBtn.gameObject); });
        }
        public SocketIOComponent socket;
        void AddSocketListners()
        {
            socket.On(Constant.OnWinNo, OnRoundEnd);
            socket.On(Constant.OnTimerStart, OnTimerStart);
            socket.On(Constant.OnDissconnect, (e) => print("dissconnected"));
            socket.On(Constant.OnWinAmount, OnWinAmount);
            socket.On(Constant.OnTimeUp, (e) =>
            {
                Debug.Log("timeup");
                isTimeUp = true;
            });

        }
        void OnWinAmount(SocketIOEvent res)
        {
            //OnWinAmount o = JsonConvert.DeserializeObject<OnWinAmount>(res.data.ToString());
        }
        void OnTimerStart(SocketIOEvent res)
        {
            Debug.Log("timer started");
            //this will get the current timer from the sever unless the timer is 0
            //it not it will wait for it
            StartCoroutine(GetCurrentTimer());
        }
        IEnumerator GetCurrentTimer()
        {
            yield return new WaitUntil(() => currentTime <= 0);
            SocketRequest.intance.SendEvent(Constant.OnTimer, (json) =>
            {
                print("current timer " + json);
                Timer time = JsonConvert.DeserializeObject<Timer>(json);
                StopCoroutine(Timer());
                if (time.result == 0) StartCoroutine(Timer());
                else StartCoroutine(Timer(time.result));
            });
        }
        private void DisableToggleBgImage(GameObject target)
        {
            Debug.Log(">>>>>>>>>> Current Selected Chip >>>> " + currentlySectedChip);
           // chipNo1Btn.transform.GetChild(0).GetComponent<Image>().enabled = true;
           // chipNo5Btn.transform.GetChild(0).GetComponent<Image>().enabled = true;
            chipNo10Btn.transform.GetChild(0).GetComponent<Image>().enabled = true;
            chipNo50Btn.transform.GetChild(0).GetComponent<Image>().enabled = true;
            chipNo100Btn.transform.GetChild(0).GetComponent<Image>().enabled = true;
            chipNo200Btn.transform.GetChild(0).GetComponent<Image>().enabled = true;
            chipNo500Btn.transform.GetChild(0).GetComponent<Image>().enabled = true;
            chipNo1000Btn.transform.GetChild(0).GetComponent<Image>().enabled = true;
            //chipNo5000Btn.transform.GetChild(0).GetComponent<Image>().enabled = true;
            chipDeleteBtn.transform.GetChild(0).GetComponent<Image>().enabled = true;
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
                zeroBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[0].ToString() == "0" ? string.Empty : betHolder[0].ToString();
                oneBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[1].ToString() == "0" ? string.Empty : betHolder[1].ToString();
                twoBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[2].ToString() == "0" ? string.Empty : betHolder[2].ToString();
                threeBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[3].ToString() == "0" ? string.Empty : betHolder[3].ToString();
                fourBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[4].ToString() == "0" ? string.Empty : betHolder[4].ToString();
                fiveBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[5].ToString() == "0" ? string.Empty : betHolder[5].ToString();
                sixBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[6].ToString() == "0" ? string.Empty : betHolder[6].ToString();
                sevenBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[7].ToString() == "0" ? string.Empty : betHolder[7].ToString();
                eightBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[8].ToString() == "0" ? string.Empty : betHolder[8].ToString();
                nineBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[9].ToString() == "0" ? string.Empty : betHolder[9].ToString();
                totalBet = betHolder.Sum();
                SoundManager.instance.PlayClip("addbet");
                UpdateUi();
            };
        }
        private void ResetAllBets()
        {
            for (int i = 0; i < betHolder.Length; i++)
            {
                betHolder[i] = 0;
            }
            oneBetBtn.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            twoBetBtn.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            threeBetBtn.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            fourBetBtn.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            fiveBetBtn.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            sixBetBtn.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            sevenBetBtn.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            eightBetBtn.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            nineBetBtn.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            zeroBetBtn.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            totalBet = 0;
            isUserPlacedBets = false;
            UpdateUi();
        }

        private void AddBet(int betIndex, GameObject btnREf)
        {
            Debug.Log("isdataLoaded " + isdataLoaded);
            if (!isdataLoaded)
            {
                m.print("please wait data to load");
                AndroidToastMsg.ShowAndroidToastMessage("please wait");
                return;
            }
            Debug.Log("canPlaceBet " + canPlaceBet);
            if (!canPlaceBet || isTimeUp) return;
            if (currentlySectedChip == 0)
            {
                m.print("please select a chip first");
                AndroidToastMsg.ShowAndroidToastMessage("please select a chip first");
                return;
            }
            
            if (balance < currentlySectedChip || balance < currentlySectedChip + betHolder.Sum())
            {
                m.print("not enough balanc");
                AndroidToastMsg.ShowAndroidToastMessage("not enough balance");
                Debug.Log("return here ");
                return;
            }
            if (betHolder[betIndex] + currentlySectedChip > SINGLE_BET_LIMIT)
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
            btnREf.transform.GetChild(0).GetComponent<Text>().text = betHolder[betIndex].ToString() == "0" ? string.Empty : betHolder[betIndex].ToString();
            currentComment = commenstArray[1];
            SoundManager.instance.PlayClip("addbet");
            UpdateUi();
        }
        bool isDataLoaded = false;
        public override void Show(object data = null)
        {
            base.Show();
            if (data == null) return;
            LoadDefaultData();
            var o = JsonConvert.DeserializeObject<CurrenRoundInfo>(data.ToString());
            UpdateRoundData(o, true);
            object user = new { playerId = sc.data.Email };
            SocketRequest.intance.SendEvent(Constant.OnPreBet, user, (json) =>
            {

                BackEndData3<PreviousRoundBets> bets = JsonConvert.DeserializeObject<BackEndData3<PreviousRoundBets>>(json);

                if (bets.status != 200) return;
                Debug.Log(bets.status);
                if (bets.data.isCurrRoundBet == 1)
                {
                    betHolder[0] = bets.data.no_0;
                    betHolder[1] = bets.data.no_1;
                    betHolder[2] = bets.data.no_2;
                    betHolder[3] = bets.data.no_3;
                    betHolder[4] = bets.data.no_4;
                    betHolder[5] = bets.data.no_5;
                    betHolder[6] = bets.data.no_6;
                    betHolder[7] = bets.data.no_7;
                    betHolder[8] = bets.data.no_8;
                    betHolder[9] = bets.data.no_9;
                    zeroBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[0].ToString() == "0" ? string.Empty : betHolder[0].ToString();
                    oneBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[1].ToString() == "0" ? string.Empty : betHolder[1].ToString();
                    twoBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[2].ToString() == "0" ? string.Empty : betHolder[2].ToString();
                    threeBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[3].ToString() == "0" ? string.Empty : betHolder[3].ToString();
                    fourBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[4].ToString() == "0" ? string.Empty : betHolder[4].ToString();
                    fiveBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[5].ToString() == "0" ? string.Empty : betHolder[5].ToString();
                    sixBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[6].ToString() == "0" ? string.Empty : betHolder[6].ToString();
                    sevenBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[7].ToString() == "0" ? string.Empty : betHolder[7].ToString();
                    eightBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[8].ToString() == "0" ? string.Empty : betHolder[8].ToString();
                    nineBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[9].ToString() == "0" ? string.Empty : betHolder[9].ToString();
                    totalBet = betHolder.Sum();
                    isUserPlacedBets = true;
                    currentComment = commenstArray[4];
                }

                UpdateUi();
            });
        }
        private void LoadDefaultData()
        {
            StartCoroutine(AnimateBulbs());
            currentComment = commenstArray[1];
            lastwinNo = -1;
            canPlaceBet = false;
            LastWinImg.gameObject.SetActive(false);
            isthisisAFirstRound = true;
            isLastGameWinAmountReceived = true;
            isTimerStarted = false;
            isdataLoaded = false;
            isSomethingWentWrong = false;
            isCurrentBetPlace = false;
            winningAmount = string.Empty;
        }
        bool isTimerStarted;
        private bool isSomethingWentWrong;
        private bool isCurrentBetPlace;
        int lastwinNo;
        int minmumTimeAllowed = 6;
        string tempRoundCount;
        void UpdateRoundData(CurrenRoundInfo curretRoundInfo, bool isFirstARound = false)
        {

            Debug.Log("Update round");
            if (curretRoundInfo.gametimer < minmumTimeAllowed)
            {
                AndroidToastMsg.ShowAndroidToastMessage("Please wait");
                Debug.Log("Please wait");
                currentTime = 0;
                UpdateUi();
                return;
            }
            isdataLoaded = true;
            isTimeUp = false;
            isPreviousBetPlaced = false;
            canPlaceBet = true;
            roundcount = curretRoundInfo.RoundCount.ToString();
            balance = curretRoundInfo.balance;
            totalBet = 0;
            int arrayLimit = curretRoundInfo.previousWinData.Count - 1;
            curretRoundInfo.previousWinData.Reverse();
            for (int i = 0; i <= arrayLimit; i++)
            {
                int number = arrayLimit - i;
                previousWinNOsTxt[number].text = curretRoundInfo.previousWinData[i].winNo.ToString();
                previousWinXTxt[number].text = curretRoundInfo.previousWinData[i].winx.ToString();
            }
            Debug.Log("is a first round " + isFirstARound);
            if (isFirstARound)
            {
                if (tempRoundCount == roundcount)
                {

                }
                int lastroundNo = curretRoundInfo.previousWinData[0].winNo;
                string winx = curretRoundInfo.previousWinData[0].winx;
                StartCoroutine(Timer(curretRoundInfo.gametimer));

#if UNITY_ANDROID
                SpinWheel.instane.SetWheelInitialAngle(lastroundNo, winx);
#else
                SpinWheelWithoutPlugin.instane.SetWheelInitialAngle(lastroundNo, winx);
#endif
            }
            else
            {
                if (previousBet.Sum() > 0)
                    betOkBtn.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Pre";
            }

            currentComment = commenstArray[1];

            if (curretRoundInfo.pendingWinningData != null)
            {
                Debug.Log("won some amount");
                OnWinTheBet(curretRoundInfo.pendingWinningData.win_no);
                winningAmount = curretRoundInfo.pendingWinningData.winPoints.ToString();

            }
            else
            {
                winningAmount = string.Empty;
                Debug.Log("didnot won anything");
            };

            tempRoundCount = roundcount;
            UpdateUi();
        }
        public void SomeThingWentWrong()
        {
            StopAllCoroutines();
            isSomethingWentWrong = true;
            LoadDefaultData();
            balance = 0;
            totalBet = 0;
            previousWinAmount = 0;
            UpdateUi();
            try
            {

                StartNextOrNewRound(true);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
                throw;
            }
            m.print("something went wrong called");
        }
        private void UpdateUi()
        {
            
            balanceText.text = balance.ToString();
            totalBetText.text = totalBet.ToString();
            winnigText.text = winningAmount;
            count.text = "count:" + roundcount;
            comments.text = currentComment;
        }
        public override void Hide()
        {
            base.Hide();
            Debug.Log("hide");
            UpdateUi();
            StopAllCoroutines();
            SpinWheel.instane.ForceFullyStopWheel();
            balance = 0;
            isActive = false;
            lastroundcount = roundcount;
            SoundManager.instance?.PlayClip("clear");
            SocketRequest.intance.SendEvent(Constant.onleaveRoom);
        }
        private void OnBetCalculation()
        {
            if (!isdataLoaded)
            {
                m.print("please wait data to load");
                AndroidToastMsg.ShowAndroidToastMessage("please wait data to load");
                return;
            }
            if (!isLastGameWinAmountReceived)
            {

                currentComment = commenstArray[1];
                SendTakeAmountRequest();
                UpdateUi();
                previousWinAmount = 0;
                return;
            }
            if (previousBet.Sum() != 0 && !isPreviousBetPlaced
                && betOkBtn.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == "Pre")
            {
                PlacePreviousBets();
                betOkBtn.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "BET OK";
                UpdateUi();
                return;
            }
            if (betHolder.Sum() == 0)
            {
                currentComment = commenstArray[0];
                UpdateUi();
                return;
            }
            if (!canPlaceBet) return;
            canPlaceBet = false;
            isUserPlacedBets = true;
            currentComment = commenstArray[2] + betHolder.Sum().ToString();
            betOkBtn.interactable = clearBtn.interactable = doubleBtn.interactable = false;
            Array.Copy(betHolder, previousBet, betHolder.Length);
            SendBets();
            UpdateUi();
        }
        public override void OnConnectionLost()
        {
            base.OnConnectionLost();

        }
        void SendTakeAmountRequest()
        {
            object o = new { playerId = sc.data.Email };
            SocketRequest.intance.SendEvent(Constant.OnTakeAmount, o, (res) =>
             {
                 Debug.Log(res);
                 WinAmountConfirmation winAmountConfirmation = JsonConvert.DeserializeObject<WinAmountConfirmation>(res);
                 if (winAmountConfirmation.status)
                 {
                     LastWinImg.gameObject.SetActive(false);
                     betOkBtn.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "BET OK";
                     canPlaceBet = true;
                     isLastGameWinAmountReceived = true;
                     currentComment = "Amount Successfully Added";
                     balance = winAmountConfirmation.data.balance;
                     winningAmount = string.Empty;

                     if (previousBet.Sum() > 0)
                         betOkBtn.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Pre";
                 }
                 else
                 {
                     currentComment = winAmountConfirmation.message;
                 }

                 UpdateUi();
             });
        }

        /// <summary>
        /// This function just copy the PreviousBetArray into currenBetArray
        /// and updates the UI
        /// </summary>
        private void PlacePreviousBets()
        {
            bool isEnoughBalance = previousBet.Sum() < balance;
            if (!isEnoughBalance)
            {
                m.print("not enough balance");
                AndroidToastMsg.ShowAndroidToastMessage("not enough balance");
                return;
            }
            Array.Copy(previousBet, betHolder, previousBet.Length);

            oneBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[1] == 0 ? string.Empty : betHolder[1].ToString();
            twoBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[2] == 0 ? string.Empty : betHolder[2].ToString();
            threeBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[3] == 0 ? string.Empty : betHolder[3].ToString();
            fourBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[4] == 0 ? string.Empty : betHolder[4].ToString();
            fiveBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[5] == 0 ? string.Empty : betHolder[5].ToString();
            sixBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[6] == 0 ? string.Empty : betHolder[6].ToString();
            sevenBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[7] == 0 ? string.Empty : betHolder[7].ToString();
            eightBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[8] == 0 ? string.Empty : betHolder[8].ToString();
            nineBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[9] == 0 ? string.Empty : betHolder[9].ToString();
            zeroBetBtn.transform.GetChild(0).GetComponent<Text>().text = betHolder[0] == 0 ? string.Empty : betHolder[0].ToString();
            isPreviousBetPlaced = true;
            totalBet = betHolder.Sum();
            UpdateUi();
        }

        int currentTime = 0;
        /// <summary>
        /// This is the 60 sec timer 
        /// </summary>
        /// <param name="counter"></param>
        /// <returns></returns>
        private IEnumerator Timer(int counter = 60) //60
        {
            isTimeUp = false;
            timerRingImg.color = timerRingColor;
            betOkBtn.interactable = clearBtn.interactable = doubleBtn.interactable = true;
            isUserPlacedBets = false;
            Debug.Log("timer started");
            while (counter > 0)
            {
                //if (isSomethingWentWrong) yield break;
                yield return new WaitForSeconds(1f);
                counter--;
                currentTime = counter;
                timerText.text = counter.ToString();
                MonitorBets();
                bool canShowRingAnimation = counter < 20 && counter > 10;
                if (canShowRingAnimation)
                {
                    SoundManager.instance?.PlayClip("countdown");
                    timerRingImg.color = Color.yellow;
                }
                if (counter < 12) isTimeUp = true;

            }

        }

        void MonitorBets()
        {
            bool canPostBetsToServer = isTimeUp || isUserPlacedBets;
            if (canPostBetsToServer)
            {
                if (canPlaceBet)
                {
                    canPlaceBet = false;
                    if (!isUserPlacedBets)
                        SendBets();
                }
                timerRingImg.color = timerRingColor;
                betOkBtn.interactable
                    = clearBtn.interactable
                    = doubleBtn.interactable = false;
            }

        }

        /// <summary>
        /// this will call before 6 sec from the server
        /// </summary>
        /// <param name="res"></param>
        void OnRoundEnd(SocketIOEvent res)
        {
            Debug.Log("on round end");
            if (!this.gameObject.activeInHierarchy) return;
            Debug.Log(res.data);
            Weel o = JsonConvert.DeserializeObject<Weel>(res.data.ToString());
            int no = o.data.win_no;
            string xImg = o.data.winX;
            lastwinNo = no;
           
            StartCoroutine(Spin(no, xImg));
        }
        IEnumerator Spin(int winNo, string xFactorImage)
        {
            yield return new WaitUntil(() => currentTime <= 0);



        #if UNITY_ANDROID

            SpinWheelWithoutPlugin.instane.Spin(winNo);
            SpinWheelWithoutPlugin.instane.onSpinComplete = () =>
            //SpinWheel.instane.SpinTheWheel(winNo, xFactorImage);
            //SpinWheel.instane.onSpinComplete = () =>
            {
                winNumber.text = winNo.ToString();
                StartNextOrNewRound(); };
#else
            SpinWheelWithoutPlugin.instane.Spin(winNo);
            SpinWheelWithoutPlugin.instane.onSpinComplete = () =>
            {  
            winNumber.text = winNo.ToString();
            StartNextOrNewRound();
            };
#endif
        }
        private void StartNextOrNewRound(bool isAFirstRound = false)
        {
            SocketRequest.intance.SendEvent(Constant.OnUserInfo, null, (res) =>
            {
                Debug.Log("new round started");
                Debug.Log(res);
                canPlaceBet = true;
                isBetConfirmed = false;
                isdataLoaded = false;
                currentComment = commenstArray[1];
                UpdateUi();
                var o = JsonConvert.DeserializeObject<CurrenRoundInfo>(res);
                UpdateRoundData(o, isAFirstRound);
            }, Constant.OnCurrentTimer);

            isTimerStarted = false;
            ResetAllBets();
        }
        private void SendBets()
        {
            if (betHolder.Sum() == 0)
            {
                currentComment = commenstArray[0];
                UpdateUi();
                return;
            }
            Debug.Log("user id " + sc.data.Email);

            Bet data = new Bet
            {
                no_0 = betHolder[0],
                no_1 = betHolder[1],
                no_2 = betHolder[2],
                no_3 = betHolder[3],
                no_4 = betHolder[4],
                no_5 = betHolder[5],
                no_6 = betHolder[6],
                no_7 = betHolder[7],
                no_8 = betHolder[8],
                no_9 = betHolder[9],
                gameId = (int)Games.funWheel,
                round_count = long.Parse(roundcount),
                device = SystemInfo.deviceUniqueIdentifier,
                playerId = sc.data.Email,
                points = betHolder.Sum().ToString(),
            };
            PostBet(data);
            canPlaceBet = false;
        }
        private void PostBet(Bet data)
        {

            SocketRequest.intance.SendEvent(Constant.OnPlaceBet, data, (res) =>
            {
                var response = JsonConvert.DeserializeObject<BetConfirmation>(res);
                if (response == null)
                {
                    SomeThingWentWrong();
                    return;
                }
                if (Constant.IS_INVALID_USER == response.status)
                {
                    OnInvalidUser(response.message);
                    return;
                }
                if (response.status == "200") { balance -= betHolder.Sum(); isBetConfirmed = true; }
                currentComment = response.message;
                UpdateUi();
                Debug.Log("is bet placed starus with statu - " + JsonUtility.FromJson<BetConfirmation>(res).status);

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
        private void OnWinTheBet(int winnigNO)
        {
            isLastGameWinAmountReceived = false;
            canPlaceBet = false;
            betOkBtn.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Take";
            currentComment = commenstArray[3];
            m.print("last round won and win no is" + winnigNO);
            //LastWinImg.sprite = winImages[winnigNO];
            
            LastWinImg.gameObject.SetActive(true);
            currentComment = commenstArray[3];
            UpdateUi();
        }

        private void OnApplicationPause(bool pause)
        {
            //when appliction resume
            if (!pause)
            {
                //SomeThingWentWrong();
            }
        }
        public override ScreenID ScreenID => ScreenID.FUN_TARGET_TIMER_GAME_SCREEN;
        protected override string ScreenName => "FunTargetTimerGameScreen";
        // Animation Functions
        private IEnumerator AnimateBulbs()
        {
            bool switchOn = false;
            while (isActive)
            {
                yield return new WaitForSeconds(0.25f);
                if (switchOn)
                {
                   // headerBulbs.color = Color.white;
                    baseWheelBulbs.enabled = true;
                }
                else
                {
                 //   headerBulbs.color = Color.red;
                    baseWheelBulbs.enabled = false;
                }
                switchOn = !switchOn;
            }
        }
    }
}
[Serializable]
public class User
{
    public string user_id;
    public string device;

    public int game;//it the game id
}
[Serializable]
public class LastWinAmountAdder
{
    public string user_id;
    public string device;
    public int game;//it the game id
    public int round_count;//it the game id
}

[Serializable]
public class AddWinAmountResponse
{
    public string message;
    public string status;
    public string coins;
    public string sec;
    public int win_amount;
    public int is_winning_amount_add;
}

[Serializable]
public class LastRoundWins
{
    public int winNo;
    public string winX;
}

[Serializable]
public class RoundCount
{
    public string round_count;
}


namespace FunTargetSocketClasses
{
    public class PreviousWinData
    {
        public double RoundCount;
        public int winNo;
        public string winx;
    }
    [SerializeField]
    public class CurrenRoundInfo
    {
        public int gametimer;
        public float balance;
        public double RoundCount;
        public List<PreviousWinData> previousWinData;
        public OnWinAmount pendingWinningData;
    }


    public class WeelData
    {
        public double RoundCount;
        public int win_no;
        public string winX;
    }

    public class Weel
    {
        public bool status;
        public string message;
        public WeelData data;
    }

    public class Timer
    {
        public int result;//timer
    }
    public class OnWinAmount
    {
        public long RoundCount;
        public int win_no;
        public int winPoints;
    }

    public class WinAmountConfirmationData
    {
        public string playerId;
        public int balance;
    }

    public class WinAmountConfirmation
    {
        public bool status;
        public string message;
        public WinAmountConfirmationData data;
    }

    public class PreviousRoundBets
    {
        public int isCurrRoundBet;
        public int no_0;
        public int no_1;
        public int no_2;
        public int no_3;
        public int no_4;
        public int no_5;
        public int no_6;
        public int no_7;
        public int no_8;
        public int no_9;
    }
}