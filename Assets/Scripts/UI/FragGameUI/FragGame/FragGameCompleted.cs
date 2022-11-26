using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FragGameCompleted : PanelBase
{
    // Start is called before the first frame update
    void Start()
    {
        GetControl<Button>("continueBtn").onClick.AddListener(() =>
        {
            
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnFadeIn()
    {
        
    }
    
    
}
