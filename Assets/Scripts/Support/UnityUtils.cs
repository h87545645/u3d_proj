
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

}