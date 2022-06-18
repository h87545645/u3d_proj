using System.Collections.Generic;
using UnityEngine;


public class ObjInfo : MonoBehaviour
{
    public int InstanceId = -1;
    public string AssetName = string.Empty;

    void Awake()
    {
        if (string.IsNullOrEmpty(AssetName)) return;
        //非空，说明通过克隆实例化，添加引用计数

        InstanceId = gameObject.GetInstanceID();
        PrefabLoadMgr.I.AddAssetRef(AssetName, this.gameObject);
    }

    void OnDestroy()
    {
        //被动销毁，保证引用计数正确
        PrefabLoadMgr.I.Destroy(this.gameObject);
    }
}


public class PrefabLoadMgr
{
	private static PrefabLoadMgr _instance = null;
	public static PrefabLoadMgr I
	{
		get
		{
			if (_instance == null) _instance = new PrefabLoadMgr();
			return _instance;
		}
	}

	public delegate void PrefabLoadCallback(string name, GameObject obj);

    public class PrefabObject
	{
		public string _assetName;

		public int _lockCallbackCount; //记录回调当前数量，保证异步是下一帧回调
		public List<PrefabLoadCallback> _callbackList = new List<PrefabLoadCallback>();
		public List<Transform> _callParentList = new List<Transform>();

		public UnityEngine.Object _asset;

		public int _refCount;
		public HashSet<int> _goInstanceIDSet = new HashSet<int>(); //实例化的GameObject引用列表
	}

	private Dictionary<string, PrefabObject> _loadedList;
    private List<PrefabObject> _loadedAsyncList; //异步加载，延迟回调
	private Dictionary<int, PrefabObject> _goInstanceIDList; //创建的实例对应的asset

	private GameObject _assetParent;

	private PrefabLoadMgr()
	{
        _loadedList = new Dictionary<string, PrefabObject>();
        _loadedAsyncList = new List<PrefabObject>();

        _goInstanceIDList = new Dictionary<int, PrefabObject>();
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying)
        {
            _assetParent = new GameObject("AssetsList");
            GameObject.DontDestroyOnLoad(_assetParent);
        }
#else
		_assetParent = new GameObject("AssetsList");
		GameObject.DontDestroyOnLoad(_assetParent);
#endif
	}
	private GameObject InstanceAsset(PrefabObject _prefabObj, Transform _parent)
	{
		Transform tempParent = _parent;
		if (_parent == null || _parent.gameObject == null || !_parent.gameObject.activeInHierarchy)
			tempParent = _assetParent.transform;

		GameObject go = GameObject.Instantiate(_prefabObj._asset, tempParent) as GameObject;
		go.name = go.name.Replace("(Clone)", "");
		int instanceID = go.GetInstanceID();

		ObjInfo obgInfo = go.AddComponent<ObjInfo>();
		
        if(!go.activeSelf)
        {//保证GameObject active一次，ObjInfo才能触发Awake，未Awake的脚本不能触发OnDestroy
            go.SetActive(true);
            go.SetActive(false);
        }
        
		if (obgInfo != null)
		{
			obgInfo.InstanceId = instanceID;
			obgInfo.AssetName = _prefabObj._assetName;
		}

		_prefabObj._goInstanceIDSet.Add(instanceID);
		_goInstanceIDList.Add(instanceID, _prefabObj);

		if (_parent != null)
			go.transform.SetParent(_parent);

		return go;
	}

	private void DoInstanceAssetCallback(PrefabObject _prefabObj)
	{
		if (_prefabObj._callbackList.Count == 0) return;

        //先将回掉提取保存，再回调，保证回调中加载和销毁不出错
        int count = _prefabObj._lockCallbackCount; 
        var callbackList = _prefabObj._callbackList.GetRange(0, count);
        var callParentList = _prefabObj._callParentList.GetRange(0, count);

        _prefabObj._lockCallbackCount = 0;
        _prefabObj._callbackList.RemoveRange(0, count);
        _prefabObj._callParentList.RemoveRange(0, count);

        for (int i = 0; i < count; i++)
		{
			if (callbackList[i] != null)
			{
				GameObject newObj = InstanceAsset(_prefabObj, callParentList[i]);//prefab需要实例化

				try
				{
					callbackList[i](_prefabObj._assetName, newObj);    
				}
				catch (System.Exception e)
				{
					Debug.LogError(e);
				}

                //如果回调之后，节点挂在默认节点下，认为该节点无效，销毁
                if (newObj.transform.parent == _assetParent.transform)
                    Destroy(newObj);
            }
		}
	}


	public GameObject LoadSync(string _assetName, Transform _parent = null)
    {
        PrefabObject prefabObj = null;
        if (_loadedList.ContainsKey(_assetName))
        {
            prefabObj = _loadedList[_assetName];
            prefabObj._refCount++;

            if (prefabObj._asset == null)
            {//说明在异步加载中，需要不影响异步加载,加载后要释放
                prefabObj._asset = AssetsLoadMgr.I.LoadSync(_assetName);
                var newGo = InstanceAsset(prefabObj, _parent);
                AssetsLoadMgr.I.Unload(prefabObj._asset);
                prefabObj._asset = null;

                return newGo;
            }
            else return InstanceAsset(prefabObj, _parent);
        }

        prefabObj = new PrefabObject();
        prefabObj._assetName = _assetName;
        prefabObj._refCount = 1;
        prefabObj._asset = AssetsLoadMgr.I.LoadSync(_assetName);

        _loadedList.Add(_assetName, prefabObj);

        return InstanceAsset(prefabObj, _parent);
    }

	public void LoadAsync(string _assetName, PrefabLoadCallback _callFun, Transform _parent = null)
	{
		PrefabObject prefabObj = null;
		if (_loadedList.ContainsKey(_assetName))
		{
			prefabObj = _loadedList[_assetName];
			prefabObj._callbackList.Add(_callFun);
			prefabObj._callParentList.Add(_parent);
            prefabObj._refCount++;

            if(prefabObj._asset != null) _loadedAsyncList.Add(prefabObj);
			return;
		}

		prefabObj = new PrefabObject();
		prefabObj._assetName = _assetName;
		prefabObj._callbackList.Add(_callFun);
		prefabObj._callParentList.Add(_parent);
        prefabObj._refCount = 1;

        _loadedList.Add(_assetName, prefabObj);
        
		AssetsLoadMgr.I.LoadAsync(_assetName, (string name, UnityEngine.Object obj) =>
        {
            prefabObj._asset = obj;

            prefabObj._lockCallbackCount = prefabObj._callbackList.Count;
            DoInstanceAssetCallback(prefabObj);
        }
        );
	}

	public void Destroy(GameObject _obj)
	{
		if (_obj == null) return;

		int instanceID = _obj.GetInstanceID();

		if (!_goInstanceIDList.ContainsKey(instanceID))
		{//非从本类创建的资源，直接销毁即可
			if (_obj is GameObject) UnityEngine.Object.Destroy(_obj);
#if UNITY_EDITOR
            else if (UnityEditor.EditorApplication.isPlaying)
            {
                Debug.LogError("PrefabLoadMgr destroy NoGameObject name=" + _obj.name + " type=" + _obj.GetType().Name);
            }
#else
			else Debug.LogError("PrefabLoadMgr destroy NoGameObject name=" + _obj.name + " type=" + _obj.GetType().Name);
#endif
            return;
		}

        var prefabObj = _goInstanceIDList[instanceID];
		if (prefabObj._goInstanceIDSet.Contains(instanceID))
		{//实例化的GameObject
            prefabObj._refCount--;
            prefabObj._goInstanceIDSet.Remove(instanceID);
			_goInstanceIDList.Remove(instanceID);
			UnityEngine.Object.Destroy(_obj);
		}
		else
		{//error
			string errormsg = string.Format("PrefabLoadMgr Destroy error ! assetName:{0}", prefabObj._assetName);
			Debug.LogError(errormsg);
			return;
		}

		if (prefabObj._refCount < 0)
		{
			string errormsg = string.Format("PrefabLoadMgr Destroy refCount error ! assetName:{0}", prefabObj._assetName);
			Debug.LogError(errormsg);
			return;
		}

		if (prefabObj._refCount == 0)
		{
            _loadedList.Remove(prefabObj._assetName);

			AssetsLoadMgr.I.Unload(prefabObj._asset);
            prefabObj._asset = null;
		}
	}

    //用于解绑回调
    public void RemoveCallBack(string _assetName, PrefabLoadCallback _callFun)
    {
        if (_callFun == null) return;

        PrefabObject prefabObj = null;
        if (_loadedList.ContainsKey(_assetName))
            prefabObj = _loadedList[_assetName];

        if (prefabObj != null)
        {
            int index = prefabObj._callbackList.IndexOf(_callFun);
            if (index >= 0)
            {
                prefabObj._refCount--;
                prefabObj._callbackList.RemoveAt(index);
                prefabObj._callParentList.RemoveAt(index);

                if (index < prefabObj._lockCallbackCount)
                {//说明是加载回调过程中解绑回调，需要降低lock个数
                    prefabObj._lockCallbackCount--;
                }
            }

            if (prefabObj._refCount < 0)
            {
                string errormsg = string.Format("PrefabLoadMgr Destroy refCount error ! assetName:{0}", prefabObj._assetName);
                Debug.LogError(errormsg);
                return;
            }

            if (prefabObj._refCount == 0)
            {
                _loadedList.Remove(prefabObj._assetName);

                AssetsLoadMgr.I.Unload(prefabObj._asset);
                prefabObj._asset = null;
            }
        }


    }

    // 用于外部实例化，增加引用计数
    public void AddAssetRef(string _assetName, GameObject _gameObject)
    {
        if (!_loadedList.ContainsKey(_assetName))
            return;

        PrefabObject prefabObj = _loadedList[_assetName];

        int instanceID = _gameObject.GetInstanceID();
        if(_goInstanceIDList.ContainsKey(instanceID))
        {
            string errormsg = string.Format("PrefabLoadMgr AddAssetRef error ! assetName:{0}", _assetName);
            Debug.LogError(errormsg);
            return;
        }

        prefabObj._refCount++;

        prefabObj._goInstanceIDSet.Add(instanceID);
        _goInstanceIDList.Add(instanceID, prefabObj);
    }

    private void UpdateLoadedAsync()
	{
		if (_loadedAsyncList.Count == 0) return;

		int count = _loadedAsyncList.Count;
        for (int i = 0; i < count; i++)
        {
            _loadedAsyncList[i]._lockCallbackCount = _loadedAsyncList[i]._callbackList.Count;
        }

        for (int i = 0; i < count; i++)
		{
			DoInstanceAssetCallback(_loadedAsyncList[i]);
		}
		_loadedAsyncList.RemoveRange(0, count);
	}


    public void Update()
	{
		UpdateLoadedAsync();
	}
}
