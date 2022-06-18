using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using independent;
using UnityEditor;
using System;
using UnityEditor.Events;

public interface IPopuper
{
    /**
  * 初始化数据模型
  */
    IPopuper initVMModel(ArrayList data);

    /**
     * 移除数据模型
     */
    IPopuper removeVMModel();

    /**
     * 注册游戏事件
     */
    IPopuper registerGameEvent();

    /**
     * 移除游戏事件
     */
    IPopuper removeGameEvent();

    /**
     * 弹窗已经完全展示
     */
    IPopuper popupIsShowing(float duration = 0);
}

public enum PopupType
{
    DEFAULT = 0,    // 无动画处理
    POPUP = 1,      // 弹出
    ANIMATION = 2,  // 帧动画
    OPACITY = 3,    // 渐显
};

public enum AdaptType
{
    FIXED_DIRECTION_LANDSCAPE = 0,  // 固定为横屏显示, 弹窗只会横放显示
    FOLLOW_UI_DIRECTION = 1,   // 跟随适配
}

public enum AdaptSizeType
{
    None = 0,           // 占位
    Normal = 1,     // 尺寸设置
    WithBang = 2        // 考虑刘海
}

[ExecuteInEditMode]
public class Popuper : MonoBehaviour
{
    [SerializeField, SetProperty("isModal")]
    [Tooltip("是否为模态对话框")]
    private bool _isModal = true;
    public bool isModal
    {
        private set { _isModal = value; }
        get { return _isModal; }
    }

    //////////////////////////////////////////////////////////////////////////////////

    [SerializeField, SetProperty("isMask")]
    [Tooltip("是否需要mask")]
    private bool _isMask = true;
    public bool isMask
    {
        private set
        {
            _isMask = value;
            if (!_isMask)
            {
                _isMaskClick = false;
                _isMaskBlock = false;
                _isMaskBackground = false;
            }
            else
            {
                _isMaskBlock = true;
            }
            _editorCreateMask();
        }
        get { return _isMask; }
    }

    //////////////////////////////////////////////////////////////////////////////////
    [SerializeField, SetProperty("isMaskClick")]
    [Tooltip("mask是否需要响应点击")]
    private bool _isMaskClick = false;
    public bool isMaskClick
    {
        private set
        {
            if (_isMask)
            {
                _isMaskClick = value;
                _isMaskBlock = !value;
                _editorCreateMask();
            }
            else
            {
                _isMaskClick = false;
            }
        }
        get { return _isMask; }
    }
    //////////////////////////////////////////////////////////////////////////////////


    [SerializeField, SetProperty("isMaskBlock")]
    [Tooltip("mask是否阻断点击")]
    private bool _isMaskBlock = true;
    public bool isMaskBlock
    {
        private set
        {
            if (_isMask)
            {
                _isMaskBlock = value;
                _isMaskClick = !value;
                _editorCreateMask();
            }
            else
            {
                _isMaskBlock = false;
            }
        }
        get { return _isMask; }
    }


    //////////////////////////////////////////////////////////////////////////////////


    [SerializeField, SetProperty("isMaskBackground")]
    [Tooltip("是否需要压黑背景")]
    private bool _isMaskBackground = true;
    public bool isMaskBackground
    {
        private set
        {
            if (_isMask)
            {
                _isMaskBackground = value;
                _editorCreateMask();
            }
            else
            {
                _isMaskBackground = false;
            }
        }
        get { return _isMaskBackground; }
    }


    //////////////////////////////////////////////////////////////////////////////////


    [SerializeField]
    [Tooltip("是否是直接打开窗口, 如果需要自己监听网络时间，等待消息回复之后再打开窗口等，则置为false")]
    public bool openOnLoad = true;



    //////////////////////////////////////////////////////////////////////////////////


    [SerializeField]
    [Tooltip("是否在直接打开次窗口时需要隐藏其他窗口")]
    public bool hideOther = true;

    //////////////////////////////////////////////////////////////////////////////////

    [SerializeField]
    [Tooltip("是否常驻")]
    public bool isAlwaysShow = true;


    //////////////////////////////////////////////////////////////////////////////////


    [SerializeField]
    [Tooltip("是否使用系统的打开音效")]
    public bool userInnerAudio = true;


    //////////////////////////////////////////////////////////////////////////////////


    [SerializeField]
    [Tooltip("弹窗是否需要自动关闭")]
    public bool isAutoClose = false;


    //////////////////////////////////////////////////////////////////////////////////

    [SerializeField]
    [Tooltip("弹窗自动关闭的时长设置")]
    public float autoCloseDuration = 20;

    //////////////////////////////////////////////////////////////////////////////////
    [SerializeField]
    [Tooltip("开启节点scale缩放适配")]
    public bool nodeAdaptScale = false;

    //////////////////////////////////////////////////////////////////////////////////
    [SerializeField]
    [Tooltip("需要scale适配的节点")]
    public GameObject[] adaptScaleNode;
    //////////////////////////////////////////////////////////////////////////////////
    [SerializeField]
    [Tooltip("节点适配类型选择")]
    public AdaptSizeType nodeAdaptType = AdaptSizeType.None;
    //////////////////////////////////////////////////////////////////////////////////
    [SerializeField]
    [Tooltip("需要尺寸适配的节点")]
    public GameObject[] adaptSizeNode;

    //////////////////////////////////////////////////////////////////////////////////

    [SerializeField]
    [Tooltip("弹窗展开时的适配方式，默认为横屏适配，否则跟随UI的适配方向适配")]
    public AdaptType adaptType = AdaptType.FIXED_DIRECTION_LANDSCAPE;

    //////////////////////////////////////////////////////////////////////////////////
    [SerializeField, SetProperty("isMask")]
    [Tooltip("弹窗展示动画类型")]
    private PopupType _popupType = PopupType.POPUP;
    public PopupType popupType
    {
        private set
        {
            _popupType = value;
            // if (_popupType == PopupType.ANIMATION) ;
            // {
            //     // gameObject.transform.FindChild("childName")
            //     gameObject.getChildByName(PopuperConfig.stencil.popupNode).addComponent(cc.Animation);
            // }
            // else
            // {
            //     gameObject.getChildByName(PopuperConfig.stencil.popupNode).removeComponent(cc.Animation);
            // }
        }
        get { return _popupType; }
    }


    //////////////////////////////////////////////////////////////////////////////////

    private int _orignalNodeScale = 1;
    [HideInInspector]
    public int nodeScale
    {
        get { return _orignalNodeScale; }
    }


    //////////////////////////////////////////////////////////////////////////////////

    [HideInInspector]
    public string popupName
    {
        get { return transform.name; }
    }


    //////////////////////////////////////////////////////////////////////////////////

    private PopInstance _uiInstance = null;
    [HideInInspector]
    public PopInstance uiInstance
    {
        get { return _uiInstance; }
    }


    //////////////////////////////////////////////////////////////////////////////////

    private ArrayList _args;
    [HideInInspector]
    public ArrayList args
    {
        set
        {
            _args = value;
        }
        get { return _args; }
    }

    //////////////////////////////////////////////////////////////////////////////////
    private Button _maskButton;

    //////////////////////////////////////////////////////////////////////////////////
    protected Button[] _buttonArr;
    //////////////////////////////////////////////////////////////////////////////////


    /**
 * 初始化数据模型
 */
    public void initVMModel(ArrayList data) { }

    /**
     * 移除数据模型
     */
    public void removeVMModel() { }

    /**
     * 注册游戏事件
     */
    public void registerGameEvent() { }

    /**
     * 移除游戏事件
     */
    public void removeGameEvent() { }

    /**
     * 弹窗已经完全展示
     */
    public void popupIsShowing(float duration = 0) { }


    private void Awake()
    {

#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            Debug.Log(gameObject.name);
            _editorCreateMask(() =>
            {
                _editorCreatePopup();
            });
        }
        else
        {
            registerGameEvent();
            _adaptePopup();
        }
        //     GameObject btnNode = GameObject.Find("popupNode/closeButton");

        //     Button closeButton = (Button)btnNode.GetComponent<Button>();
        //     closeButton.transition = Button.Transition.ColorTint;
        //     closeButton.onClick.AddListener(delegate ()
        //   { 
        //       this.onPopupCloseBtnClick(closeButton.name);
        //   });
#else
        registerGameEvent();
        _adaptePopup();
#endif
    }


    private void OnDestroy()
    {
#if !UNITY_EDITOR
    if (!UnityEditor.EditorApplication.isPlaying){
     
        removeGameEvent();
        removeVMModel();
    }
#endif
    }


    private void OnEnable()
    {
        if (openOnLoad)
        {
            onPopupOpen();
        }
        else
        {
            ready4Open();
        }
    }

    public void init(PopInstance instance, ArrayList args)
    {
        _args = args;
        _uiInstance = instance;
        if (openOnLoad)
        {
            initVMModel(args);
        }
    }

    /**
     * manager外部调动播放popup的弹出动画
     */
    public void showCallByManager()
    {
        float duration = PopupHelper.I.popupOpen(this);

        UnityUtils.DelayFuc(() =>
        {
            popupIsShowing(duration);
        }, 0);

        UnityUtils.DelayFuc(() =>
        {
            if (transform.gameObject != null)
            {
                _getPopupButton();
            }
        }, duration);
    }

    public void hideCallByManager(Action action)
    {
        float duration = PopupHelper.I.popupClose(this);
        UnityUtils.DelayFuc(() =>
        {
            if (action != null)
            {
                action();
            }
        }, duration);
    }

    public void foreClose()
    {
        onPopupClose();
    }

    private void ready4Open()
    {
#if !UNITY_EDITOR
         PopupHelper.I.ready4Open(this);
#endif
    }

    protected void onPopupOpen(ArrayList args = null)
    {
#if !UNITY_EDITOR
         uiInstance.showInstance(args)
#endif
    }

    public void onPopupCloseBtnClick(object arg = null)
    {
        var args = new ArrayList { arg };
        this.onPopupClose(args);
    }

    public void onPopupClose(ArrayList args = null)
    {
#if !UNITY_EDITOR
         uiInstance.showInstance(args)
#endif

        if (_uiInstance.state < PopState.SHOWING)
        {
            _uiInstance.state = PopState.ERROR;
            return;
        }
        if (_maskButton != null)
        {
            _maskButton.interactable = false;
        }

        for (var i = 0; i < _buttonArr.Length; i++)
        {
            if (_buttonArr[i] != null)
            {
                _buttonArr[i].interactable = false;
            }
        }

        _uiInstance.killTheInstance(args);
    }


    public bool canAutoClose(float dt)
    {
        if (!isAutoClose)
        {
            return false;
        }
        if (Double.IsNaN(autoCloseDuration))
        {
            return false;
        }
        autoCloseDuration = autoCloseDuration - dt;
        if (autoCloseDuration <= 0)
        {
            isAutoClose = false;
            return true;
        }
        return false;
    }

    private void _getPopupButton()
    {
        Transform popupNode = transform.Find(PopuperConfig.stencil.popupNode);
        _buttonArr = popupNode.GetComponentsInChildren<Button>();

        Transform maskNode = transform.Find(PopuperConfig.stencil.popupMask);
        if (maskNode != null)
        {
            _maskButton = maskNode.GetComponent<Button>();
        }
    }

    private void _popupResize()
    {
        _adaptePopup();
    }

    /* 
* AdaptePopup 之后执行，用于在 adaptSizeNode 变化之后执行钩子函数
*/
    protected void afterAdaptePopup() { }

    private void _adaptePopup()
    {
#if UNITY_EDITOR
        return;
#endif
        if (transform == null)
        {
            return;
        }

    }

    /**
      * 编辑器模式下创建对话框模板
      */
    private void _editorCreatePopup()
    {
#if !UNITY_EDITOR
        return;
#endif
        GameObject popupNode = null;
        Transform popupNodeTrans = transform.Find(PopuperConfig.stencil.popupNode);
        if (popupNodeTrans != null)
        {
            popupNode = popupNodeTrans.gameObject;
        }
        if (popupNode == null)
        {
            popupNode = new GameObject(PopuperConfig.stencil.popupNode);
            popupNode.AddComponent<RectTransform>();
            popupNode.transform.SetParent(gameObject.transform, false);
        }
        GameObject bgNode = null;
        Transform bgTrans = transform.Find(PopuperConfig.stencil.nodeBg);
        if (bgTrans != null)
        {
            bgNode = bgTrans.gameObject;
        }
        if (bgNode == null)
        {
            bgNode = new GameObject(PopuperConfig.stencil.nodeBg);
            bgNode.transform.SetParent(popupNode.transform, false);
        }
        GameObject contentNode = transform.Find(PopuperConfig.stencil.nodeContent) != null ? transform.Find(PopuperConfig.stencil.nodeContent).gameObject : null;
        if (contentNode == null)
        {
            contentNode = new GameObject(PopuperConfig.stencil.nodeContent);
            contentNode.transform.SetParent(popupNode.transform, false);
        }
        GameObject btnNode = transform.Find(PopuperConfig.stencil.nodeCloseBtn) != null ? transform.Find(PopuperConfig.stencil.nodeCloseBtn).gameObject : null;
        if (btnNode == null)
        {
            btnNode = new GameObject(PopuperConfig.stencil.nodeCloseBtn);
            btnNode.transform.SetParent(popupNode.transform, false);
        }
        Image closeRender = btnNode.GetComponent<Image>();
        if (closeRender == null)
        {
            closeRender = btnNode.AddComponent<Image>();
        }
        Button closeButton = btnNode.GetComponent<Button>();
        if (closeButton == null)
        {
            closeButton = btnNode.AddComponent<Button>();
            closeButton.transition = Button.Transition.ColorTint;
            UnityAction<UnityEngine.Object> action = new UnityAction<UnityEngine.Object>(this.onPopupCloseBtnClick);
            UnityEventTools.AddObjectPersistentListener<UnityEngine.Object>(closeButton.onClick, action, closeButton);
            //     closeButton.onClick.AddListener(delegate ()
            //   {
            //       this.onPopupCloseBtnClick(closeButton.name);
            //   });
            //     StartCoroutine(UnityUtils.DelayFuc(() =>
            //    {

            //    }, Time.deltaTime));

        }

    }



    /**
    * 编辑器模式下创建对话框mask节点
    * @param callback
*/
    private void _editorCreateMask(UnityAction callback = null)
    {
#if !UNITY_EDITOR
        return;
#endif
        GameObject maskNode = null;
        Transform popupMask = transform.Find(PopuperConfig.stencil.popupMask);
        if (popupMask != null)
        {
            maskNode = popupMask.gameObject;
        }
        // GameObject maskNode = transform.Find(PopuperConfig.stencil.popupMask).gameObject;
        if (!isMask)
        {
            if (maskNode != null)
            {
                DestroyImmediate(maskNode);
            }
            return;
        }
        if (isMask && maskNode == null)
        {
            maskNode = Instantiate(AssetDatabase.LoadAssetAtPath(PopuperConfig.maskPrefab.maskURL, typeof(GameObject)) as GameObject);
            maskNode.name = PopuperConfig.stencil.popupMask;
            maskNode.transform.SetParent(transform, false);
            // maskNode.GetComponent(). = isMaskBackground;
            maskNode.GetComponent<RawImage>().enabled = isMaskBackground;
            Button maskButton = maskNode.GetComponent<Button>();
            maskButton.enabled = isMaskClick;
            maskButton.transition = Button.Transition.None;
            UnityAction<UnityEngine.Object> action = new UnityAction<UnityEngine.Object>(this.onPopupCloseBtnClick);
            UnityEventTools.AddObjectPersistentListener<UnityEngine.Object>(maskButton.onClick, action, maskNode);
            // maskButton.onClick.AddListener(delegate ()
            // {
            //     onPopupCloseBtnClick(maskButton.name);
            // });
            if (callback != null)
            {
                callback();
            }
        }
        else
        {
            // 刷新响应
            Button maskButton = maskNode.GetComponent<Button>();
            if (isMaskClick)
            {

                maskButton.transition = Button.Transition.None;
                UnityAction<UnityEngine.Object> action = new UnityAction<UnityEngine.Object>(this.onPopupCloseBtnClick);
                UnityEventTools.AddObjectPersistentListener<UnityEngine.Object>(maskButton.onClick, action, maskNode);
                // maskButton.onClick.AddListener(delegate ()
                // {
                //     onPopupCloseBtnClick(maskButton.name);
                // });
            }
            else
            {

            }
            maskButton.enabled = isMaskClick;
            maskNode.GetComponent<RawImage>().enabled = isMaskBackground;
        }

    }
}
