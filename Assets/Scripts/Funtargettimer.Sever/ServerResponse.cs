using Com.BigWin.Frontend.Data;
using SocketIO;
using System;
using UnityEngine;

namespace Server.FuntargetTimer
{
    class ServerResponse : MonoBehaviour
    {
        public SocketIOComponent socket;
        public static ServerResponse  intance;
        public Action OnForceExit;
        void Awake()
        {
            intance = this;
            socket.On("open", OnConnected);
            socket.On(Constant.OnDissconnect, OnDissconnect);
            socket.On("OnForceExit", OnForceExist);
        }
        void OnConnected(SocketIOEvent e)
        {
            print("connected");
        }  void OnDissconnect(SocketIOEvent e)
        {
            OnForceExit?.Invoke();
            print("dissconnect");
        }

        void CurrentRoundInfo(SocketIOEvent e)
        {
            Debug.Log("current round info"+e.data);
        }
        void OnBetPlaced(SocketIOEvent e)
        {
            Debug.Log("OnBetPlaced " + e.data);
        } 
        void OnWinAmount(SocketIOEvent e)
        {
            Debug.Log("OnWinAmount " + e.data);
        }
        void OnForceExist(SocketIOEvent e)
        {
            Debug.Log("someone connected from different location");
            Debug.Log("force exit");

            SocketRequest.intance.Disconnect();
            OnForceExit?.Invoke();
        }
    }
}
