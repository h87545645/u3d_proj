using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragInputCtrl : MonoBehaviour
{
    public FragJumpButtonCtrl jump;

    public FragMoveButtonCtrl left;

    public FragMoveButtonCtrl right;

    private bool _inputEnable = false;
    
    // private float _upWaitTime  = 0f;

    // Update is called once per frame
    void Update()
    {
        if (!_inputEnable)
        {
            return;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            // Debug.Log("left arrow");
            left.OnPointerDown(null);
        }else if ( Input.GetKeyUp(KeyCode.LeftArrow))
        {
            Debug.Log("-------------------left key up  :");
            left.OnPointerUp(null);

        } 
        if (Input.GetKey(KeyCode.RightArrow)  /*&& !Input.GetKey(KeyCode.RightArrow)*/ )
        {
            right.OnPointerDown(null);
            
        }else if (Input.GetKeyUp(KeyCode.RightArrow)/*&& !Input.GetKey(KeyCode.LeftArrow)*/)
        {
            Debug.Log("-------------------right key up  :");
            right.OnPointerUp(null);
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
        _inputEnable = enable;
        jump.enabled = enable;
        left.enabled = enable;
        right.enabled = enable;
    }

    public void OnMenuBtn()
    {
        if (!_inputEnable)
        {
            return;
        }
        UIManager.GetInstance().ShowPanel<FragSettingPanel>("FragSettingPanel", E_UI_Layer.Tip);
    }
}
