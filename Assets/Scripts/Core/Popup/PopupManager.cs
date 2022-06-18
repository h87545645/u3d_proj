using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using independent;

public enum PopState
{
    /**
* 打开出错
*/
    ERROR = -1,
    /**
     * 等待显示
     */
    WAIT = 0,

    /**
     * 初始化中，加载资源中
     */
    INIT = 1,

    /**
     * 资源加载已经完成，只需要直接显示
     */
    READY = 2,

    /**
     * 已经显示
     */
    SHOWING = 3,
    /**
     * 被隐藏掉
     */
    DISABLE = 4,
    /**
     * 显示完毕
     */
    FINISHED = 5,
    /**
     * 结束整个生命周期
     */
    END = 6,
};

/**
 * 弹窗实例类
 * @export
 * @class PopInstance
 */
public class PopInstance
{
    // private delegate void PopCall();
    private Action<PopState, ArrayList> _callback;
    // private GameObject _prefab = null;
    private ArrayList _args;

    private GameObject _uiNode = null;
    [HideInInspector]
    public GameObject uiNode
    {
        get { return this._uiNode; }
    }

    private Popuper _uiComp = null;
    [HideInInspector]
    public Popuper uiComp
    {
        get { return this._uiComp; }
    }

    private int _priority = 0;
    [HideInInspector]
    public int priority
    {
        get { return this._priority; }
    }

    private string _url = "";
    [HideInInspector]
    public string Url
    {
        get { return this._url; }
    }

    private PopInstance _keyInstance = null;
    [HideInInspector]
    public PopInstance keyInstance
    {
        get { return this._keyInstance; }
    }
    private bool isKilling = false;
    public PopState state = PopState.WAIT;

    public PopInstance(string url, Action<PopState, ArrayList> callbcak = null, int priority = 0, ArrayList args = null)
    {
        this._url = url;
        this._args = args;
        this._callback = callbcak;
        this._priority = priority;
    }

    public void refresh(Action<PopState, ArrayList> callbcak = null, int priority = 0, ArrayList args = null)
    {
        this._args = args;
        this._callback = callbcak;
        this._priority = priority;
    }


    public void initInstance(Action<PopInstance> callback, bool preLoad = false)
    {
        string uiResUrl = this.Url;
        this._changeInitialState(PopState.INIT, preLoad);
        Action<GameObject> _doInstance = (GameObject prefab) =>
        {
            this._uiNode = prefab;
            this._uiComp = this._uiNode.GetComponent<Popuper>();
            this._uiComp.init(this, this._args);
            this._changeInitialState(PopState.READY, preLoad);
        };
        PrefabLoadMgr.I.LoadAsync(uiResUrl, (string name, GameObject go) =>
        {
            _doInstance(go);
            if (callback != null)
            {
                callback(this);
            }
        });
    }

    public void showInstance(ArrayList args)
    {
        this._uiComp.showCallByManager();
        this._changeInstanceState(PopState.SHOWING, args);
    }

    /**
   * 结束显示
   */
    public void killTheInstance(ArrayList args)
    {
        isKilling = true;
        if (this.state == PopState.SHOWING)
        {
            this._uiComp.hideCallByManager(() =>
            {
                this.destroy();
            });
            this._changeInstanceState(PopState.FINISHED, args);
        }
        else
        {
            this._changeInstanceState(PopState.FINISHED, args);
            this.destroy();
        }
    }

    public void disableInstance(PopInstance instance)
    {
        if (this == instance) return;
        if (this._keyInstance != null) return;
        if (this.state == PopState.DISABLE) return;
        if (this.state == PopState.FINISHED) return;
        if (this.isKilling) return;

        if (this.state == PopState.SHOWING)
        {
            this._uiComp.hideCallByManager(() =>
            {
                this._changeInstanceState(PopState.DISABLE);
                this._keyInstance = instance;
                this._uiNode.SetActive(true);
            });
        }
        else
        {
            this._changeInstanceState(PopState.DISABLE);
            this._keyInstance = instance;
            this._uiNode.SetActive(false);
        }
    }

    public void enableInstance()
    {
        if (this.keyInstance == null) return;
        if (this.state != PopState.DISABLE) return;
        if (this.keyInstance.state >= PopState.FINISHED)
        {
            this._keyInstance = null;
            this._uiNode.SetActive(true);
            this._uiComp.showCallByManager();
            this._changeInstanceState(PopState.SHOWING);
        }
    }


    public void destroy()
    {
        if (this._uiNode != null)
        {
            PrefabLoadMgr.I.Destroy(this._uiNode);
        }
    }
    /**
     * 是否有相同弹窗正在展示
     * @param url 
     */
    public bool isSamePopup(string url)
    {
        if (this.Url == url)
        {
            return true;
        }
        return false;
    }

    private void _changeInstanceState(PopState state, ArrayList args = null)
    {
        if (this.state == state)
        {
            return;
        }
        this.state = state;
        if (this._callback != null)
        {
            this._callback(state, args);
        }
    }

    private void _changeInitialState(PopState state, bool preload = false, ArrayList args = null)
    {
        this.state = state;
        if (this._callback != null && !preload)
        {
            this._callback(state, args);
        }
    }

}

/**
 * 弹窗管理类
 * @export
 * @class PopupManager
 */
public class PopupManager
{
    private int _uuid;
    [HideInInspector]
    public int uuid
    {
        get { return this._uiRoot.GetInstanceID(); }
    }

    private bool _locked = false;
    [HideInInspector]
    public bool Lock
    {
        set { this._locked = value; }
    }

    private GameObject _uiRoot = null;
    private List<PopInstance> _uiShowList = new List<PopInstance>();
    private List<PopInstance> _uiQueue = new List<PopInstance>();

    private Dictionary<string, PopInstance> _popupPreDict = new Dictionary<string, PopInstance>();

    public PopupManager(GameObject rootLayer)
    {
        this._uiRoot = rootLayer;
    }

    public void popupSoon(string url, Action<PopState, ArrayList> callback = null, ArrayList args = null)
    {
        if (this._locked)
        {
            return;
        }
        if (this._isPopDisplayed(url))
        {
            callback(PopState.ERROR, null);
            return;
        }

        PopInstance uiInstance = null;
        Action _inHandle = () =>
      {
          if (this._uiRoot != null)
          {
              uiInstance.uiNode.transform.SetParent(this._uiRoot.transform);
              uiInstance.uiNode.transform.SetSiblingIndex(ConstDesignSize.PopupConstIndex);
              if (uiInstance.uiComp.hideOther)
              {
                  for (var i = 0; i < this._uiShowList.Count; i++)
                  {
                      if (this._uiShowList[i].state == PopState.SHOWING)
                      {
                          if (this._uiShowList[i].uiComp != null && !this._uiShowList[i].uiComp.isAlwaysShow)
                          {
                              this._uiShowList[i].disableInstance(uiInstance);
                          }
                      }
                  }
              }

          }
      };
        if (this._popupPreDict.ContainsKey(url))
        {
            uiInstance = this._popupPreDict[url];
            this._popupPreDict.Remove(url);
            uiInstance.refresh(callback, 0, args);
        }
        else
        {
            uiInstance = new PopInstance(url, callback, 0, args);
        }

        if (uiInstance.state == PopState.READY)
        {
            _inHandle();
        }
        else
        {
            if (uiInstance.state == PopState.INIT)
            {
                uiInstance.initInstance((instant) =>
                {
                    _inHandle();
                });
            }
        }
        this._uiShowList.Add(uiInstance);
    }

    /// <summary>
    ///  插入一个UI，将其放入队列前端
    /// </summary>
    public void popupInsert(string url, Action<PopState, ArrayList> callback = null, ArrayList args = null)
    {
        if (this._locked)
        {
            return;
        }
        if (this._isPopDisplayed(url))
        {
            if (callback != null)
            {
                callback(PopState.ERROR, null);
            }
            return;
        }
        if (this._isPopInQueue(url))
        {
            if (callback != null)
            {
                callback(PopState.ERROR, null);
            }
            return;
        }
        if (this._popupPreDict.ContainsKey(url))
        {
            PopInstance uiInstance = this._popupPreDict[url];
            this._popupPreDict.Remove(url);
            uiInstance.refresh(callback, 1, args);
            this._uiQueue.Insert(0, uiInstance);
        }
        else
        {
            this._uiQueue.Insert(0, new PopInstance(url, callback, 1, args));
        }
    }

    /// <summary>
    /// push一个popUp
    /// </summary>
    /// <param name="url"></param>对话框文件路径
    /// <param name="callback"></param>回调函数
    /// <param name="args"></param>其他用户自定义参数
    public void popupPush(string url, Action<PopState, ArrayList> callback = null, ArrayList args = null)
    {
        if (this._locked)
        {
            return;
        }
        if (this._isPopDisplayed(url))
        {
            callback(PopState.ERROR, null);
            return;
        }
        if (this._popupPreDict.ContainsKey(url))
        {
            PopInstance uiInstance = this._popupPreDict[url];
            this._popupPreDict.Remove(url);
            uiInstance.refresh(callback, 1, args);
            this._uiQueue.Add(uiInstance);
        }
        else
        {
            this._uiQueue.Add(new PopInstance(url, callback, 0, args));
        }
    }

    /// <summary>
    /// 预先准备弹窗
    /// </summary>
    public void preparePopup(string url)
    {
        if (this._locked)
        {
            return;
        }
        if (this._popupPreDict.ContainsKey(url))
        {
            return;
        }
        PopInstance uiInstance = new PopInstance(url, null, 0, null);
        uiInstance.initInstance(null, true);
        this._popupPreDict.Add(url, uiInstance);
    }

    /// <summary>
    /// 主动销毁
    /// </summary>        
    public void destoryPrepare(string url)
    {
        if (this._popupPreDict.ContainsKey(url))
        {
            PopInstance popInstance = this._popupPreDict[url];
            this._popupPreDict.Remove(url);
            popInstance.destroy();
            popInstance = null;
        }
    }

    /// <summary>
    /// 外部强制关闭UI
    /// </summary>
    public void closePopup(MonoBehaviour uiClass)
    {
        this._uiShowList.ForEach(instance =>
        {
            if (instance.uiComp.GetInstanceID() == uiClass.GetInstanceID())
            {
                instance.killTheInstance(null);
            }
        });
    }

    /// <summary>
    /// 外部强制关闭UI
    /// </summary>
    public void closePopupBy(string url)
    {
        this._uiShowList.ForEach(instance =>
        {
            if (instance.Url == url)
            {
                instance.killTheInstance(null);
            }
        });
    }

    /// <summary>
    /// 强制关闭显示队列的第一个弹窗
    /// </summary>
    public void forceClosePopup()
    {
        if (this._uiShowList.Count <= 0)
        {
            return;
        }
        PopInstance currPopuper = this._uiShowList[this._uiShowList.Count - 1];
        if (currPopuper != null && currPopuper.state >= PopState.SHOWING)
        {
            currPopuper.uiComp.foreClose();
        }
    }

    /// <summary>
    /// 清除manager数据
    /// </summary>
    public void clear()
    {
        for (var i = 0; i < this._uiShowList.Count; i++)
        {
            this._uiShowList[i].destroy();
        }
        //清理弹窗队列
        this._uiQueue.Clear();
    }

    /// <summary>
    /// 是否这个节点在显示
    /// </summary>
    public bool isTargetPopupShowing(string url)
    {
        for (var i = 0; i < this._uiShowList.Count; i++)
        {
            if (this._uiShowList[i].state != PopState.FINISHED)
            {
                if (url == this._uiShowList[i].Url)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 打开的弹窗数量
    /// </summary>
    /// <returns></returns>
    public int getOpendNum()
    {
        int count = 0;
        for (var i = 0; i < this._uiShowList.Count; i++)
        {
            if (this._uiShowList[i].state != PopState.FINISHED)
            {
                count += 1;
            }
        }

        return count;
    }

    /// <summary>
    /// 是否有弹窗正在显示
    /// </summary>
    /// <returns></returns>
    public bool isPopupShowing()
    {
        for (var i = 0; i < this._uiShowList.Count; i++)
        {
            if (this._uiShowList[i].state < PopState.FINISHED)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 是否还存在弹窗。
    /// </summary>
    /// <returns></returns>    
    public bool havePopup()
    {
        return this._uiShowList.Count > 0 || this._uiQueue.Count > 0;
    }


    public void update()
    {
        if (this._uiShowList.Count > 0)
        {

            // 先看看有没有需要再次激活显示的窗口
            for (var i = 0; i < this._uiShowList.Count; ++i)
            {
                this._uiShowList[i].enableInstance();
            }

            // 再看看有没有需要完全移除掉的窗口
            var throwCnt = 0;
            for (var i = 0; i < this._uiShowList.Count; ++i)
            {
                var uiInstance = this._uiShowList[i];

                // 移除处理完成的实体，并取出新的实体
                if (uiInstance.state == PopState.FINISHED)
                {
                    throwCnt += 1;
                }
                else
                {

                    // 弹窗弹出出错处理
                    if (uiInstance.state == PopState.ERROR)
                    {
                        uiInstance.killTheInstance(null);
                    }

                    // 弹窗自动关闭检测
                    if (uiInstance.uiComp != null && uiInstance.uiComp.canAutoClose(Time.deltaTime))
                    {
                        uiInstance.killTheInstance(null);
                    }

                    if (throwCnt > 0)
                    {
                        this._uiShowList[i - throwCnt] = this._uiShowList[i];
                    }
                }
            }

            this._uiShowList.RemoveRange(this._uiShowList.Count - 1 - throwCnt, throwCnt);
        }

        // 走到这里，说明当前显示列表为空，检查显示队列里面是否有需要显示的窗口，进行显示
        if (this._uiQueue.Count > 0)
        {
            // 当前有弹窗显示,则只做预加载
            // let insArray : Array<PopInstance> = this._uiQueue

            if (this._uiShowList.Count > 0)
            {
                for (var i = 0; i < this._uiQueue.Count; ++i)
                {
                    var uiInstane = this._uiQueue[i];
                    if (uiInstane.state >= PopState.READY)
                    {
                        continue;
                    }
                    else
                    {
                        if (uiInstane.state == PopState.INIT)
                        {
                            break;
                        }
                        else
                        {
                            uiInstane.initInstance(null, true);
                        }
                    }
                }
                // 当前无弹窗显示，加载并回调
            }
            else
            {
                var uiInstance = this._uiQueue[0];

                // 已经有显示则return
                if (this._isPopDisplayed(uiInstance.Url))
                { // 如果已经展示，则忽略
                    uiInstance = null;
                    return;
                }

                // 资源已经准备好的就直接显示
                if (uiInstance.state == PopState.READY)
                {
                    if (uiInstance.uiNode && this._uiRoot)
                    {
                        uiInstance.uiNode.transform.SetParent(this._uiRoot.transform);
                        uiInstance.uiNode.transform.SetSiblingIndex(ConstDesignSize.PopupConstIndex);
                        this._uiShowList.Add(uiInstance);
                        this._uiQueue.RemoveAt(0);
                    }
                }
                else
                {
                    // 还未进行初始化则进行初始化
                    if (uiInstance.state < PopState.INIT)
                    {
                        uiInstance.initInstance((instance) =>
                        {
                            if (uiInstance.uiNode && this._uiRoot)
                            {
                                uiInstance.uiNode.transform.SetParent(this._uiRoot.transform);
                                uiInstance.uiNode.transform.SetSiblingIndex(ConstDesignSize.PopupConstIndex);
                            }
                        });

                        this._uiShowList.Add(uiInstance);
                        this._uiQueue.RemoveAt(0);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 是否有相同弹窗在队列中
    /// </summary>
    /// <returns></returns>
    private bool _isPopInQueue(string url)
    {
        bool value = false;
        foreach (PopInstance instance in this._uiQueue)
        {
            if (instance.Url == url)
            {
                value = true;
            }
        }
        return value;
    }

    /// <summary>
    /// 是否有相同弹窗正在展示
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private bool _isPopDisplayed(string url)
    {
        for (var i = 0; i < this._uiShowList.Count; ++i)
        {
            if (this._uiShowList[i].state != PopState.FINISHED && this._uiShowList[i].state != PopState.ERROR && this._uiShowList[i].isSamePopup(url))
            {
                return true;
            }
        }
        return false;
    }
}

