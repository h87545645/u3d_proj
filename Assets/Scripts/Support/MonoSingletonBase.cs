using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 继承自 MonoBehaviour 的单例基类
/// </summary>
/// <typeparam name="T"></typeparam>
public class MonoSingletonBase<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T GetInstance()
    {
        if(_instance == null)
        {
            GameObject uObj_cur = new GameObject();
            uObj_cur.name = typeof(T).ToString();
            _instance = uObj_cur.AddComponent<T>();
        }
        return _instance;
    }

    protected virtual void Awake() 
    {
        if(_instance != null)
        {
            Debug.LogError("存在多个Mono单例 : " + _instance.GetType().ToString());
            // Destroy(this.GetComponent<T>());
            return;
        }
        _instance = this as T;
    }
}