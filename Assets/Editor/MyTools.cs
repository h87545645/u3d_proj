using UnityEditor;
using UnityEngine;

public class MyTools
{
    //[MenuItem("Assets/My Tools/Tools 1", false, 2)]
    //static void MyTools1()
    //{
    //    Debug.Log(Selection.activeObject.name);
    //}
    //[MenuItem("Assets/My Tools/Tools 2", false, 2)]
    //static void MyTools2()
    //{
    //    Debug.Log(Selection.activeObject.name);
    //}


    [MenuItem("Tools/My Tools/AssetLoadTest", false, 1)]
    static void AssetLoadTest()
    {
        PrefabLoadMgr.I.LoadSync("player.prefab", GameObject.Find("Canvas/Panel").transform);
    }
}
