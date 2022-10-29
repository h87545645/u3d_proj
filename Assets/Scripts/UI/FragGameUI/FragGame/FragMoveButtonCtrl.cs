using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FragMoveButtonCtrl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Game_Direction direction;
    public void OnPointerDown(PointerEventData eventData)
    {
        EventCenter.PostEvent<Game_Direction,bool>(Game_Event.FragGameDirection, direction,false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EventCenter.PostEvent<Game_Direction,bool>(Game_Event.FragGameDirection, Game_Direction.None,false);
    }
}