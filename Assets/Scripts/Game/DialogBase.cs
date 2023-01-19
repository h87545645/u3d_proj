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
    public float speed = 1;    //��ʾ���ٶ�.
    public float interval = 100;//���仰�м���
    
    private bool _isDialogShowing = false; //��ǰ�Ƿ�������ʾ�Ի���
    private Queue<string> _sentenceQueue = new Queue<string>();
    private string _currentSentence = string.Empty;
    private float _curInterval = 0;
    private float _curSentenceCnt = 0;
    private Vector3 _originScale = new Vector3(1, 1, 1);


    protected void Awake()
    {
        if (dialogGo == null)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                PrefabLoadMgr.I.LoadAsync("dialogprefab", (string name, GameObject obj) =>
                {
                    dialogGo = obj;
                    text = dialogGo.GetComponentInChildren<TextMeshProUGUI>();
                    dialoImage = dialogGo.transform.GetChild(0).Find("Image");
                    _originScale = new Vector3(1/transform.localScale.x,1/transform.localScale.y,1/transform.localScale.z);
                    dialoImage.localScale = _originScale;
                    dialogGo.SetActive(false);
                } ,transform);
         
            }
            else
            {
                dialogGo = PrefabLoadMgr.I.LoadSync("dialogprefab", transform);
                text = dialogGo.GetComponentInChildren<TextMeshProUGUI>();
                dialoImage = dialogGo.transform.GetChild(0).Find("Image");
                _originScale = new Vector3(1/transform.localScale.x,1/transform.localScale.y,1/transform.localScale.z);
                dialoImage.localScale = _originScale;
                dialogGo.SetActive(false);
            }
          
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /// <summary>
    /// ��ʾ����.
    /// </summary>
    public virtual void Speak(string sentence)
    {
        _sentenceQueue.Enqueue(GetNstring(sentence));
        //�ظ����� ���ִ�ӡ�� ����.
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
    /// ��str����Ϊ������"/n"�������
    /// </summary>
    /// <param name="str">��Ҫת����str</param>
    /// <returns>�����е�Str</returns>
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
    /// �ı����ֻ�.
    /// </summary>
    private void ShowText()
    {
        //�����������(��ʾ�ٶ�) < �ı����ȣ������У�����ֹͣInvoke���õ�ǰ����.
        if (_curSentenceCnt < _currentSentence.Length)
        {
            _curSentenceCnt += Time.deltaTime * speed;    //ÿ�ε������Ӽ���.

            //m_LogingTextΪ ��Ҫ��ʾ��Text�ı�������.
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
                    //ֹͣInvoke���÷���.
                    
                    
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
