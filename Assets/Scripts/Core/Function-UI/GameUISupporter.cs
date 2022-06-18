using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using independent;
using UnityEngine.UI;
using UnityEditor;

public class GameUISupporter : MonoBehaviour
{

    [SerializeField, SetProperty("sceneType")]
    [Tooltip("UI场景设定")]
    private SceneType _sceneType = SceneType.SCENE_GAME;
    public SceneType sceneType
    {
        get { return _sceneType; }
        set
        {
            if (value != this._sceneType)
            {
                this._editorFreshRelatePb();

            }
        }
    }


    private BangsSet bangsSet = BangsSet.CUSTOM;


    [Tooltip("自定义刘海宽度")]
    private float bwCustomVal = 0;
    [Tooltip("预设刘海宽度")]
    private float bwFixedVal = -1;

    [Tooltip("是否适配其他节点")]
    private bool isOtherAdapt = false;

    [Tooltip("ui之外需要适配的节点")]
    public List<GameObject> adaptNodes = new List<GameObject>();

    public CanvasScaler cvanvas = null;


    private List<PopupManager> _popupMgrList = new List<PopupManager>();

    private static GameUISupporter _Instance = null;
    public static GameUISupporter I
    {
        get
        {
            if (_Instance == null) _Instance = new GameUISupporter();
            return _Instance;
        }
    }

    void Awake()
    {
        GameUISupporter._Instance = this;
#if UNITY_EDITOR
        this._editorCreateRoot();
#else
        this._initGameUI();
        this._updateGameUI();
        this._adapteGameUI();
#endif

    }

    // Update is called once per frame
    void OnDestroy()
    {
#if !UNITY_EDITOR
        for (var i = 0; i < this._popupMgrList.Count; i++)
        {
            this._popupMgrList[i].clear();
        }

        this._popupMgrList.Clear();

        GameUISupporter._Instance = null;
         
#endif


    }


    private void _gameUIResize()
    {
        if (gameObject == null)
        {
            return;
        }
        //todo
        UIAdapter.I.bangsWidth = this.bangsSet == BangsSet.CUSTOM ? this.bwCustomVal : this.bwFixedVal;
        UIAdapter.I.adaptOrientation();
        UIAdapter.I.updateAdapter(ref this.cvanvas);
        this._adapteGameUI();
    }

    /// <summary>
    /// 隐藏所有UI
    /// </summary>
    public void hideAllUI()
    {
        
    }


    /// <summary>
    /// 显示所有UI
    /// </summary>
    public void showAllUI()
    {

    }


    /**
 * 更新UI位置
 */
    public void updateUIPosition(Vector2 offsetVec)
    {
    }

    /**
	 * 添加一个popupManager
	 * @param mgr 
	 */
    public void addPopupMgr(PopupManager mgr)
    {
        if (!this._isPopupMgrAdded(mgr.uuid))
        {
            this._popupMgrList.Add(mgr);
        }
        else
        {
            Debug.LogError("The same popup manager cannot be added twice, uuid : " + mgr.uuid);
        }
    }


    /**
 * 删除一个popupManager
 * @param mgr 
 */
    public void removePopupMgr(PopupManager mgr)
    {
        for (var i = 0; i < this._popupMgrList.Count; i++)
        {
            if (mgr.uuid == this._popupMgrList[i].uuid)
            {
                this._popupMgrList.RemoveAt(i);
            }
        }
    }

    private bool _isPopupMgrAdded(int uuid)
    {
        for (var i = 0; i < this._popupMgrList.Count; i++)
        {
            if (uuid == this._popupMgrList[i].uuid)
            {
                return true;
            }
        }
        return false;
    }

    protected void update()
    {
#if !UNITY_EDITOR
		for (var i = 0; i < this._popupMgrList.Count; i++) {
			this._popupMgrList[i].update();
		}
#endif
    }

    /**
 * 产生creator加载的预制节点
 */
    private void _updateGameUI(bool produce = true)
    {
        for (var i = 0; i < UIConstConfig.configList.Count; ++i)
        {
            var config = UIConstConfig.configList[i];
            var nodeName = config.name;

            var compT = config.compT;
            UIBaseManager component = null;
            switch (compT)
            {
                case UIMgrType.center:
                    {
                        component = this.transform.Find(nodeName).GetComponent<UICenterManager>() as UIBaseManager;

                        break;
                    }
            }
            if (component == null)
            {
                continue;
            }
            // 创建关联的静态节点
            component.updateStaticUI(produce);

            // 创建动态的UI节点
            component.createDynamicUI();
        }

    }

    /**
	 * 适配UI
	 */
    private void _adapteGameUI()
    {
        for (var i = 0; i < UIConstConfig.configList.Count; ++i)
        {
            var rootNode = this.transform.Find(UIConstConfig.configList[i].name);

            UIAdapter.I.adaptUI(rootNode, UIConstConfig.configList[i].adaptionType);

        }

        // 适配其他节点
        if (!this.isOtherAdapt)
        {
            this.adaptNodes.Clear();

        }
        else
        {
            foreach (var item in this.adaptNodes)
            {
                UIAdapter.I.adaptUI(item.transform, AdaptionType.Center);
            }

        }
    }

    private void _initGameUI()
    {
        // 初始化UIAdapter todo
        // UIAdapter.I.screenOrientation = this.screenOrientation
        UIAdapter.I.bangsWidth = this.bangsSet == BangsSet.CUSTOM ? this.bwCustomVal : this.bwFixedVal;
        UIAdapter.I.adaptOrientation();
        UIAdapter.I.updateAdapter(ref this.cvanvas);

        // // 发送适配更新事件
        // EventCenter.sendEvent(UIEvent.UIAdaptUpdate)
    }



    /**
	 * 生成基础节点，上下左右4个节点，并关联相应的组件脚本
	 */
    private void _editorCreateRoot()
    {
#if UNITY_EDITOR
        var designSize = this.GetComponent<RectTransform>().sizeDelta;

        var width = designSize.x;
        var height = designSize.y;

        for (var i = 0; i < UIConstConfig.configList.Count; i++)
        {
            var config = UIConstConfig.configList[i];
            var node = new GameObject(config.name);
            var rtrans = node.AddComponent<RectTransform>();
            rtrans.sizeDelta = config.size;
            rtrans.pivot = config.pivot;
            rtrans.position = new Vector2(config.pos.x*width , config.pos.y*height);
            if (config.compT == UIMgrType.center)
            {
                node.AddComponent<UICenterManager>();
            }

        }

#endif



    }

    private void _editorFreshRelatePb()
    {

    }
}
