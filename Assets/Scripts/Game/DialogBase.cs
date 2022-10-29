using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DialogBase : MonoBehaviour
{
    public GameObject dialogGo;
    public Transform dialoImage;
    public TextMeshProUGUI text;
    public float speed = 1;    //显示的速度.
    public float interval = 100;//两句话中间间隔
    
    private bool _isDialogShowing = false; //当前是否正在显示对话框
    private Queue<string> _sentenceQueue = new Queue<string>();
    private string _currentSentence = string.Empty;
    private float _curInterval = 0;
    private float _curSentenceCnt = 0;
    private Vector3 _originScale = new Vector3(1, 1, 1);


    protected void Awake()
    {
        if (dialogGo == null)
        {
            dialogGo = PrefabLoadMgr.I.LoadSync("dialogprefab", transform);
            text = dialogGo.GetComponentInChildren<TextMeshProUGUI>();
            dialoImage = dialogGo.transform.GetChild(0).Find("Image");
            _originScale = new Vector3(1/transform.localScale.x,1/transform.localScale.y,1/transform.localScale.z);
            dialoImage.localScale = _originScale;
            dialogGo.SetActive(false);
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /// <summary>
    /// 显示文字.
    /// </summary>
    public virtual void Speak(string sentence)
    {
        _sentenceQueue.Enqueue(GetNstring(sentence));
        //重复调用 文字打印机 方法.
        if (!_isDialogShowing)
        {
            _isDialogShowing = true;
            _currentSentence = _sentenceQueue.Dequeue();
            dialogGo.SetActive(true);
            dialoImage.localScale = new Vector3(0, 0, 0);
            dialoImage.DOScale(_originScale,0.5f);
            InvokeRepeating("ShowText", 0, Time.deltaTime * 0.01f);
        }
    }
    
    /// <summary>
    /// 将str更换为带换行"/n"的输入框
    /// </summary>
    /// <param name="str">需要转换的str</param>
    /// <returns>带换行的Str</returns>
    private string GetNstring(string str)
    {
        string result = "";
        int point = 0;
        for (int i = 0; i < str.Length; i++)
        {
            if (point == 20)
            {              
                result += '\n';
                point = 0;
            }
            result += str[i];
            point++;
        }     
        return result;
    }

    /// <summary>
    /// 文本打字机.
    /// </summary>
    private void ShowText()
    {
        //如果计数长度(显示速度) < 文本长度，则运行，否则停止Invoke调用当前方法.
        if (_curSentenceCnt < _currentSentence.Length)
        {
            _curSentenceCnt += Time.deltaTime * speed;    //每次调用增加计数.

            //m_LogingText为 需要显示的Text文本的物体.
            text.text = _currentSentence.Substring(0, (int)_curSentenceCnt);
        }
        else
        {
            _curInterval ++;
            if (_curInterval >= interval)
            {
                _curSentenceCnt = 0;
                _curInterval = 0;
                if (_sentenceQueue.Count > 0)
                {
                    _currentSentence = _sentenceQueue.Dequeue();
                }
                else
                {
                    //停止Invoke调用方法.
                    
                    
                    dialoImage.DOScale(0, 0.2f);
                    StartCoroutine(UnityUtils.DelayFuc(() =>
                    {
                        if (!_isDialogShowing)
                        {
                            dialogGo.SetActive(false);
                        }
                    }, 0.2f));
                    _isDialogShowing = false;
                    CancelInvoke();
                    FinishedDialog();
                }
            }
     
        }
    }

    protected virtual void FinishedDialog()
    {
        
    }
}
