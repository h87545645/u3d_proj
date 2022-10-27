using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogBase : MonoBehaviour
{
    public GameObject dialogGo;
    public TextMeshPro text;
    
    
    private float _speed = 1;    //��ʾ���ٶ�.
    private Queue<string> _sentenceQueue = new Queue<string>();
    private string _currentSentence = string.Empty;


    private void Awake()
    {
        if (dialogGo == null)
        {
            dialogGo = PrefabLoadMgr.I.LoadSync("", transform);
            text = dialogGo.GetComponentInChildren<TextMeshPro>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /// <summary>
    /// ��ʾ����.
    /// </summary>
    public void Speak(string sentence)
    {
        _sentenceQueue.Enqueue(sentence);
        //�ظ����� ���ִ�ӡ�� ����.
        InvokeRepeating("ShowText", 0, Time.deltaTime * 0.01f);

    }
}
