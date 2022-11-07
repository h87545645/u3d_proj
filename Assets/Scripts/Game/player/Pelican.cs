using System;
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
    public Light2D globalLight;
    

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
        StartCoroutine(UnityUtils.DelayFuc(() =>
        {
            string test = GameMgr.GetInstance().langMgr.getValue("^game_guide1");
            Speak(test);
        },1));
    }

    private void OnBecameVisible()
    {
        isVisible = true;
    }

    private void OnBecameInvisible()
    {
        isVisible = false;
        float minDis = int.MaxValue;
        int idx = 0;
        for (int i = 0; i < standPos.childCount; i++)
        {
            float dis = Vector3.Distance(Camera.main.transform.position,standPos.GetChild(i).position);
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
    }


    public void FlyTo(Transform pos)
    {
        pelicanRenderer.flipX = pos.position.x < transform.position.x ? true : false;
        transform.DOMove(pos.position,1,true);
    }
}
