// WARNING:: DON'T DARE TO DELETE THIS SCRIPT OR MESS UP WITHOUT KNOWLEDGE !!
using Com.BigWin.Frontend.Data;
using UnityEngine;

namespace Com.BigWin.Frontend
{
    // THIS SCRIPT HAS TO BE PUT ON THE CANVAS CONPONENT IN HIERARCHY
    public class ScreenControllerManager : MonoBehaviour
    {
        private ScreenController screenController;
        private SplashScreenData SplashScreenData;
        void Awake()
        {
            screenController = new ScreenController(transform);
            SplashScreenData = new SplashScreenData();
            
        }

        private void Start()
        {
        }
    }
}