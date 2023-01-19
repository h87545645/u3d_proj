using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FragSettingPanel : PanelBase
{
    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("===>>> FragSettingPanel start panel go :"+transform.parent.name);
        // Debug.Log("===>>> FragSettingPanel start panel trans :",transform);
        GetControl<Button>("BackButton").onClick.AddListener(() =>
        {
            UIManager.GetInstance().HidePanel(gameObject.name);
            SceneMgr.GetInstance().LoadScene("FragMenuScene",null);
        });
        
        
        GetControl<Button>("ExitButton").onClick.AddListener(() =>
        {
            Application.Quit();
        });
        
        GetControl<Button>("CloseButton").onClick.AddListener(() =>
        {
            UIManager.GetInstance().HidePanel(gameObject.name);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
