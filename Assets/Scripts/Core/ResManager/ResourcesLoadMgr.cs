using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourcesLoadMgr
{
    private static ResourcesLoadMgr _instance = null;

    public static ResourcesLoadMgr I
    {
        get
        {
            if (_instance == null) _instance = new ResourcesLoadMgr();
            return _instance;
        }
    }

    private HashSet<string> _resourcesList;

    private ResourcesLoadMgr()
    {
        _resourcesList = new HashSet<string>();
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
            name = name.Substring(0, name.LastIndexOf("."));
            name = name.Replace("\\", "/");
            txt += name + "\n";
        }

        path = path + "FileList.bytes";
        if (File.Exists(path)) File.Delete(path);
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

            if (!_resourcesList.Contains(line))
                _resourcesList.Add(line);
        }
    }

    public bool IsFileExist(string _assetName)
    {
        return _resourcesList.Contains(_assetName);
    }

    public ResourceRequest LoadAsync(string _assetName)
    {
        if (!_resourcesList.Contains(_assetName))
        {
            Debug.LogError("EditorAssetLoadMgr No Find File " + _assetName);
            return null;
        }

        ResourceRequest request = Resources.LoadAsync(_assetName);

        return request;
    }
    public UnityEngine.Object LoadSync(string _assetName)
    {
        if (!_resourcesList.Contains(_assetName))
        {
            Debug.LogError("EditorAssetLoadMgr No Find File " + _assetName);
            return null;
        }

        UnityEngine.Object asset = Resources.Load(_assetName);

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