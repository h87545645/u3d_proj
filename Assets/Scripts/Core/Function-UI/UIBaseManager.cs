using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UIBaseManager : MonoBehaviour
{

    [Tooltip("内置预制, 常驻预制体")]
    public List<GameObject> innerPrefabs = new List<GameObject>();
    //////////////////////////////////////////////////////////////////////////////////
    [Tooltip("动态创建的文件路径")]
    public List<string> dynamicPaths = new List<string>();
    //////////////////////////////////////////////////////////////////////////////////
    [Tooltip("'自定义预制, 会随时替换，更新的预制体，比如活动之类的")]
    public List<GameObject> customPrefabs = new List<GameObject>();
    //////////////////////////////////////////////////////////////////////////////////
    [SerializeField, SetProperty("customUI")]
    [Tooltip("自定义预制")]
    private bool _customUI = true;
    public bool customUI
    {
        private set
        {
            this._customUI = value;
            if (!_customUI)
            {
                this.customPrefabs.Clear();
            }
        }
        get { return _customUI; }
    }
    //////////////////////////////////////////////////////////////////////////////////
    [SerializeField, SetProperty("nodeRotation")]
    private float _nodeRotation = 0.0f;
    public float nodeRotation
    {
        private set
        {
            this._nodeRotation = value;

        }
        get { return _nodeRotation; }
    }
    //////////////////////////////////////////////////////////////////////////////////
    [SerializeField, SetProperty("nodeSwitchTime")]
    private float _nodeSwitchTime = 0.25f;
    public float nodeSwitchTime
    {
        get { return _nodeSwitchTime; }
    }
    //////////////////////////////////////////////////////////////////////////////////
    public List<string> _childNames = new List<string>();
    //////////////////////////////////////////////////////////////////////////////////
    [SerializeField, SetProperty("isNodeDisplay")]
    private bool _isNodeDisplay = true;
    public bool isNodeDisplay
    {
        private set
        {
            this._isNodeDisplay = value;
        }
        get { return _isNodeDisplay; }
    }


    //////////////////////////////////////////////////////////////////////////////////


    // Start is called before the first frame update
    void Start()
    {

    }

    protected void OnDestroy()
    {
        this._unloadDynamicRes();
    }

    public void initManager()
    {

    }

    public void updateUIPosition(Transform offsetTrans)
    {
        transform.position = new Vector3(transform.position.x + offsetTrans.position.x, transform.position.y + offsetTrans.position.y);
    }

    public void hideUI()
    {

    }

    public void showUI()
    {

    }

    /// <summary>
    /// 创建动态加载的预制UI
    /// </summary>
    public void createDynamicUI()
    {
#if UNITY_EDITOR
        return;
#endif
        int loadCnt = 0;
        if (this.dynamicPaths.Count > 0)
        {

        }
        else
        {
            this.initManager();
        }
    }

    /// <summary>
    /// 创建或者删除静态加载的预制UI
    /// </summary>
    public void updateStaticUI(bool preview = false)
    {

    }

    private void _removeUI(List<GameObject> pbList)
    {

    }

    /// <summary>
    /// 创建UI
    /// </summary>
    private void _createUI(List<GameObject> pbList)
    {

    }

    /// <summary>
    /// 移除动态加载的资源
    /// </summary>
    private void _unloadDynamicRes()
    {
#if UNITY_EDITOR
        return;
#endif

    }



    public void editorRelatePrefab(GameObject prefab, int index)
    {
#if !UNITY_EDITOR
        return;
#endif
        if (prefab == null)
        {
            return;
        }
        this.innerPrefabs[index] = prefab;
    }

    public void editorRelateDynamic(string path, List<string> names)
    {
        this.dynamicPaths.Clear();
        for (var i = 0; i < names.Count; i++)
        {
            this.dynamicPaths[i] = path + names[i];
        }
    }

    public void editorClearPrefab()
    {
#if !UNITY_EDITOR
        return;
#endif
        this.innerPrefabs.Clear();
    }
}
