using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIManager : SingletonBase<UIManager>
{
    private Dictionary<string, PanelBase> dict_allPanel = new Dictionary<string, PanelBase>();

    private Transform uObj_TipLayer;

    private Transform uObj_TopLayer;

    private Transform uObj_BotLayer;

    public UIManager()
    {
        GameObject uObj_uiRoot = GameObject.Find("/Canvas");
        if (uObj_uiRoot == null)
        {
            uObj_uiRoot = PrefabLoadMgr.I.LoadSync("Canvas.prefab");
            //uObj_uiRoot = ResLoadMgr.GetInstance().LoadRes<GameObject>("UI/Canvas");
        }

        Transform uTrans_uiRoot = uObj_uiRoot.transform;
        //创建Canvas，让其过场景的时候不被移除
        GameObject.DontDestroyOnLoad(uObj_uiRoot);

        //找到各层
        uObj_TipLayer = uTrans_uiRoot.Find("uObj_tip");
        uObj_TopLayer = uTrans_uiRoot.Find("uObj_top");
        uObj_BotLayer = uTrans_uiRoot.Find("uObj_bot");

        //加载EventSystem，有了它，按钮等组件才能响应
        GameObject uObj_eventSystem = GameObject.Find("/EventSystem");
        if (uObj_eventSystem == null)
        {
            uObj_eventSystem = PrefabLoadMgr.I.LoadSync("EventSystem.prefab");
            //uObj_eventSystem = ResLoadMgr.GetInstance().LoadRes<GameObject>("UI/EventSystem");
        }
        GameObject.DontDestroyOnLoad(uObj_eventSystem);
    }

    public void ShowPanel<T>(string sPanelName, E_UI_Layer eLayerType = E_UI_Layer.Top, UnityAction<T> callback = null) where T : PanelBase
    {
        //已经显示了此面板
        if (dict_allPanel.ContainsKey(sPanelName))
        {
            //调用重写方法，具体内容自己添加
            dict_allPanel[sPanelName].ShowUI();
            if (callback != null)
                callback(dict_allPanel[sPanelName] as T);
            return;
        }

        PrefabLoadMgr.I.LoadAsync(sPanelName, (string path  ,GameObject uObj_temp) => {
            //把它作为Canvas的子对象
            //并且设置它的相对位置
            //找到父对象
            Transform uObj_father = uObj_BotLayer;
            switch (eLayerType)
            {
                case E_UI_Layer.Tip:
                    uObj_father = uObj_TipLayer;
                    break;
                case E_UI_Layer.Top:
                    uObj_father = uObj_TopLayer;
                    break;
            }
            //设置父对象
            uObj_temp.transform.SetParent(uObj_father);

            //设置相对位置和大小
            uObj_temp.transform.localPosition = Vector3.zero;
            uObj_temp.transform.localScale = Vector3.one;

            (uObj_temp.transform as RectTransform).offsetMax = Vector2.zero;
            (uObj_temp.transform as RectTransform).offsetMin = Vector2.zero;

            uObj_temp.GetComponent<PanelBase>().ShowUI();

            //得到预设体身上的脚本（继承自BasePanel）
            T panel_temp = uObj_temp.GetComponent<T>();

            //执行外面想要做的事情
            if (callback != null)
            {
                callback(panel_temp);
            }

            //在字典中添加此面板
            dict_allPanel.Add(sPanelName, panel_temp);
        });
    }

    //隐藏面板
    public void HidePanel(string sPanelName)
    {
        if (dict_allPanel.ContainsKey(sPanelName))
        {
            //调用重写方法，具体内容自己添加
            dict_allPanel[sPanelName].HideUI();
            GameObject.Destroy(dict_allPanel[sPanelName].gameObject);
            dict_allPanel.Remove(sPanelName);
        }
    }

    /// <summary>
    /// 得到某一个已经显示的面板 方便外部使用
    /// </summary>
    public T GetPanel<T>(string sName) where T : PanelBase
    {
        if (dict_allPanel.ContainsKey(sName))
        {
            return dict_allPanel[sName] as T;
        }
        return null;
    }

    /// <summary>
    /// 给控件添加自定义事件监听
    /// </summary>
    /// <param name="uObj_control">控件对象</param>
    /// <param name="eTriggerType">事件类型</param>
    /// <param name="fun_callback">事件的响应函数</param>
    public static void AddCustomEventListener(UIBehaviour uObj_control, EventTriggerType eTriggerType, UnityAction<BaseEventData> fun_callback)
    {
        EventTrigger uObj_trigger = uObj_control.GetComponent<EventTrigger>();
        if (uObj_trigger == null)
        {
            uObj_trigger = uObj_control.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry obj_entry = new EventTrigger.Entry();
        obj_entry.eventID = eTriggerType;
        obj_entry.callback.AddListener(fun_callback);

        uObj_trigger.triggers.Add(obj_entry);
    }
}