using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class AssetBundleWebGL : MonoSingletonBase<AssetBundleWebGL>
{
    
    // public delegate void AssetBundleLoadCallBack(AssetBundle ab);

    
    

    public void LoadAssetBundle(string assetName , AssetBundleLoadMgr.AssetBundleObject abObj)
    {
        StartCoroutine(this.LoadAB4WEBGL(assetName , abObj));
    }
    
    public void LoadAssetBundle(string assetName , UnityAction<AssetBundle> callback = null)
    {
        StartCoroutine(this.LoadAB4WEBGL(assetName , callback));
    }
    
    
    private IEnumerator LoadAB4WEBGL(string assetName ,  AssetBundleLoadMgr.AssetBundleObject abObj)
    {
        Debug.Log("===>>> LoadAB4WEBGL : " + assetName);
        string path = Application.streamingAssetsPath + "/";
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(path+assetName);
        yield return request.SendWebRequest();
        if (request.isHttpError)
        {
            Debug.LogError(GetType() + "/ERROR/" + request.error);
        }
        else
        {
            AssetBundle ab = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
            abObj._ab = ab;
            // ab.LoadAsset
            // ab.Unload(false);
        }
        request.Dispose();
    }
    
    private IEnumerator LoadAB4WEBGL(string assetName ,  UnityAction<AssetBundle> callBack = null)
    {
        Debug.Log("===>>> LoadAB4WEBGL callback : " + assetName);
        string path = Application.streamingAssetsPath + "/";
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(path+assetName);
        yield return request.SendWebRequest();
        if (request.isHttpError)
        {
            Debug.LogError(GetType() + "/ERROR/" + request.error);
        }
        else
        {
            AssetBundle ab = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
            if (callBack != null)
            {
                callBack(ab);
            }
            // ab.LoadAsset
            // ab.Unload(false);
        }
        request.Dispose();
    }
}
