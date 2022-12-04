using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FragGameController : MonoBehaviour
{
    public FragGameCameraCtrl fragGameCameraCtrl;

    public FragHero fragHero;

    public Pelican pelican;

    public Light2D globalLight;

    public FragGameCompleted completedPanel;

    public FragInputCtrl inputCtrl;

    // private long _totalPlayTime = 0;

    // private float _curLevelTime = 0;
    //
    // private bool _levelLongStay = false;

    // private bool _isAlreadyGuide = true;

    private bool _isCompleted = false;
    
    
    //???????????????
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            FragGameRecord.GetInstance().SetRecordItem();
            RecordUtil.Save();
        }
    }
    
    //??????????????
    private void OnApplicationQuit()
    {
        FragGameRecord.GetInstance().SetRecordItem();
        RecordUtil.Save();
    }

    private void Awake()
    {
        EventCenter.AddListener<int>(Game_Event.FragGameCameraMove,OnCameraMove);
        EventCenter.AddListener(Game_Event.FragGameFinish,OnPlayerCompleted);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (FragGameRecord.GetInstance().reocrd.playerAlreadyGuide)
        {
            StartGame();
        }
        else
        {
            StartGuide();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isCompleted)
        {
            FragGameRecord.GetInstance().reocrd.playerTotalTime += Time.deltaTime;
        }
        
        // _curLevelTime += Time.deltaTime;
        // if (!_levelLongStay)
        // {
        //     _curLevelTime += Time.deltaTime;
        // }
    }

    private void StartGuide()
    {
        EventCenter.PostEvent<bool>(Game_Event.FragActiveAllUI,false);
        fragGameCameraCtrl.enable = false;
        fragHero.heroRigidbody2D.mass = 0;
        fragHero.OnGuide();
        StartCoroutine(UnityUtils.DelayFuc(() =>
        {
            pelican.LookAt(fragHero.heroRigidbody2D.transform);
            pelican.GuideSpeak(() =>
            {
                fragHero.guideAnim.gameObject.SetActive(false);
                FragGameRecord.GetInstance().reocrd.playerAlreadyGuide = true;
                StartGame();
            });
        },3));
    }

    private void StartGame()
    {
        inputCtrl.EnableInput(true);
        fragHero.heroRigidbody2D.gameObject.SetActive(true);
        fragHero.SetRecordPos();
        EventCenter.PostEvent<bool>(Game_Event.FragActiveAllUI,true);
        fragGameCameraCtrl.enable = true;
        fragHero.heroRigidbody2D.mass = 2;
        InvokeRepeating(nameof(OnPelicanSpeak), 35, 35);
    }


    private void OnPelicanSpeak()
    {
        pelican.RandomSpeak();
    }

    private void OnCameraMove(int index)
    {
        // _curLevelTime = 0;
        // _levelLongStay = false;
        StartCoroutine(UnityUtils.DelayFuc(() =>
        {
            if (!pelican.isVisible)
            {
                pelican.FlyToPlayer(fragHero.heroRigidbody2D.transform);
            }
        }, 5));
       
        switch (index)
        {
            case 6:
            {
                fragHero.OnLight(true);
                globalLight.intensity = 0.2f;
                break;
            }
            case 7:
            {
                globalLight.intensity = 0f;
                break;
            }
            case 8:
            {
                globalLight.intensity = 0.5f;
                break;
            }
            default:
            {
                fragHero.OnLight(false);
                break;
            }
        }
    }

    private void OnPlayerCompleted()
    {
        if (!_isCompleted)
        {
            inputCtrl.EnableInput(false);
            FragGameRecord.GetInstance().history.jumpCnt = FragGameRecord.GetInstance().reocrd.jumpCnt;
            FragGameRecord.GetInstance().history.playerTotalTime = FragGameRecord.GetInstance().reocrd.playerTotalTime;
            _isCompleted = true;
            fragHero.RemoveListener();
            pelican.FlyToPlayer(fragHero.heroRigidbody2D.transform, () =>
            {
                pelican.Speak(GameMgr.GetInstance().langMgr.getValue("^game_completed_speak"));
                StartCoroutine(UnityUtils.DelayFuc(() =>
                {
                    completedPanel.OnFadeIn();
                },3));
            });
        }
    }
}
