using System;
using System.Collections;
using System.Collections.Generic;
using Honeti;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FragMenuMainUI : PanelBase
{

    public I18N Lang = null;
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
        
        
        GetControl<Button>("LangButton").onClick.AddListener(() =>
        {
            LanguageCode code = Lang.gameLang != LanguageCode.EN ? LanguageCode.EN : LanguageCode.SCN;
            Lang.setLanguage(code);
        });
    }
}
