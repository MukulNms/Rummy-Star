using Com.BigWin.Frontend.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.BigWin.Frontend
{
    public partial class ScreenController
    {
        private Stack<ScreenID> screensStack;
        private ScreenID lastSubScreenID, currentSubScreenId;
        private ScreenID currentScreenId, lastScreenId;
        private Dictionary<ScreenID, Screen> screensCollection;
        private Transform screensContainer;
        public DataStorageController data;

        public ScreenController(Transform screensContainer)
        {
            this.screensContainer = screensContainer;
            screensStack = new Stack<ScreenID>();
            lastSubScreenID = ScreenID.NONE;
            screensCollection = new Dictionary<ScreenID, Screen>();
            data = new DataStorageController(screensContainer);
            LoadScreens(screensContainer.gameObject);
            Server.FuntargetTimer.ServerResponse.intance.OnForceExit = () =>
            {

                screensCollection[currentScreenId].OnConnectionLost();
                GenricDialogue.intance.Show("Connection Lost",true,"Logout");
                GenricDialogue.intance.OnDialogHide = () => {
                    Show(ScreenID.LOGIN_SCREEN);
                    // SocketRequest.intance.SendEvent(Constant.OnLogout);
                    // SocketRequest.intance.Disconnect();
                    //SceneManager.LoadScene(0);

                };
            };
            //Show(ScreenID.LOGIN_SCREEN);
            Show(ScreenID.SPLASH_SCREEN);
            //GenricDialogue.intance.Show("Connection Lost", true, "Logout");
            //GenricDialogue.intance.Show("Connection Lost", true, "Logout");
            //GenricDialogue.intance.OnDialogHide = () => Show(ScreenID.LOGIN_SCREEN);
        }

        private void LoadScreens(GameObject screens)
        {
            Screen[] s = screens.GetComponentsInChildren<Screen>(true);
            foreach (Screen screen in s)
            {
                screen.Initialize(screens.transform, this);
                screen.gameObject.SetActive(true);
                screensCollection.Add(screen.ScreenID, screen);
                screen.gameObject.SetActive(false);
            }
        }
       
        private void PrintAllScreens()
        {
            foreach (KeyValuePair<ScreenID, Screen> pair in screensCollection)
                Debug.Log(pair.Key + " , " + pair.Value);
        }

        public void Show(ScreenID id, bool ShowAsSubScreen = false,object data=null)
        {
            if (ShowAsSubScreen)
            {
                lastSubScreenID = currentSubScreenId;
                currentSubScreenId = id;
            }
            else
            {
                lastScreenId = currentScreenId;
                currentScreenId = id;
            }
            Push(id, ShowAsSubScreen);
            Hide(ShowAsSubScreen);
            screensCollection[id].Show(data);
        }

        public ScreenID Hide(bool subScreen = false)
        {
            if (!subScreen)
            {
                // Normal screens
                if (screensStack.Count > 0)
                {
                    ScreenID screenID = Pop();
                    screensCollection[lastScreenId].Hide();
                    return screenID;
                }
            }
            else
            {
                // Child Screens under Screens
                if (lastSubScreenID != ScreenID.NONE)
                {
                    screensCollection[lastSubScreenID].Hide();
                    lastSubScreenID = ScreenID.NONE;
                }
            }
            return ScreenID.NONE;
        }

        public void Back()
        {
            ScreenID currentScreenID = Hide();
            ScreenID lastScreenID = currentScreenID - 1;
            Push(lastScreenID);
            screensCollection[lastScreenID].ActivateScreen(true);
        }

        private void Push(ScreenID screenID, bool subScreen = false)
        {
            if (!subScreen) screensStack.Push(screenID);
            else lastScreenId = screenID;
        }

        private ScreenID Pop()
        {
            return screensStack.Pop();
        }

        public ScreenID Peek()
        {
            return screensStack.Peek();
        }
    }
}