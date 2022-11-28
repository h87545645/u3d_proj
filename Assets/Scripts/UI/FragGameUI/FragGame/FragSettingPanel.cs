using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FragSettingPanel : PanelBase
{
    // Start is called before the first frame update
    void Start()
    {
        GetControl<Button>("BackButton").onClick.AddListener(() =>
        {
            SceneMgr.GetInstance().LoadScene("FragMenuScene",null);
        });
        
        
        GetControl<Button>("ExitButton").onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
