using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FragMoveButtonCtrl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Game_Direction direction;
    private bool _onPointerDownStart = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (_onPointerDownStart)
        {
            return;
        }

        _onPointerDownStart = true;
        Debug.Log("===>>> frog move OnPointerDown dire :"+direction);
        EventCenter.PostEvent<Game_Direction,bool>(Game_Event.FragGameDirection, direction,false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _onPointerDownStart = false;
        Debug.Log("===>>> frog move OnPointerUp dire :"+direction);
        EventCenter.PostEvent<Game_Direction,bool>(Game_Event.FragGameDirection, Game_Direction.None,false);
    }

    private void OnEnable()
    {
        // throw new NotImplementedException();
    }
}
