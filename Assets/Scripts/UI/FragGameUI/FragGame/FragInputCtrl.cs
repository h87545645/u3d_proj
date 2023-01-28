using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragInputCtrl : MonoBehaviour
{
    public FragJumpButtonCtrl jump;

    public ScrollRocker moveRocker;

    // public FragMoveButtonCtrl left;
    //
    // public FragMoveButtonCtrl right;

    public bool inputEnable = false;
    
    // private float _upWaitTime  = 0f;


    private void Start()
    {
        EventCenter.AddListener(Game_Event.FragStanding, this.OnFragStanding);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(Game_Event.FragStanding , this.OnFragStanding);
    }

    // Update is called once per frame
    void Update()
    {
        if (!inputEnable)
        {
            return;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            // Debug.Log("left arrow");
            // left.OnPointerDown(null);
            EventCenter.PostEvent<Game_Direction,bool>(Game_Event.FragGameDirection, Game_Direction.Left,false);
        }else if ( Input.GetKeyUp(KeyCode.LeftArrow))
        {
            // Debug.Log("-------------------left key up  :");
            // left.OnPointerUp(null);
            EventCenter.PostEvent<Game_Direction,bool>(Game_Event.FragGameDirection, Game_Direction.None,false);
        } 
        if (Input.GetKey(KeyCode.RightArrow)  /*&& !Input.GetKey(KeyCode.RightArrow)*/ )
        {
            // right.OnPointerDown(null);
            EventCenter.PostEvent<Game_Direction, bool>(Game_Event.FragGameDirection, Game_Direction.Right, false);
        }else if (Input.GetKeyUp(KeyCode.RightArrow)/*&& !Input.GetKey(KeyCode.LeftArrow)*/)
        {
            // Debug.Log("-------------------right key up  :");
            // right.OnPointerUp(null);
            EventCenter.PostEvent<Game_Direction,bool>(Game_Event.FragGameDirection, Game_Direction.None,false);
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump.OnPointerDown(null);
        }else if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("-------------------space key up  :");
            jump.OnPointerUp(null);
        }

    }

    public void EnableInput(bool enable)
    {
        Debug.Log("===>>> EnableInput EnableInput start GetInstanceID: " +this.GetInstanceID());

        inputEnable = enable;
        Debug.Log("===>>> EnableInput inputEnable inputEnable : "+ inputEnable);
        jump.enabled = enable;
        moveRocker.enabled = enable;
        // left.enabled = enable;
        // right.enabled = enable;
    }

    public void OnMenuBtn()
    {
        Debug.Log("===>>> EnableInput GetInstanceID : " +this.GetInstanceID());
        if (!inputEnable)
        {
            return;
        }
        UIManager.GetInstance().ShowPanel<FragSettingPanel>("FragSettingPanel", E_UI_Layer.Tip);
    }

    private void OnFragStanding()
    {
        moveRocker.resetDir();
    }
}
