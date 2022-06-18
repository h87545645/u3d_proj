using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using UnityEngine;

public class FileVersionData
{
    public string name;
    public int size;
    public string md5;
    public int version;
    public bool initPackage; //是否在初始包内 

    public override string ToString()
    {
        return name + "\t" + size + "\t" + md5 + "\t" + version;
    }

    public void InitData(string str)
    {
        var sps = str.Split('\t');

        name = sps[0];
        size = int.Parse(sps[1]);
        md5 = sps[2];
        version = int.Parse(sps[3]);

#if BIG_PACKAGE && !UNITY_ANDROID
        initPackage = version % 1000 == 0;
#endif
    }

}
public class FileVersionMgr
{

    private static FileVersionMgr _Instance = null;

    public static FileVersionMgr I
    {
        get
        {
            if (_Instance == null) _Instance = new FileVersionMgr();
            return _Instance;
        }
    }

    private Dictionary<string, FileVersionData> _fileList;

    private FileVersionMgr()
    {
        _fileList = new Dictionary<string, FileVersionData>();
    }

    public void Clear()
    {
        _fileList.Clear();
    }

    public void InitVersionFile(string path)
    {
        _fileList.Clear();
        if (!File.Exists(path)) return;

        var lines = File.ReadAllLines(path);
        foreach(var line in lines)
        {
            if (string.IsNullOrEmpty(line)) continue;

            var fileVersionData = new FileVersionData();
            fileVersionData.InitData(line);

            _fileList.Add(fileVersionData.name, fileVersionData);
        }

    }

    public void SaveVersionFile(string path)
    {
        if (File.Exists(path)) File.Delete(path);

        StringBuilder sb = new StringBuilder();
        foreach(var fileData in _fileList.Values)
        {
            sb.Append(fileData.ToString() + "\n");
        }

        File.WriteAllText(path, sb.ToString());
    }

    public FileVersionData GetFileVersionData(string name)
    {
        if (_fileList.ContainsKey(name))
            return _fileList[name];
        return null;
    }

    //提取大于当前版本文件
    public List<FileVersionData> FindUpdateFiles(int version)
    {
        List<FileVersionData> updateList = new List<FileVersionData>();

        foreach(var fileData in _fileList.Values)
        {
            //大包初始版本，不提取包内资源
            if (fileData.initPackage) continue;

            if (fileData.version >= version)
                updateList.Add(fileData);
        }

        return updateList;
    }

    //根据版本获取路径
    public string GetFilePath(string name)
    {
        if(_fileList.ContainsKey(name) && _fileList[name].initPackage)
        {
            return GPath.StreamingAssetsPath + name;
        }

        return GPath.StreamingAssetsPath + name;
    }

    //根据文件是否存在，返回路径，没有返回null
    public string GetFilePathByExist(string name)
    {
        if (_fileList.ContainsKey(name) && _fileList[name].initPackage)
        {
            return GPath.StreamingAssetsPath + name;
        }

        string path = GPath.StreamingAssetsPath + name;
        if (File.Exists(path)) return path;

        path = GPath.StreamingAssetsPath + name;
        if (File.Exists(path)) return path;

        return null;
    }


    //新文件或有更改，替换
    public bool ReplaceFileVersionData(FileVersionData newData)
    {
        if (!_fileList.ContainsKey(newData.name))
        {
            _fileList.Add(newData.name, newData);
            return true;
        }

        var oldData = _fileList[newData.name];
        if (newData.size != oldData.size || newData.md5 != oldData.md5)
        {
            _fileList[newData.name] = newData;
            return true;
        }
        return false;
    }

    public void DeleteFileVersionData(string name)
    {
        if (_fileList.ContainsKey(name))
            _fileList.Remove(name);
    }



}



public struct VersionCode
{
    private int _version;

    public int version
    {
        get { return _version; }
        set { _version = value; }
    }

    public int maxVer
    {
        get { return _version / 1000000; }
    }
    public int midVer
    {
        get { return _version / 1000 % 1000; }
    }
    public int minVer
    {
        get { return _version % 1000; }
    }

    public override string ToString()
    {
        return maxVer + "." + midVer + "." + minVer;
    }
}

public class HotfixMgr
{
    public delegate void HotfixCallback(HotfixState state, string msg, Action<bool> callback);

    public enum HotfixState
    {
        None,  //正在为您初始化资源包
        ReadLocalVersion, //读取本地版本
        CheckCDNVersion,  //获取资源cdn版本
        DownloadVersionFile, //下载版本文件
        CompareAssetVersion, //比对所有版本文件
        DownloadFiles, //下载需要的资源文件
        SaveVersion, //保存版本
        Finished,   //更新完毕,祝你游戏愉快

        RequsetUpdatePackage,  //请求换包
        RequestDownloadFiles, //请求开始下载文件
        RequestErrorRestart, // error重新开始下载
    }

    private static HotfixMgr _Instance = null;

    public static HotfixMgr I
    {
        get
        {
            if (_Instance == null) _Instance = new HotfixMgr();
            return _Instance;
        }
    }

    private HotfixState _currentState;
    private HotfixCallback _hotfixCallback;
    private VersionCode _clientVersion;
    private VersionCode _serverVersion;

    private HotfixMgr()
    {
        _currentState = HotfixState.None;
        _hotfixCallback = null;
        _clientVersion = new VersionCode();
        _serverVersion = new VersionCode();

    }

    public HotfixState currentState
    {
        get { return _currentState; }
    }

    public VersionCode currentVersion
    {
        get { return _clientVersion; }
    }

    public void RegisterRequsetCallback(HotfixCallback hotfixCallback)
    {
        _hotfixCallback = hotfixCallback;
    }

    //启动前，请先调用RegisterRequsetCallback注册回调
    public void Start()
    {
        if(_hotfixCallback == null)
        {
            Debug.LogError("Null Error is _currentState");
            return ;
        }

#if UNIRY_EDITOR && !TEST_AB
        //编辑器状态，不热更
        Finished();
#else
        ReadLocalVersion();
#endif
    }

    private void UpdateError(string msg="")
    {
        _hotfixCallback(HotfixState.RequestErrorRestart, msg, (run) =>
        {
            if (run) Start();
        });
    }

    private void ReadLocalVersion()
    {
        _currentState = HotfixState.ReadLocalVersion;
        string versionPath = GPath.PersistentAssetsPath + GPath.VersionFileName;

        int version;
        if (File.Exists(versionPath))
        {
            version = int.Parse(File.ReadAllText(versionPath));
        }
        else
        {
            version = int.Parse(Resources.Load<TextAsset>(GPath.VersionFileName).text);
        }

        _clientVersion.version = version;
    }

    private void CheckCDNVersion()
    {
        DownloadMgr.I.ClearAllDownloads();

        _currentState = HotfixState.CheckCDNVersion;

        string versionServerFile = GPath.PersistentAssetsPath + "versionServer.txt";
        if (File.Exists(versionServerFile)) File.Delete(versionServerFile);

        DownloadUnit versionUnit = new DownloadUnit();
        versionUnit.name = "version.txt";
        versionUnit.downUrl = GPath.CDNUrl + GPath.VersionFileName;
        versionUnit.savePath = versionServerFile;
        versionUnit.errorFun = (DownloadUnit downUnit, string msg) =>
        {
            Debug.LogWarning("CheckAssetVersion Download Error " + msg + "\n" + downUnit.downUrl);
            DownloadMgr.I.DeleteDownload(versionUnit);
            UpdateError();
        };
        versionUnit.completeFun = (DownloadUnit downUnit) =>
        {
            if (!File.Exists(versionServerFile))
            {//文件不存在，重新下载
                UpdateError();
                return;
            }

            string versionStr = File.ReadAllText(versionServerFile);
            int curVersion = 0;
            if (!int.TryParse(versionStr, out curVersion))
            {
                Debug.LogError("CheckAssetVersion version Error " + versionStr);
                UpdateError();
                return;
            }

            _serverVersion.version = curVersion;
            Debug.Log("本地版本:" + _clientVersion + " 服务器版本：" + curVersion);

            if(_serverVersion.midVer < _clientVersion.midVer)
            {
                UpdateError();
            }
            else if(_serverVersion.midVer > _clientVersion.midVer)
            {//换包
                _hotfixCallback(HotfixState.RequestErrorRestart, "", (run) =>
                {

                });
            }
            else if (_serverVersion.minVer < _clientVersion.minVer)
            {
                Finished();
            }
            else DownloadVersionFile();
        };

        DownloadMgr.I.DownloadAsync(versionUnit);
    }

    private void DownloadVersionFile()
    {
        _currentState = HotfixState.DownloadVersionFile;

        string versionListServerFile = GPath.PersistentAssetsPath + _serverVersion.ToString() + ".txt";
        if (File.Exists(versionListServerFile)) File.Delete(versionListServerFile);

        DownloadUnit versionListUnit = new DownloadUnit();
        versionListUnit.name = _serverVersion.ToString() + ".txt";
        versionListUnit.downUrl = GPath.CDNUrl + versionListUnit.name;
        versionListUnit.savePath = versionListServerFile;
        versionListUnit.errorFun = (DownloadUnit downUnit, string msg) =>
        {
            Debug.LogWarning("CompareVersion Download Error " + msg + "\n" + downUnit.downUrl);
            DownloadMgr.I.DeleteDownload(versionListUnit);
            UpdateError();
        };
        versionListUnit.completeFun = (DownloadUnit downUnit) =>
        {
            if (!File.Exists(versionListServerFile))
            {//文件不存在，重新下载
                UpdateError();
                return;
            }

            FileVersionMgr.I.InitVersionFile(versionListServerFile);
            var updateList = FileVersionMgr.I.FindUpdateFiles(_clientVersion.version);

            Debug.Log("版本文件数量:" + updateList.Count);
            if (updateList.Count > 0) StartDownloadList(updateList);
            else SaveVersion();
        };

        DownloadMgr.I.DownloadAsync(versionListUnit);
        Debug.Log("版本文件url:" + versionListUnit.downUrl);
    }

    private void StartDownloadList(List<FileVersionData> updateList)
    {
        _currentState = HotfixState.CompareAssetVersion;

        string saveRootPath = GPath.PersistentAssetsPath;
        string urlRootPath = GPath.CDNUrl;

        List<DownloadUnit> downloadList = new List<DownloadUnit>();
        Dictionary<string, int> downloadSizeList = new Dictionary<string, int>();
        int downloadCount = updateList.Count;
        int downloadMaxCount = downloadCount;

        int existAllSize = 0;
        int totalAllSize = 0;

        int downloadedFileSizes = 0;
        int downloadAllFileSize = 0;

        foreach (var fileData in updateList)
        {
            string savePath = saveRootPath + fileData.name;
            if (File.Exists(savePath))
            {
                FileInfo fi = new FileInfo(savePath);
                if (fileData.size == (int)fi.Length)
                {//长度相等，可能是已经下载的
                    downloadSizeList.Add(fileData.name, fileData.size);
                    existAllSize += fileData.size;
                }
                else
                {//长度不相等，需要重新下载
                    //Debug.Log("StartDownloadList Delete fileData.size="+ fileData.size + " fi.Length="+ fi.Length);
                    fi.Delete();
                    downloadSizeList.Add(fileData.name, 0);
                    totalAllSize += fileData.size;
                }
            }
            else
            {
                downloadSizeList.Add(fileData.name, 0);
                totalAllSize += fileData.size;
            }

            DownloadUnit downloadUnit = new DownloadUnit();
            downloadUnit.name = fileData.name;
            downloadUnit.downUrl = urlRootPath + fileData.version + "/Assets/" + fileData.name;
            downloadUnit.savePath = savePath;
            downloadUnit.size = fileData.size;
            downloadUnit.md5 = fileData.md5;

            downloadUnit.errorFun = (DownloadUnit downUnit, string msg) =>
            {
                string errorMgs = "StartDownloadList Error " + downUnit.downUrl + " " + msg + "\n";
                Debug.LogWarning(errorMgs);
            };

            downloadUnit.progressFun = (DownloadUnit downUnit, int curSize, int allSize) =>
            {
                downloadedFileSizes += curSize - downloadSizeList[downUnit.name];
                downloadSizeList[downUnit.name] = curSize;
            };

            downloadUnit.completeFun = (DownloadUnit downUnit) =>
            {
                downloadCount--;

                int percent = (downloadMaxCount - downloadCount) * 10 / downloadMaxCount;

                if (downloadCount == 0)
                {//下载完成
                    SaveVersion();
                }
            };

            downloadList.Add(downloadUnit);
        }

        downloadedFileSizes = 0;
        downloadAllFileSize = totalAllSize;

        //如果文件都存在，用已下载的作为长度，概率极低，为了表现进度特殊处理
        if (totalAllSize == 0)
        {
            downloadedFileSizes = 1;
            downloadAllFileSize = 1;
        }


        Debug.Log("下载文件总大小:" + totalAllSize);

        Action downloadFun = () =>
        {
            _currentState = HotfixState.DownloadFiles;
            foreach (var downUnit in downloadList)
            {
                DownloadMgr.I.DownloadAsync(downUnit);
            }
        };

        if (totalAllSize < 1024 * 1024) //<1MB
        {
            downloadFun();
        }
        else
        {
            _hotfixCallback(HotfixState.RequestDownloadFiles, "游戏需要更新部分资源("+
                (totalAllSize/1024/1024) + "M),建议您在无线局域网环境下更新", (run) =>
            {
                if(run == true) downloadFun();

            });
        }

    }

    private void SaveVersion()
    {
        _currentState = HotfixState.SaveVersion;

        _clientVersion.version = _serverVersion.version + 1;
        string versionPath = GPath.PersistentAssetsPath + GPath.VersionFileName;
        File.WriteAllText(versionPath, _clientVersion.version.ToString());
    }

    public void Finished()
    {
        _currentState = HotfixState.Finished;

        _hotfixCallback(HotfixState.Finished, "", null);

    }

}