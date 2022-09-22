using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using independent;


[ExecuteInEditMode]
public class UICenterManager : UIBaseManager
{
    /////////////////////////////////////////////////////////////////////////////////////////////////////////

    [SerializeField, SetProperty("blockWait")]
    private float _blockWait = 0.25f;
    public float blockWait
    {
        get { return _blockWait; }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    [SerializeField, SetProperty("popupMgr")]
    private PopupManager _popupMgr = null;
    public PopupManager popupMgr
    {
        get { return _popupMgr; }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    private static UICenterManager _Instance = null;

    public static UICenterManager I
    {
        get
        {
            if (_Instance == null) _Instance = new UICenterManager();
            return _Instance;
        }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 立刻弹出一个窗口
    /// </summary>
    /// <param name="url"></param>路径
    /// <param name="action"></param>回调
    /// <param name="blockWait"></param>是否需要转圈等待
    /// <param name="args"></param>其他参数
    public void popupSoon(string url, Action<PopState, ArrayList> action = null, bool blockWait = false, ArrayList args = null)
    {
#if UNITY_EDITOR
        return;
#endif
        if (blockWait)
        {
            if (this.blockWait != 0)
            {
                // this.blockWait.startwait(url);
            }
        }
        Action<PopState, ArrayList> backFunc = (PopState state, ArrayList obj) =>
        {
            if (state >= PopState.SHOWING || state == PopState.ERROR)
            {
                if (blockWait)
                {
                    if (this.blockWait != 0)
                    {
                        // this.blockWait.finishWait(url);
                    }
                }
            }
        };
        this._popupMgr.popupSoon(url, backFunc, args);
    }

    /// <summary>
    /// 队列弹出一个窗口
    /// </summary>
    /// <param name="url"></param>路径
    /// <param name="action"></param>回调
    /// <param name="blockWait"></param>是否需要转圈等待
    /// <param name="args"></param>其他参数
    public void popupPush(string url, Action<PopState, ArrayList> action = null, bool blockWait = false, ArrayList args = null)
    {
#if UNITY_EDITOR
        return;
#endif
        Action<PopState, ArrayList> backFunc = (PopState state, ArrayList obj) =>
        {
            if (state >= PopState.INIT && blockWait)
            {
                if (this.blockWait != 0)
                {
                    // this.blockWait.startWait(url);
                }
            }
            if ((state >= PopState.SHOWING || state == PopState.ERROR) && blockWait)
            {
                if (this.blockWait != 0)
                {
                    // this.blockWait.finishWait(url);
                }
            }
            if (action != null)
            {
                action(state, args);
            }
        };

        this._popupMgr.popupPush(url, backFunc, args);
    }


    /// <summary>
    /// 插入弹出一个窗口
    /// </summary>
    /// <param name="url"></param>路径
    /// <param name="action"></param>回调
    /// <param name="blockWait"></param>是否需要转圈等待
    /// <param name="args"></param>其他参数
    public void popupInsert(string url, Action<PopState, ArrayList> action = null, bool blockWait = false, ArrayList args = null)
    {
#if UNITY_EDITOR
        return;
#endif
        Action<PopState, ArrayList> backFunc = (PopState state, ArrayList obj) =>
        {
            if (state >= PopState.INIT && blockWait)
            {
                if (this.blockWait != 0)
                {
                    // this.blockWait.startWait(url);
                }
            }
            if ((state >= PopState.SHOWING || state == PopState.ERROR) && blockWait)
            {
                if (this.blockWait != 0)
                {
                    // this.blockWait.finishWait(url);
                }
            }
            if (action != null)
            {
                action(state, args);
            }
        };

        this._popupMgr.popupInsert(url, backFunc, args);
    }

    /// <summary>
    /// 强制关闭对话框
    /// </summary>
    public void closePopup(MonoBehaviour uiClass){
        this._popupMgr.closePopup(uiClass);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void initManager(){
        foreach (Transform t in transform.GetComponentsInChildren<Transform>())
        {
           switch(t.name){
               case PopuperConfig.ConstPopupLayer:{
                   this._popupMgr = new PopupManager(t.gameObject);
                //    GameUISupporter.getInstance().addPopupMgr(this._popupMgr)
                   break;
               } 
           }
        }
    }
}
