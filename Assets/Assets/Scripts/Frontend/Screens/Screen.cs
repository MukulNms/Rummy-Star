using Com.BigWin.Frontend.Data;
using Com.BigWin.WebUtils;
using System;
using System.Collections;
using UnityEngine;

namespace Com.BigWin.Frontend
{
    public abstract class Screen:MonoBehaviour
    {
        protected GameObject screenObj;
        protected ScreenController sc;
        protected WebRequestHandler webRequestHandler;
        protected CoroutineRunner coroutineRunner;
        protected GenricDialogue dialogue;

        private Transform dialogueContainer;
        protected bool isActive;//check the screen if currently active

        public virtual void  Initialize(Transform screenParent, ScreenController screenController)
        {
            Debug.Log("Screen Name : " + ScreenName);
            // screenContainer is Canvas
            screenObj = screenParent.FindRecursive(ScreenName).gameObject;
            this.sc = screenController;

            webRequestHandler = screenParent.GetComponent<WebRequestHandler>();
            coroutineRunner = screenParent.GetComponent<CoroutineRunner>();
            dialogue = GenricDialogue.intance;
        }

        protected abstract string ScreenName { get; }

        public abstract ScreenID ScreenID { get; }

        public virtual void Show(object data=null)
        {
            ActivateScreen(true);
            isActive = true;
            screenObj.transform.SetAsLastSibling();
        }

        public virtual void Back()
        {
            isActive = false;
            sc.Back();
        }
        public virtual void OnConnectionLost()
        {
            StopAllCoroutines();   
        }

        public virtual void Hide()
        {
            isActive = false;
            ActivateScreen(false);
        }

        public virtual void ActivateScreen(bool state)
        {
            screenObj.SetActive(state);
        }

        protected virtual IEnumerator CallFuntionWithDelay(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action();
        }

        public Action OnStartGame;
    }
}