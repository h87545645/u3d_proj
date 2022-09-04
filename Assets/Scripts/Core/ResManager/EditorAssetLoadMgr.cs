using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EditorAssetLoadMgr
{
    private static EditorAssetLoadMgr _instance = null;

    public static EditorAssetLoadMgr I
    {
        get
        {
            if (_instance == null) _instance = new EditorAssetLoadMgr();
            return _instance;
        }
    }

    private Dictionary<string, string> _resourcesList;


    private EditorAssetLoadMgr()
    {
        _resourcesList = new Dictionary<string, string>();
#if UNITY_EDITOR
        ExportConfig();
#endif
        ReadConfig();
    }


#if UNITY_EDITOR
    private void ExportConfig()
    {
        string path = Application.dataPath + "/Resources/";
        string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

        string txt = "";
        foreach (var file in files)
        {
            if (file.EndsWith(".meta")) continue;

            string name = file.Replace(path, "");
            //name = name.Substring(0, name.LastIndexOf("."));
            name = name.Replace("\\", "/");
            txt += name + "\n";
        }

        path = path + "FileList.bytes";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        File.WriteAllText(path, txt);
    }
#endif

    private void ReadConfig()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("FileList");
        string txt = textAsset.text;
        txt = txt.Replace("\r\n", "\n");

        foreach (var line in txt.Split('\n'))
        {
            if (string.IsNullOrEmpty(line)) continue;
            string name = Path.GetFileName(line);
            if (!_resourcesList.ContainsKey(name))
                _resourcesList.Add(name, line.Substring(0, line.LastIndexOf(".")));
        }

        //List<AssetBundleBuild> AssetBundleLists = AssetBundleCollectEditor.GetAssetBundleBuilds_UI();
        //for (int i = 0; i < AssetBundleLists.Count; i++)
        //{
        //    AssetBundleBuild assetBundle = AssetBundleLists[i];
        //    for (int s = 0; s < assetBundle.assetNames.Length; s++)
        //    {
        //        string path = assetBundle.assetNames[s];
        //        string name = Path.GetFileName(path);
        //        asset_name_path.Add(name, path);
        //    }
        //}
    }

    public bool IsFileExist(string _assetName)
    {
        return _resourcesList.ContainsKey(_assetName);
    }

    public UnityEngine.Object LoadAsync(string _assetName)
    {
        if (!_resourcesList.ContainsKey(_assetName))
        {
            Debug.LogError("EditorAssetLoadMgr No Find File " + _assetName);
            return null;
        }

        //ResourceRequest request = Resources.LoadAsync(_resourcesList[_assetName]);
        UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(_resourcesList[_assetName]);
        return asset;
    }
    public UnityEngine.Object LoadSync(string _assetName)
    {
        if (!_resourcesList.ContainsKey(_assetName))
        {
            Debug.LogError("EditorAssetLoadMgr No Find File " + _assetName);
            return null;
        }

        //UnityEngine.Object asset = Resources.Load(_assetName);
        UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(_resourcesList[_assetName]);
        return asset;
    }

    public void Unload(UnityEngine.Object asset)
    {
        if (asset is GameObject)
        {
            return;
        }

        Resources.UnloadAsset(asset);
        asset = null;
    }
}