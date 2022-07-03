using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ProjectTools : UnityEditor.AssetModificationProcessor
{
    //[InitializeOnLoadMethod]
    //static void InitializeOnLoadMethod()
    //{
    //    //全局监听project视图下资源变化
    //    EditorApplication.projectChanged += delegate ()
    //    {
    //        Debug.Log("change");
    //    };

    //}

    ////监听打开资源
    //public static bool IsOpenForEdit(string assetPath, out string message)
    //{
    //    message = null;
    //    //Debug.LogFormat("assets path : {0}", assetPath);
    //    //false表示不允许在unity中打开该资源
    //    return true;
    //}

    ////监听济源即将被创建
    //public static void OnWillCreateAssets(string path)
    //{
    //    Debug.LogFormat("path :{0}", path);
    //}

    //public static string[] OnWillSaveAssets(string[] paths)
    //{
    //    if(paths != null)
    //    {
    //        Debug.LogFormat("assets save path : {0} ",string.Join(",", paths));
    //    }
    //    return paths;
    //}

    //public static AssetMoveResult OnWillMoveAsset(string oldPath,string newPath)
    //{
    //    Debug.LogFormat("from : {0} to : {1}", oldPath, newPath);
    //    //didmove 表示资源可移动
    //    return AssetMoveResult.DidMove;
    //}
}
