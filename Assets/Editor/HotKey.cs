using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HotKeyTest
{
    [MenuItem("Assets/HotKey %#d",false,-1)]
    private static void HotKey()
    {
        Debug.Log("Command Shift + D");
    }
}
