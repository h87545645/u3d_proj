using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

//�����л�ģ��
public class SceneMgr : SingletonBase<SceneMgr>
{
    /// <summary>
    /// �л�����
    /// </summary>
    /// <param name="sName">������</param>
    /// <param name="fun_temp">���س�����ɺ�Ļص�����</param>
    public void LoadScene(string sName, UnityAction fun_temp)
    {
        //����ͬ������
        SceneManager.LoadScene(sName);
        //������ɹ���Ż�ִ��func
        fun_temp();
    }

    /// <summary>
    /// �첽���س���
    /// </summary>
    /// <param name="name">������</param>
    /// <param name="fun_temp">���س�����ɺ�Ļص�����</param>
    public void LoadSceneAsync(string name, UnityAction fun_temp)
    {
        //����Monoģ��
        MonoMgr.GetInstance().StartCoroutine(ILoadSceneAsync(name, fun_temp));
    }

    private IEnumerator ILoadSceneAsync(string name, UnityAction fun_temp)
    {
        AsyncOperation obj_ao = SceneManager.LoadSceneAsync(name);
        while (!obj_ao.isDone)
        {
            //���¼����ķַ��������
            EventCenter.PostEvent<float>("Loading", obj_ao.progress);
            //����һ֡
            yield return obj_ao.progress;
        }
        //������ɺ�ִ��func
        fun_temp();
    }
}