
using System.Collections;
using UnityEngine;
using System;
public static class UnityUtils
{

    //     public static void editorQueryAssets(string path)
    //     {
    // #if !UNITY_EDITOR
    //         return;
    // #endif

    //     }


    public static IEnumerator DelayFuc(Action action, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        action();
    }

    public static string TimeToStringHMS(float time)
    {
        int hour = (int)time / 3600;
        int minute = ((int)time - hour * 3600) / 60;
        int second = (int)time - hour * 3600 - minute * 60;
        // int millisecond = (int)((time - (int)time ) * 1000);
        string data = string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
        return data;
    }

}