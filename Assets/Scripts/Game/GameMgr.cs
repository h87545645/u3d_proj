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
        Debug.Log("===>>>GameMgr Awake");
        base.Awake();
        GameObject.DontDestroyOnLoad(gameObject);
        DOTween.Init(true,true, LogBehaviour.Verbose);
        if (langMgr == null)
        {
            langMgr = GetComponent<I18N>();
        }

        if (!Debug.isDebugBuild)
        {
            Debug.unityLogger.logEnabled = false;
        }
        
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // string path = Application.streamingAssetsPath + "/WebGL";
            // StartCoroutine(this.LoadAB4WEBGL(path));
            AssetBundleWebGL.GetInstance().LoadAssetBundle("WebGL", (AssetBundle ab) =>
            {
                AssetBundleLoadMgr.I.LoadMainfest(ab);
                SceneMgr.GetInstance().LoadScene("FragMenuScene",null);
            });
        }
        else
        {
            AssetBundleLoadMgr.I.LoadMainfest();
            SceneMgr.GetInstance().LoadScene("FragMenuScene",null);
        }
     
        
       
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
    

}
