using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIBlockWaitMgr : MonoSingletonBase<UIBlockWaitMgr>
{

    public CanvasGroup canvasGroup;

    public Transform rotate;
    
    private Dictionary<string , int> waitDict;

    private double waitTime;
    

    protected override void Awake()
    {
        base.Awake();
        GameObject.DontDestroyOnLoad(gameObject);
        waitDict = new Dictionary<string , int>();
    }

    private void Update()
    {
        if (waitDict.Count > 0)
        {
            waitTime += Time.deltaTime;
            if (waitTime >= 0.01)
            {
                if (!canvasGroup.blocksRaycasts)
                {
                    canvasGroup.blocksRaycasts = true;
                    canvasGroup.alpha = 0.7f;
                    rotate.gameObject.SetActive(true);
                    // rotate.eulerAngles = new Vector3(0f,0f,rotate.eulerAngles.z += Time.deltaTime * 10f) ;
                    
                }
                else
                {
                    rotate.Rotate(0f,0f, -100f*Time.deltaTime,Space.Self);
                }
            }
        }
    }

    public void StartWait(string url)
    {
        waitDict.TryAdd(url, 0);
        waitDict[url]++;
    }

    public void FinishWait(string url)
    {
        if (waitDict.ContainsKey(url))
        {
            waitDict[url]--;
            if (waitDict[url] == 0)
            {
                waitDict.Remove(url);
            }

            CheckWaitFinished();
        }
    }

    private void CheckWaitFinished()
    {
        if (waitDict.Count == 0)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0f;
            rotate.gameObject.SetActive(false);
            rotate.eulerAngles = new Vector3(0f,0f,0f) ;
            waitTime = 0f;
        }
    }
}
