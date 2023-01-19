using System;
using System.Collections;
using System.Collections.Generic;
using Honeti;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FragMenuMainUI : PanelBase
{
    
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
            // SceneMgr.GetInstance().LoadScene("FragGameScene",null);
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
            // PrefabLoadMgr.I.LoadSync("test", transform);
            // return;
            // PrefabLoadMgr.I.LoadAsync("test", (string name, GameObject obj) =>
            // {
            //     // dialogGo = obj;
            //     // text = dialogGo.GetComponentInChildren<TextMeshProUGUI>();
            //     // dialoImage = dialogGo.transform.GetChild(0).Find("Image");
            //     // _originScale = new Vector3(1/transform.localScale.x,1/transform.localScale.y,1/transform.localScale.z);
            //     // dialoImage.localScale = _originScale;
            //     // dialogGo.SetActive(false);
            // } ,transform);
            // return;
            LanguageCode code = GameMgr.GetInstance().langMgr.gameLang != LanguageCode.EN ? LanguageCode.EN : LanguageCode.SCN;
            GameMgr.GetInstance().langMgr.setLanguage(code);
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
