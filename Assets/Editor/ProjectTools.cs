using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ProjectTools : UnityEditor.AssetModificationProcessor
{
    //[InitializeOnLoadMethod]
    //static void InitializeOnLoadMethod()
    //{
    //    //ȫ�ּ���project��ͼ����Դ�仯
    //    EditorApplication.projectChanged += delegate ()
    //    {
    //        Debug.Log("change");
    //    };

    //}

    ////��������Դ
    //public static bool IsOpenForEdit(string assetPath, out string message)
    //{
    //    message = null;
    //    //Debug.LogFormat("assets path : {0}", assetPath);
    //    //false��ʾ��������unity�д򿪸���Դ
    //    return true;
    //}

    ////������Դ����������
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
    //    //didmove ��ʾ��Դ���ƶ�
    //    return AssetMoveResult.DidMove;
    //}
}
