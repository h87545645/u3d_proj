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
        //����Canvas�������������ʱ�򲻱��Ƴ�
        GameObject.DontDestroyOnLoad(uObj_uiRoot);

        //�ҵ�����
        uObj_TipLayer = uTrans_uiRoot.Find("uObj_tip");
        uObj_TopLayer = uTrans_uiRoot.Find("uObj_top");
        uObj_BotLayer = uTrans_uiRoot.Find("uObj_bot");

        //����EventSystem������������ť�����������Ӧ
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

        if (dict_allPanel.ContainsKey(sPanelName))
        {
  
            dict_allPanel[sPanelName].ShowUI();
            if (callback != null)
                callback(dict_allPanel[sPanelName] as T);
            return;
        }
        //start wait rotate
        UIBlockWaitMgr.GetInstance().StartWait(sPanelName);
        PrefabLoadMgr.I.LoadAsync(sPanelName, (string path  ,GameObject uObj_temp) => {
  
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

            uObj_temp.transform.SetParent(uObj_father);

            uObj_temp.transform.localPosition = Vector3.zero;
            uObj_temp.transform.localScale = Vector3.one;

            (uObj_temp.transform as RectTransform).offsetMax = Vector2.zero;
            (uObj_temp.transform as RectTransform).offsetMin = Vector2.zero;

            uObj_temp.GetComponent<PanelBase>().ShowUI();


            T panel_temp = uObj_temp.GetComponent<T>();

 
            if (callback != null)
            {
                callback(panel_temp);
            }


            dict_allPanel.Add(sPanelName, panel_temp);
            
            //finish wait
            UIBlockWaitMgr.GetInstance().FinishWait(sPanelName);
        });
    }

    //�������
    public void HidePanel(string sPanelName)
    {
        if (dict_allPanel.ContainsKey(sPanelName))
        {
  
            dict_allPanel[sPanelName].HideUI();
            GameObject.Destroy(dict_allPanel[sPanelName].gameObject);
            dict_allPanel.Remove(sPanelName);
        }
    }


    public T GetPanel<T>(string sName) where T : PanelBase
    {
        if (dict_allPanel.ContainsKey(sName))
        {
            return dict_allPanel[sName] as T;
        }
        return null;
    }


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