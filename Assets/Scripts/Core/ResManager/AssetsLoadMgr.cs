#define TEST_AB

using System.Collections.Generic;
using UnityEngine;

public class AssetsLoadMgr
{
    // const bool TEST_AB = true;
    public delegate void AssetsLoadCallback(string name, UnityEngine.Object obj);

    private class AssetObject
    {
        public string _assetName;

        public int _lockCallbackCount; //记录回调当前数量，保证异步是下一帧回调
        public List<AssetsLoadCallback> _callbackList = new List<AssetsLoadCallback>();

        public int _instanceID; //asset的id
        public AsyncOperation _request;
        public UnityEngine.Object _asset;
        public bool _isAbLoad;

        public bool _isWeak = true; //是否是弱引用
        public int _refCount;

        public int _unloadTick; //卸载使用延迟卸载，UNLOAD_DELAY_TICK_BASE + _unloadList.Count
    }

    private class PreloadAssetObject
    {
        public string _assetName;
        public bool _isWeak = true; //是否是弱引用
    }


    private static AssetsLoadMgr _instance = null;
    public static AssetsLoadMgr I
    {
        get
        {
            if (_instance == null) _instance = new AssetsLoadMgr();
            return _instance;
        }
    }

    public const int UNLOAD_DELAY_TICK_BASE = 60 * 60; //卸载最低延迟
    private const int LOADING_INTERVAL_MAX_COUNT = 50; //每加载50个后，空闲时进行一次资源清理

    private List<AssetObject> tempLoadeds = new List<AssetObject>(); //创建临时存储变量，用于提升性能

    private Dictionary<string, AssetObject> _loadingList;
    private Dictionary<string, AssetObject> _loadedList;
    private Dictionary<string, AssetObject> _unloadList;
    private List<AssetObject> _loadedAsyncList; //异步加载，延迟回调
    private Queue<PreloadAssetObject> _preloadedAsyncList; //异步预加载，空闲时加载

    private Dictionary<int, AssetObject> _goInstanceIDList; //创建的实例对应的asset

    private int _loadingIntervalCount; //加载的间隔时间

    private AssetsLoadMgr()
    {
        _loadingList = new Dictionary<string, AssetObject>();
        _loadedList = new Dictionary<string, AssetObject>();
        _unloadList = new Dictionary<string, AssetObject>();
        _loadedAsyncList = new List<AssetObject>();
        _preloadedAsyncList = new Queue<PreloadAssetObject>();

        _goInstanceIDList = new Dictionary<int, AssetObject>();
    }

    //判断资源是否存在，对打入atlas的图片无法判断，图片请用AtlasLoadMgr
    public bool IsAssetExist(string _assetName)
    {
#if UNITY_EDITOR && !TEST_AB
        // return false;
        return EditorAssetLoadMgr.I.IsFileExist(_assetName);
#else
        if (ResourcesLoadMgr.I.IsFileExist(_assetName)) return true;
        return AssetBundleLoadMgr.I.IsABExist(_assetName);
#endif
    }

    //预加载，isWeak弱引用，true为使用过后会销毁，为false将不会销毁，慎用
    public void PreLoad(string _assetName, bool _isWeak = true)
    {
        AssetObject assetObj = null;
        if (_loadedList.ContainsKey(_assetName)) assetObj = _loadedList[_assetName];
        else if (_loadingList.ContainsKey(_assetName)) assetObj = _loadingList[_assetName];
        //如果已经存在，改变其弱引用关系
        if (assetObj != null)
        {
            assetObj._isWeak = _isWeak;
            if (_isWeak && assetObj._refCount == 0 && !_unloadList.ContainsKey(_assetName))
                _unloadList.Add(_assetName, assetObj);
            return;
        }

        PreloadAssetObject plAssetObj = new PreloadAssetObject();
        plAssetObj._assetName = _assetName;
        plAssetObj._isWeak = _isWeak;

        _preloadedAsyncList.Enqueue(plAssetObj);
    }
    //同步加载，一般用于小型文件，比如配置。
    public UnityEngine.Object LoadSync(string _assetName)
    {
        if (!IsAssetExist(_assetName))
        {
            Debug.LogError("AssetsLoadMgr Asset Not Exist " + _assetName);
            return null;
        }
        
        AssetObject assetObj = null;
        if (_loadedList.ContainsKey(_assetName))
        {
            assetObj = _loadedList[_assetName];
            assetObj._refCount++;
            return assetObj._asset;
        }
        else if (_loadingList.ContainsKey(_assetName))
        {
            assetObj = _loadingList[_assetName];

            if (assetObj._request != null)
            {
                if (assetObj._request is AssetBundleRequest)
                    assetObj._asset = (assetObj._request as AssetBundleRequest).asset; //直接取，会异步变同步
                else assetObj._asset = (assetObj._request as ResourceRequest).asset;
                assetObj._request = null;
            }
            else
            {
#if UNITY_EDITOR && !TEST_AB
                assetObj._asset = EditorAssetLoadMgr.I.LoadSync(_assetName);
#else
                if (assetObj._isAbLoad)
                {
                    AssetBundle ab1 = AssetBundleLoadMgr.I.LoadSync(_assetName);
                    assetObj._asset = ab1.LoadAsset(ab1.GetAllAssetNames()[0]);

                    //异步转同步，需要卸载异步的引用计数
                    AssetBundleLoadMgr.I.Unload(_assetName);
                }
                else
                {
                    assetObj._asset = ResourcesLoadMgr.I.LoadSync(_assetName);
                }
#endif
            }

            if (assetObj._asset == null)
            {//提取的资源失败，从加载列表删除
                _loadingList.Remove(assetObj._assetName);
                Debug.LogError("AssetsLoadMgr assetObj._asset Null " + assetObj._assetName);
                return null;
            }

            assetObj._instanceID = assetObj._asset.GetInstanceID();
            _goInstanceIDList.Add(assetObj._instanceID, assetObj);

            _loadingList.Remove(assetObj._assetName);
            _loadedList.Add(assetObj._assetName, assetObj);
            _loadedAsyncList.Add(assetObj); //原先异步加载的，加入异步表

            assetObj._refCount++;

            return assetObj._asset;
        }

        assetObj = new AssetObject();
        assetObj._assetName = _assetName;

#if UNITY_EDITOR && !TEST_AB
        assetObj._asset = EditorAssetLoadMgr.I.LoadSync(_assetName);
#else
        if (AssetBundleLoadMgr.I.IsABExist(_assetName))
        {
            assetObj._isAbLoad = true;
            Debug.LogWarning("AssetsLoadMgr LoadSync doubtful asset=" + assetObj._assetName);
            AssetBundle ab1 = AssetBundleLoadMgr.I.LoadSync(_assetName);
            assetObj._asset = ab1.LoadAsset(ab1.GetAllAssetNames()[0]);
        }
        else if (ResourcesLoadMgr.I.IsFileExist(_assetName))
        {
            assetObj._isAbLoad = false;
            assetObj._asset = ResourcesLoadMgr.I.LoadSync(_assetName);
        } 
        else return null;
#endif
        if (assetObj._asset == null)
        {//提取的资源失败，从加载列表删除
            Debug.LogError("AssetsLoadMgr assetObj._asset Null " + assetObj._assetName);
            return null;
        }

        assetObj._instanceID = assetObj._asset.GetInstanceID();
        _goInstanceIDList.Add(assetObj._instanceID, assetObj);

        _loadedList.Add(_assetName, assetObj);

        assetObj._refCount = 1;

        return assetObj._asset;
    }

    //用于解绑回调
    public void RemoveCallBack(string _assetName, AssetsLoadCallback _callFun)
    {
        if (_callFun == null) return;
        //对于不确定的回调，依据回调函数删除
        if (string.IsNullOrEmpty(_assetName)) RemoveCallBackByCallBack(_callFun);

        AssetObject assetObj = null;
        if (_loadedList.ContainsKey(_assetName)) assetObj = _loadedList[_assetName];
        else if (_loadingList.ContainsKey(_assetName)) assetObj = _loadingList[_assetName];

        if (assetObj != null)
        {
            int index = assetObj._callbackList.IndexOf(_callFun);
            if (index >= 0)
            {
                assetObj._callbackList.RemoveAt(index);
            }
        }
    }

    //资源销毁，请保证资源销毁都要调用这个接口
    public void Unload(UnityEngine.Object _obj)
    {
        if (_obj == null) return;

        int instanceID = _obj.GetInstanceID();

        if (!_goInstanceIDList.ContainsKey(instanceID))
        {//非从本类创建的资源，直接销毁即可
            if (_obj is GameObject) UnityEngine.Object.Destroy(_obj);
#if UNITY_EDITOR
            else if (UnityEditor.EditorApplication.isPlaying)
            {
                Debug.LogError("AssetsLoadMgr destroy NoGameObject name=" + _obj.name + " type=" + _obj.GetType().Name);
            }
#else
            else Debug.LogError("AssetsLoadMgr destroy NoGameObject name=" + _obj.name+" type="+_obj.GetType().Name);
#endif
            return;
        }

        var assetObj = _goInstanceIDList[instanceID];
        if (assetObj._instanceID == instanceID)
        {//_obj不是GameObject，不销毁
            assetObj._refCount--;
        }
        else
        {//error
            string errormsg = string.Format("AssetsLoadMgr Destroy error ! assetName:{0}", assetObj._assetName);
            Debug.LogError(errormsg);
            return;
        }

        if (assetObj._refCount < 0)
        {
            string errormsg = string.Format("AssetsLoadMgr Destroy refCount error ! assetName:{0}", assetObj._assetName);
            Debug.LogError(errormsg);
            return;
        }

        if (assetObj._refCount == 0 && !_unloadList.ContainsKey(assetObj._assetName))
        {
            assetObj._unloadTick = UNLOAD_DELAY_TICK_BASE + _unloadList.Count;
            _unloadList.Add(assetObj._assetName, assetObj);
        }

    }

    //异步加载，即使资源已经加载完成，也会异步回调。
    public void LoadAsync(string _assetName, AssetsLoadCallback _callFun)
    {
        if (!IsAssetExist(_assetName))
        {
            Debug.LogError("AssetsLoadMgr Asset Not Exist " + _assetName);
            return;
        }
        
        AssetObject assetObj = null;
        if (_loadedList.ContainsKey(_assetName))
        {
            assetObj = _loadedList[_assetName];
            assetObj._callbackList.Add(_callFun);
            _loadedAsyncList.Add(assetObj);
            return;
        }
        else if (_loadingList.ContainsKey(_assetName))
        {
            assetObj = _loadingList[_assetName];
            assetObj._callbackList.Add(_callFun);
            return;
        }

        assetObj = new AssetObject();
        assetObj._assetName = _assetName;

        assetObj._callbackList.Add(_callFun);

#if UNITY_EDITOR && !TEST_AB
        _loadingList.Add(_assetName, assetObj);
        assetObj._request = EditorAssetLoadMgr.I.LoadAsync(_assetName);
#else
        if (AssetBundleLoadMgr.I.IsABExist(_assetName))
        {
            assetObj._isAbLoad = true;
            _loadingList.Add(_assetName, assetObj);

            AssetBundleLoadMgr.I.LoadAsync(_assetName,
                (AssetBundle _ab) =>
                {
                    if (_ab == null)
                    {
                        string errormsg = string.Format("LoadAsset request error ! assetName:{0}", assetObj._assetName);
                        Debug.LogError(errormsg);
                        _loadingList.Remove(_assetName);
                        //重新添加，保证成功
                        for (int i = 0; i < assetObj._callbackList.Count; i++)
                        {
                            LoadAsync(assetObj._assetName, assetObj._callbackList[i]);
                        }
                        return;
                    }

                    if (_loadingList.ContainsKey(_assetName) && assetObj._request == null && assetObj._asset == null)
                    {
                        assetObj._request = _ab.LoadAssetAsync(_ab.GetAllAssetNames()[0]);
                    }

                }
            );
        }
        else if (ResourcesLoadMgr.I.IsFileExist(_assetName))
        {
            assetObj._isAbLoad = false;
            _loadingList.Add(_assetName, assetObj);

            assetObj._request = ResourcesLoadMgr.I.LoadAsync(_assetName);
        }
        else return;
#endif
    }

    //外部加载的资源，加入资源管理，给其他地方调用
    public void AddAsset(string _assetName, UnityEngine.Object _asset)
    {
        var assetObj = new AssetObject();
        assetObj._assetName = _assetName;

        assetObj._instanceID = _asset.GetInstanceID();
        assetObj._asset = _asset;
        assetObj._refCount = 1;

        _loadedList.Add(assetObj._assetName, assetObj);
        _goInstanceIDList.Add(assetObj._instanceID, assetObj);
    }

    //针对特定资源需要添加引用计数，保证引用计数正确
    public void AddAssetRef(string _assetName)
    {
        if (!_loadedList.ContainsKey(_assetName))
        {
            Debug.LogError("AssetsLoadMgr AddAssetRef Error " + _assetName);
            return;
        }

        var assetObj = _loadedList[_assetName];
        assetObj._refCount++;

    }

    private void RemoveCallBackByCallBack(AssetsLoadCallback _callFun)
    {
        foreach (var assetObj in _loadingList.Values)
        {
            if (assetObj._callbackList.Count == 0) continue;
            int index = assetObj._callbackList.IndexOf(_callFun);
            if (index >= 0)
            {
                assetObj._callbackList.RemoveAt(index);
            }
        }

        foreach (var assetObj in _loadedList.Values)
        {
            if (assetObj._callbackList.Count == 0) continue;
            int index = assetObj._callbackList.IndexOf(_callFun);
            if (index >= 0)
            {
                assetObj._callbackList.RemoveAt(index);
            }
        }
    }

    private void DoAssetCallback(AssetObject _assetObj)
    {
        if (_assetObj._callbackList.Count == 0) return;

        int count = _assetObj._lockCallbackCount; //先提取count，保证回调中有加载需求不加载
        for (int i = 0; i < count; i++)
        {
            if (_assetObj._callbackList[i] != null)
            {
                _assetObj._refCount++; //每次回调，引用计数+1

                try
                {
                    _assetObj._callbackList[i](_assetObj._assetName, _assetObj._asset);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        _assetObj._callbackList.RemoveRange(0, count);
    }

    private void DoUnload(AssetObject _assetObj)
    {
#if UNITY_EDITOR && !TEST_AB
        EditorAssetLoadMgr.I.Unload(_assetObj._asset);
#else
        if (_assetObj._isAbLoad)
            AssetBundleLoadMgr.I.Unload(_assetObj._assetName);
        else ResourcesLoadMgr.I.Unload(_assetObj._asset);
#endif
        _assetObj._asset = null;

        if (_goInstanceIDList.ContainsKey(_assetObj._instanceID))
        {
            _goInstanceIDList.Remove(_assetObj._instanceID);
        }
    }

    private void UpdateLoadedAsync()
    {
        if (_loadedAsyncList.Count == 0) return;

        int count = _loadedAsyncList.Count;
        for (int i = 0; i < count; i++)
        {
            //先锁定回调数量，保证异步成立
            _loadedAsyncList[i]._lockCallbackCount = _loadedAsyncList[i]._callbackList.Count;
        }
        for (int i = 0; i < count; i++)
        {
            DoAssetCallback(_loadedAsyncList[i]);
        }
        _loadedAsyncList.RemoveRange(0, count);

        if (_loadingList.Count == 0 && _loadingIntervalCount > LOADING_INTERVAL_MAX_COUNT)
        {//在连续的大量加载后，强制调用一次gc
            _loadingIntervalCount = 0;
            //Resources.UnloadUnusedAssets();
            //System.GC.Collect();
        }
    }

    private void UpdateLoading()
    {
        if (_loadingList.Count == 0) return;

        //检测加载完的
        tempLoadeds.Clear();
        foreach (var assetObj in _loadingList.Values)
        {
#if UNITY_EDITOR && !TEST_AB

            if (assetObj._request != null && assetObj._request.isDone)
            {
                assetObj._asset = (assetObj._request as ResourceRequest).asset;

                if (assetObj._asset == null)
                {//提取的资源失败，从加载列表删除
                    _loadingList.Remove(assetObj._assetName);
                    Debug.LogError("AssetsLoadMgr assetObj._asset Null " + assetObj._assetName);
                    break;
                }

                assetObj._instanceID = assetObj._asset.GetInstanceID();
                _goInstanceIDList.Add(assetObj._instanceID, assetObj);
                assetObj._request = null;
                tempLoadeds.Add(assetObj);
            }
#else
            if (assetObj._request != null && assetObj._request.isDone)
            {
                //加载完进行数据清理
                if (assetObj._request is AssetBundleRequest)
                    assetObj._asset = (assetObj._request as AssetBundleRequest).asset;
                else assetObj._asset = (assetObj._request as ResourceRequest).asset;

                if(assetObj._asset == null)
                {//提取的资源失败，从加载列表删除
                    _loadingList.Remove(assetObj._assetName);
                    Debug.LogError("AssetsLoadMgr assetObj._asset Null " + assetObj._assetName);
                    break;
                }

                assetObj._instanceID = assetObj._asset.GetInstanceID();
                _goInstanceIDList.Add(assetObj._instanceID, assetObj);
                assetObj._request = null;

                tempLoadeds.Add(assetObj);
            }
#endif
        }

        //回调中有可能对_loadingList进行操作，先移动
        foreach (var assetObj in tempLoadeds)
        {
            _loadingList.Remove(assetObj._assetName);
            _loadedList.Add(assetObj._assetName, assetObj);
            _loadingIntervalCount++; //统计本轮加载的数量

            //先锁定回调数量，保证异步成立
            assetObj._lockCallbackCount = assetObj._callbackList.Count;
        }
        foreach (var assetObj in tempLoadeds)
        {
            DoAssetCallback(assetObj);
        }
    }

    private void UpdateUnload()
    {
        if (_unloadList.Count == 0) return;

        tempLoadeds.Clear();
        foreach (var assetObj in _unloadList.Values)
        {
            if (assetObj._isWeak && assetObj._refCount == 0 && assetObj._callbackList.Count == 0)
            {//引用计数为0，且没有需要回调的函数，销毁
                if (assetObj._unloadTick < 0)
                {
                    _loadedList.Remove(assetObj._assetName);
                    DoUnload(assetObj);

                    tempLoadeds.Add(assetObj);
                }
                else assetObj._unloadTick--;
            }

            if (assetObj._refCount > 0 || !assetObj._isWeak)
            {//引用计数增加（销毁期间有加载）
                tempLoadeds.Add(assetObj);
            }
        }

        foreach (var assetObj in tempLoadeds)
        {
            _unloadList.Remove(assetObj._assetName);
        }

    }

    private void UpdatePreload()
    {
        if (_loadingList.Count > 0 || _preloadedAsyncList.Count == 0) return;

        //从队列取出一个，异步加载
        PreloadAssetObject plAssetObj = null;
        while (_preloadedAsyncList.Count > 0 && plAssetObj == null)
        {
            plAssetObj = _preloadedAsyncList.Dequeue();

            if (_loadingList.ContainsKey(plAssetObj._assetName))
            {
                _loadingList[plAssetObj._assetName]._isWeak = plAssetObj._isWeak;
            }
            else if (_loadedList.ContainsKey(plAssetObj._assetName))
            {
                _loadedList[plAssetObj._assetName]._isWeak = plAssetObj._isWeak;
                plAssetObj = null; //如果当前没开始加载，重新选一个
            }
            else
            {
                LoadAsync(plAssetObj._assetName, (AssetsLoadCallback)null);
                if (_loadingList.ContainsKey(plAssetObj._assetName))
                {
                    _loadingList[plAssetObj._assetName]._isWeak = plAssetObj._isWeak;
                }
                else if (_loadedList.ContainsKey(plAssetObj._assetName))
                {
                    _loadedList[plAssetObj._assetName]._isWeak = plAssetObj._isWeak;
                }
            }
        }
    }

    public void Update()
    {
        UpdatePreload(); //预加载，空闲时启动

        UpdateLoadedAsync(); //已经加载的异步回调
        UpdateLoading(); //加载完成，回调
        UpdateUnload(); //卸载需要销毁的资源
#if UNITY_EDITOR && !TEST_AB
        // EditorAssetLoadMgr.I.Update();
#else
        AssetBundleLoadMgr.I.Update();
#endif
    }

}
