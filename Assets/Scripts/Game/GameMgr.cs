using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    private void Awake()
    {

#if !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;
#endif
    }
    // Start is called before the first frame update
    void Start()
    {
        AssetBundleLoadMgr.I.LoadMainfest();
    }

    // Update is called once per frame
    void Update()
    {
        AssetsLoadMgr.I.Update();
        DownloadMgr.I.Update();
        PrefabLoadMgr.I.Update();
    }
}
