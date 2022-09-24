using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoMgr : MonoSingletonBase<MonoMgr>
{
    private event UnityAction event_updateFun;

    private void Update()
    {
        if (event_updateFun != null)
        {
            event_updateFun();
        }
    }

    public void AddUpdateFunListener(UnityAction fun_update)
    {
        event_updateFun += fun_update;
    }

    public void RemoveUpdateFunListener(UnityAction fun_update)
    {
        event_updateFun -= fun_update;
    }
}
