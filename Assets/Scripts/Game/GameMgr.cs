using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Honeti;

public class GameMgr : MonoSingletonBase<GameMgr>
{
    public I18N langMgr;
    private void Awake()
    {
        base.Awake();
        GameObject.DontDestroyOnLoad(gameObject);
        AssetBundleLoadMgr.I.LoadMainfest();
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
}
