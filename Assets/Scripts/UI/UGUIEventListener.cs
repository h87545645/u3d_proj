
using UnityEngine;
using UnityEngine.Events;

public class UGUIEventListener : UnityEngine.EventSystems.EventTrigger
{
    public UnityAction<GameObject> onClick;
    public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (onClick != null)
        {
            onClick(gameObject);
        }
    }

    /// <summary>
    /// ��ȡ������� UGUIEventListener �ű���ʵ�ֶ���Ϸ�������
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    static public UGUIEventListener Get(GameObject go)
    {
        UGUIEventListener listener = go.GetComponent<UGUIEventListener>();
        if (listener == null)
        {
            listener = go.AddComponent<UGUIEventListener>();
        }
        return listener;
    }
}
