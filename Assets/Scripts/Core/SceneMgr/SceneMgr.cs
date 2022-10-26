using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

//?????л????
public class SceneMgr : SingletonBase<SceneMgr>
{
    /// <summary>
    /// 切换场景
    /// </summary>
    /// <param name="sName">场景名</param>
    /// <param name="fun_temp">加载场景完成后的回调方法</param>
    public void LoadScene(string sName, UnityAction fun_temp)
    {
        //场景同步加载
        SceneManager.LoadScene(sName);
        //加载完成过后才会执行func
        if (fun_temp != null)
        {
            fun_temp();
        }
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="name">场景名</param>
    /// <param name="fun_temp">加载场景完成后的回调方法</param>
    public void LoadSceneAsync(string name, UnityAction fun_temp)
    {
        //????Mono???
        MonoMgr.GetInstance().StartCoroutine(ILoadSceneAsync(name, fun_temp));
    }

    private IEnumerator ILoadSceneAsync(string name, UnityAction fun_temp)
    {
        AsyncOperation obj_ao = SceneManager.LoadSceneAsync(name);
        while (!obj_ao.isDone)
        {
            //像事件中心分发进度情况
            EventCenter.PostEvent<float>(Game_Event.SceneLoading, obj_ao.progress);
            //挂起一帧
            yield return obj_ao.progress;
        }
        //加载完成后执行func
        fun_temp();
    }
}
