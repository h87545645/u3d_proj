using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FragJumpButtonCtrl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{

    private float LastTime = 0;


    public void OnPointerDown(PointerEventData eventData)
    {
        LastTime = Time.time;
        Debug.Log("按下");
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (LastTime > 0 && Time.time - LastTime > 0.1)
        {
            EventCenter.PostEvent<float>(Game_Event.FragGameJump, Time.time - LastTime);
            Debug.Log("抬起");
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        LastTime = 0;
        Debug.Log("离开");
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("进入");
    }
 
}
