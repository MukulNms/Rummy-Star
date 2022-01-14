using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    AndroidJavaClass unityClass;
    AndroidJavaObject unityActivity;
    AndroidJavaClass customClass;

    private void Awake()
    {

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;
    }

    void sendActivityReference(string packageName)
    {
        unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
        Debug.Log($"RecieveActivityInstance: {unityActivity.ToString()}");
        customClass = new AndroidJavaClass(packageName);
        customClass.CallStatic("RecieveActivityInstance", unityActivity);
    }

    void startService()
    {
        customClass.CallStatic("StartMyService");
    }
}
