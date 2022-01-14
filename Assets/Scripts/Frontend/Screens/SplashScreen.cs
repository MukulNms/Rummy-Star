using System;
using System.Collections;
using Com.BigWin.Frontend.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Com.BigWin.Frontend
{
    public class SplashScreen : Screen
    {
        private Transform loadingScreen;
        private Slider loadingSlider;
        private TextMeshProUGUI loadingText;

        public override void Initialize(Transform screenContainer, ScreenController screenController)
        {
            base.Initialize(screenContainer, screenController);

            loadingScreen = screenObj.transform.FindRecursive("LoadingScreen");
            loadingSlider = screenObj.transform.FindRecursive("LoadingSlider").GetComponent<Slider>();
            loadingText = screenObj.transform.FindRecursive("LoadingText").GetComponent<TextMeshProUGUI>();

        }

        public override void Show(object data = null)
        {
            base.Show(data);
            coroutineRunner.StartCoroutine(CallFuntionWithDelay(2, OnSplashTimeComplete));
        }
        public override void Hide()
        {
            base.Hide();
        }
        public override ScreenID ScreenID => ScreenID.SPLASH_SCREEN;

        protected override string ScreenName => "SplashScreen";

        private void OnSplashTimeComplete()
        {
            loadingScreen.gameObject.SetActive(true);
            coroutineRunner.StartCoroutine(CallFuntionWithDelay(3, OnLoadTimeComplete));
        }

        private void OnLoadTimeComplete()
        {
            //     string email = screenController.dataStorageController.Email;
            //     string password = screenController.dataStorageController.Password;

            // if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password))
            // {
            //     screenController.OnResumeLogin();
            // }
            // else
            // {
            loadingScreen.gameObject.SetActive(false);
            sc.OnLoginScreen();
            // }
        }

#if UNITY_EDITOR
        private IEnumerator QuickLoad(Action action)
        {
            yield return null;
            action();
        }
#endif

        protected override IEnumerator CallFuntionWithDelay(float delay, Action action)
        {
            float t = 0;
            while (t < delay)
            {
                t += Time.deltaTime;
                if (loadingText != null) loadingText.text = "Loading " + (int)(t / delay * 100) + "%";
                loadingSlider.value = (int)(t / delay * 100);
                yield return null;
            }
            if (loadingText != null) loadingText.text = "Loading 100%";
            yield return new WaitForSeconds(1);
            action();
        }
    }
}