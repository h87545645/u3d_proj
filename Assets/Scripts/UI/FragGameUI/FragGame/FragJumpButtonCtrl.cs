using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FragJumpButtonCtrl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    private float _lastTime = 0;

    private bool _onPointerDownStart = false;

    private void Update()
    {
        if (_lastTime > 0 && Time.time - _lastTime > GlobalValue.JumpMaxChargeTime)
        {
            EventCenter.PostEvent<float>(Game_Event.FragGameJump, 1);
            _lastTime = 0;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_onPointerDownStart)
        {
            return;
        }

        _onPointerDownStart = true;
        EventCenter.PostEvent(Game_Event.FragGameCharge);
        _lastTime = Time.time;
        Debug.Log("===>>> frog jump OnPointerDown");
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (_lastTime > 0 && Time.time - _lastTime > 0.05)
        {   
            Debug.Log("===>>> frog jump OnPointerUp succ");
            EventCenter.PostEvent<float>(Game_Event.FragGameJump, Time.time - _lastTime);
            _lastTime = 0;
            //Debug.Log("̧��");
        }
        else
        {
            if (_lastTime > 0)
            {
                Debug.Log("===>>> frog jump OnPointerUp cancel");
                _lastTime = 0;
                EventCenter.PostEvent(Game_Event.FragGameChargeCancel);
            }
        }
        _onPointerDownStart = false;
    }
    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    LastTime = 0;
    //    EventCenter.PostEvent(Game_Event.FragGameChargeCancel);
    //    //Debug.Log("�뿪");
    //}
    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    //Debug.Log("����");
    //}
    
    
 
}
