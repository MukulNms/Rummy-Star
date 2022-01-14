using Com.BigWin.Frontend.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.BigWin.Frontend
{
    public partial class ScreenController
    {
        public void OpenURL(string url)
        {
            // SampleWebView swv = new SampleWebView { Url = url };
            // lobby.StartCoroutine(swv.Start());
        }

        public void OnClickSignout()
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(0);
            // Application.Quit();

            Show(ScreenID.SPLASH_SCREEN);
        }
    }
}