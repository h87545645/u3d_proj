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
    public I18NTextMesh playerRecordText;

    protected override void Awake()
    {
        base.Awake();
        
    }

    private void Start()
    {
        GetControl<Button>("StartButton").onClick.AddListener(() =>
        {
            // clear player game record
            // FragGameRecord.GetInstance().reocrd.playerPosition = new Vector2(0, 0);
            // FragGameRecord.GetInstance().reocrd.playerAlreadyGuide = false;
            FragGameRecord.GetInstance().reocrd = new FragGameRecord.PlayerRecord();
            Debug.Log("===>>> platform : " + Application.platform);
            LoadGameScene();
        });
        
        GetControl<Button>("ContinueButton").onClick.AddListener(() =>
        {
            // SceneMgr.GetInstance().LoadScene("FragGameScene",null);
            LoadGameScene();
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


        InitPlayerRecordUI();
        InitButton();
    }

    private void LoadGameScene()
    {
        // AssetsLoadMgr.I.LoadAsync("FragGameScene", (string name, UnityEngine.Object obj) =>
        // {
        //     SceneMgr.GetInstance().LoadScene("FragGameScene",null);
        // });
        AssetBundleLoadMgr.I.LoadAsync("FragGameScene",(AssetBundle _ab) =>
        {
            SceneMgr.GetInstance().LoadScene("FragGameScene",null);
        });
    }

    private void InitPlayerRecordUI()
    {
        string[] record = new string[]
        {
            UnityUtils.TimeToStringHMS(FragGameRecord.GetInstance().reocrd.playerTotalTime),
            FragGameRecord.GetInstance().reocrd.jumpCnt.ToString(),
            FragGameRecord.GetInstance().reocrd.isCompleted ? GameMgr.GetInstance().langMgr.getValue("^game_completed") :FragGameRecord.GetInstance().reocrd.heightRecord.ToString()
        };
        playerRecordText._params = record;
        playerRecordText.updateTranslation();
    }

    private void InitButton()
    {
        if (!FragGameRecord.GetInstance().reocrd.playerAlreadyGuide)
        {
            GetControl<Button>("ContinueButton").gameObject.SetActive(false);
        }
    }
}
