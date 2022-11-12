using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = System.Random;

public class Pelican : DialogBase
{
    public SpriteRenderer pelicanRenderer;
    public PelicanAnimController animController;
    public Transform standPos;
    public float flySpeed = 1;

    private bool isFlying = false;
    
    // private delegate void PelicanCallBack();

    private Action _calllback;

    [HideInInspector] public bool isVisible;

    protected new void Awake()
    {
        base.Awake();
        if (animController == null)
        {
            animController = GetComponent<PelicanAnimController>();
        }
    }

    private void Start()
    {
        // StartCoroutine(UnityUtils.DelayFuc(() =>
        // {
        //     string test = GameMgr.GetInstance().langMgr.getValue("^game_guide1");
        //     Speak(test);
        // },1));
    }

    private void OnBecameVisible()
    {
        isVisible = true;
    }

    private void OnBecameInvisible()
    {
        isVisible = false;
    }

    private Transform GetNearestPos(Transform target)
    {
        float minDis = int.MaxValue;
        int idx = 0;
        for (int i = 0; i < standPos.childCount; i++)
        {
            float dis = Vector3.Distance(target.position,standPos.GetChild(i).position);
            if (minDis > dis)
            {
                idx = i;
                minDis = dis;
            }
        }

        Transform finalPos;
        Transform area = standPos.GetChild(idx);
        if (area.childCount > 1)
        {
            Random rd = new Random();
            int finalIdx = rd.Next(area.childCount) % 10;
            finalPos = area.GetChild(Math.Clamp(finalIdx,0,area.childCount-1));
        }
        else
        {
            finalPos = area.GetChild(0);
        }
        return finalPos;
    }

    public override void Speak(string sentence)
    {
        animController.Speak();
        base.Speak(sentence);
    }

    protected override void FinishedDialog()
    {
        animController.FinishSpeak();
        base.FinishedDialog();
        if (_calllback != null)
        {
            _calllback();
            _calllback = null;
        }
    }

    public void RandomSpeak()
    {
        Random rd = new Random();
        int randomIndex = rd.Next(20);
        string text = GameMgr.GetInstance().langMgr.getValue("^game_random"+randomIndex);
        Speak(text);
    }

    public void GuideSpeak(Action callback)
    {
        _calllback = callback;
        for (int i = 1; i <= 3; i++)
        {
            string str = GameMgr.GetInstance().langMgr.getValue("^game_guide"+i);
            Speak(str);
        }
    }


    public void LookAt(Transform pos)
    {
        pelicanRenderer.flipX = pos.position.x < transform.position.x ? true : false;
    }
    
    public IEnumerator FlyTo(Transform pos , Action action = null)
    {
        isFlying = true;
        LookAt(pos);
        float dis = Vector3.Distance(pos.position,transform.position);
        float time = dis / flySpeed;
        animController.Fly();
        transform.DOMove(pos.position,time,true);
        yield return new WaitForSeconds(time);
        animController.FlyEnd();
        isFlying = false;
        if (action != null)
        {
            action();
        }
    }

    public void FlyToPlayer(Transform target, Action action = null)
    {
        Transform nearPos = GetNearestPos(target);
        if (isFlying)
        {
            transform.DOKill();
        }
        StartCoroutine(FlyTo(nearPos, action));
    }
}
