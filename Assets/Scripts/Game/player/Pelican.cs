using System;
using System.Collections.Generic;
using UnityEngine;

public class Pelican : MonoBehaviour
{
    public SpriteRenderer PelicanRenderer;
    
    private float _speed = 1;    //显示的速度.
    private Queue<string> _sentenceQueue = new Queue<string>();
    private string _currentSentence = string.Empty;


    /// <summary>
    /// 文本打字机.
    /// </summary>
    private void ShowText()
    {
        //如果计数长度(显示速度) < 文本长度，则运行，否则停止Invoke调用当前方法.
        if (_speed == 1)
        {
            _speed += Time.deltaTime * 2;    //每次调用增加计数.
 
            //m_LogingText为 需要显示的Text文本的物体.
            // m_LogingText.GetComponent<TextMesh>().text = str.Substring(0, (int)_speed);
            //Substring(0,2)方法：截取字符串，从下标为0的位置截取2个字符.
        }
        else
        {
            //停止Invoke调用方法.
            CancelInvoke();
        }
    }

    /// <summary>
    /// 显示文字.
    /// </summary>
    public void Speak(string sentence)
    {
        _sentenceQueue.Enqueue(sentence);
        //重复调用 文字打印机 方法.
        InvokeRepeating("ShowText", 0, Time.deltaTime * 0.01f);

    }


}
