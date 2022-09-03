using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DemoLoadAssetBundle : MonoBehaviour
{

    public string bundleName = "player";
    public string assetName = "player";

    public string releaseAssetName = "player";

    void OnGUI()
    {
        //GUILayout.BeginArea(new Rect(600, 300, 200, 200));//固定布局  //Rect(float x,float y,float width,float height)  
        //GUILayout.BeginVertical();//内层嵌套一个纵向布局 

        if (GUILayout.Button("<size=50>loadAssetbundle</size>"))
            {
            // InstantiateBundle();
            //  Object uObj_cur = AssetsLoadMgr.I.LoadSync(assetName);
            // GameObject g = Instantiate(uObj_cur) as GameObject;
            // g.transform.parent = GameObject.Find("Canvas/Panel").gameObject.transform;
            PrefabLoadMgr.I.LoadSync(assetName, GameObject.Find("Canvas/Panel").transform);
        }

        if (GUILayout.Button("<size=50>relese</size>"))
        {
            releaseBundle();
        }

        if (GUILayout.Button("<size=50>releseAll</size>"))
        {
            AssetBundleManager.GetInstance().ClearAllABPack();
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        // PopupManager.
    }

    [ContextMenu("InstantiateBundle")]
    protected void InstantiateBundle()
    {
        GameObject uObj_cur_01 = AssetBundleManager.GetInstance().LoadABPackRes(bundleName, assetName) as GameObject;
        uObj_cur_01.transform.parent = GameObject.Find("Canvas/Panel").gameObject.transform;
        // print(Path.Combine(Application.streamingAssetsPath , "StandaloneWindows"));
        // AssetBundle assetBundle = AssetBundle.LoadFromFile( Path.Combine(Application.streamingAssetsPath , "StandaloneWindows"));
        // AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
    }

    protected void releaseBundle()
    {
        AssetBundleManager.GetInstance().UnLoadABPack(releaseAssetName);
    }
}
