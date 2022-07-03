using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaitTimeManager
{
    private static TaskBehaviour m_task;
    static WaitTimeManager()
    {
        GameObject go = new GameObject("#WaitTimeManager#");
        GameObject.DontDestroyOnLoad(go);
        m_task = go.AddComponent<TaskBehaviour>();
    }

    //等待
    static public Coroutine WaitTime(float time , UnityAction callback)
    {
        return m_task.StartCoroutine(Coroutine(time, callback));
    }

    //取消等待
    static public void CancelWait(ref Coroutine coroutine)
    {
        if(coroutine != null)
        {
            m_task.StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    static IEnumerable Coroutine(float time, UnityAction callback)
    {
        yield return new WaitForSeconds(time);
        if(callback != null)
        {
            callback();
        }
    }


    class TaskBehaviour : MonoBehaviour
    {
        internal Coroutine StartCoroutine(IEnumerable enumerable)
        {
            throw new NotImplementedException();
        }
    }
}
