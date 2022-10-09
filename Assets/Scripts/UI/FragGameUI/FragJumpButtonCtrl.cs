using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FragJumpButtonCtrl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    private float LastTime = 0;


    public void OnPointerDown(PointerEventData eventData)
    {
        EventCenter.PostEvent(Game_Event.FragGameCharge);
        LastTime = Time.time;
        //Debug.Log("按下");
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (LastTime > 0 && Time.time - LastTime > 0.05)
        {
            EventCenter.PostEvent<float>(Game_Event.FragGameJump, Time.time - LastTime);
            //Debug.Log("抬起");
        }
        else
        {
            LastTime = 0;
            EventCenter.PostEvent(Game_Event.FragGameChargeCancel);
        }
    }
    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    LastTime = 0;
    //    EventCenter.PostEvent(Game_Event.FragGameChargeCancel);
    //    //Debug.Log("离开");
    //}
    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    //Debug.Log("进入");
    //}
 
}
