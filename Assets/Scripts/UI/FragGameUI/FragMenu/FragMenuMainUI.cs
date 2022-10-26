using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FragMenuMainUI : PanelBase
{
    private void Start()
    {
        GetControl<Button>("StartButton").onClick.AddListener(() =>
        {
            SceneMgr.GetInstance().LoadScene("FragGameScene",null);
        });
        
        
        GetControl<Button>("ExitButton").onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
