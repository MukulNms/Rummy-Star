using SocketIO;
using System;
using UnityEngine;
namespace FunTimer.GameSockets
{

    class FunTimerTargertSocket:MonoBehaviour
    {
        [SerializeField] SocketIOComponent socket;

        private void Start()
        {
            print("start");
        }
    }
}
