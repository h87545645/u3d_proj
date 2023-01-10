using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Honeti;
using UnityEngine.Networking;

public class GameMgr : MonoSingletonBase<GameMgr>
{
    public I18N langMgr;
    private void Awake()
    {
        base.Awake();
        GameObject.DontDestroyOnLoad(gameObject);
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // string path = Application.streamingAssetsPath + "/WebGL";
            // StartCoroutine(this.LoadAB4WEBGL(path));
            AssetBundleWebGL.GetInstance().LoadAssetBundle("WebGL");
        }
        else
        {
            AssetBundleLoadMgr.I.LoadMainfest();
        }
     
        
        DOTween.Init(true,true, LogBehaviour.Verbose);
        if (langMgr == null)
        {
            langMgr = GetComponent<I18N>();
        }
#if !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;
#endif
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        AssetsLoadMgr.I.Update();
        DownloadMgr.I.Update();
        PrefabLoadMgr.I.Update();
    }
    
    
    // public IEnumerator LoadAB4WEBGL(string uriPath)
    // {
    //     Debug.Log("===>>> LoadAB4WEBGL : " + uriPath);
    //     UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uriPath);
    //     yield return request.SendWebRequest();
    //     if (request.isHttpError)
    //     {
    //         Debug.LogError(GetType() + "/ERROR/" + request.error);
    //     }
    //     else
    //     {
    //         AssetBundle ab = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
    //         
    //         // ab.LoadAsset
    //         ab.Unload(false);
    //     }
    //     request.Dispose();
    // }

}
