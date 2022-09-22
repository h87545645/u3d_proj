using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum RotateState
{
    IDLE = 0,
    START,
    PREPARE,
    SHOWING,
    FINISHED
}



public class UIRotateWait : MonoBehaviour
{

    const float ConstStartWaitDelayTime = 0.75f;
    const float ConstWaitDelayTime = 0.15f;
    const float ConstCloseWaitDelayTime = 0.15f;

    const float ConstAngularSpeed = 150;

    const float ConstNodeOpacityMinVal = 0f;
    const float ConstNodeOpacityMaxVal = 0.7f;
    const float ConstNodeOpacityFullVal = 255f;
    const float ConstDruationRestrain = 0.05f; // 时间约束值


    [SerializeField]
    [Tooltip("mask节点")]
    public GameObject maskNode = null;

    //////////////////////////////////////////////////////////////////////////

    [SerializeField]
    [Tooltip("旋转节点")]
    public GameObject rotateNode = null;

    //////////////////////////////////////////////////////////////////////////
    /// 
    /// 
    [SerializeField]
    [Tooltip("提示文字")]
    public TextMesh showLabel = null;

    //////////////////////////////////////////////////////////////////////////
    /// 
    /// 
    [SerializeField]
    [Tooltip("mask是否适配全屏")]
    public bool adaptMask = true;

    //////////////////////////////////////////////////////////////////////////
    /// 
    /// 

    private RotateState _rotateState = RotateState.IDLE;
    private RotateState _lastState = RotateState.IDLE; // 记录上一个状态

    private float _startWaitTime = 0f; // 启动wait时间
    private float _showWaitTime = 0f; // 显示wait时间
    private float _endWaitTime = 0f; // 隐藏wait时间

    private Dictionary<string, int> _waitShowKeys = new Dictionary<string, int>();

    // Start is called before the first frame update
    void Awake()
    {
        if (this.maskNode != null)
        {
            this.maskNode.GetComponent<CanvasGroup>().alpha = ConstNodeOpacityMinVal;
        }
        if (this.rotateNode != null)
        {
            this.rotateNode.GetComponent<CanvasGroup>().alpha = ConstNodeOpacityMinVal;
        }
        this._adaptePopup();
    }

    public void showBlock()
    {
        if (this.maskNode != null)
        {
            iTween.Stop(this.maskNode, "FadeTo");
            this.maskNode.GetComponent<CanvasGroup>().alpha = ConstNodeOpacityMinVal;
            this.maskNode.SetActive(true);
            iTween.FadeTo(this.maskNode, iTween.Hash("time", 0.35f, "alpha", ConstNodeOpacityMaxVal));

        }
    }

    public void hideBlock()
    {
        if (this.maskNode != null)
        {
            iTween.Stop(this.maskNode, "FadeTo");
            this.maskNode.GetComponent<CanvasGroup>().alpha = ConstNodeOpacityMinVal;
            this.maskNode.SetActive(true);
            iTween.FadeTo(this.maskNode, iTween.Hash("time", 0.35f, "alpha", ConstNodeOpacityMinVal, "oncomplete", "hideComplete"));
        }
    }

    private void hideComplete()
    {
        this.maskNode.SetActive(false);
    }

    public void startWait(string key)
    {
        if (this._rotateState == RotateState.IDLE)
        { // 如果当前处于IDLE状态，则重启wait，开始整个wait的生命周期
            this._startWaitTime = ConstStartWaitDelayTime;
            this._rotateState = RotateState.START;
            this._lastState = RotateState.START;

            if (this.maskNode != null)
            {
                this.maskNode.SetActive(true);
            }
        }

        if (this._waitShowKeys.ContainsKey(key))
        {
            this._waitShowKeys.Add(key, 1);
        }
        else
        {
            Debug.Log("block key is exist!");
        }
    }

    public void finishWait(string key)
    {
        if (this._waitShowKeys.ContainsKey(key))
        {
            this._waitShowKeys.Remove(key);
        }
    }

    public void clearWait()
    {
        this._waitShowKeys.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        var rotate = false;
        switch (this._rotateState)
        {
            case RotateState.IDLE:
                {
                    return;
                }
            case RotateState.START:
                {
                    this._startWaitTime -= Time.deltaTime;
                    if (this._startWaitTime <= 0)
                    {
                        this._showWaitTime = ConstWaitDelayTime;
                        this._rotateState = RotateState.PREPARE;
                        this._showWait(this._showWaitTime);
                    }
                    break;
                }
            case RotateState.PREPARE:
                {
                    rotate = true;
                    this._showWaitTime -= Time.deltaTime;
                    if (this._startWaitTime <= 0)
                    {
                        this._showWaitTime = 0;
                        this._rotateState = RotateState.SHOWING;
                    }
                    break;
                }
            case RotateState.SHOWING:
                {
                    rotate = true;
                    break;
                }
            case RotateState.FINISHED:
                {
                    rotate = true;
                    this._endWaitTime -= Time.deltaTime;
                    if (this._endWaitTime <= 0)
                    {
                        this._endWaitTime = 0;
                        this._rotateState = RotateState.IDLE;
                    }
                    break;
                }
        }
        if (rotate)
        {
            this.rotateNode.transform.Rotate(Vector3.forward, Time.deltaTime * ConstAngularSpeed);
        }

        if (this._waitShowKeys.Keys.Count <= 0)
        {
            if (this._rotateState == RotateState.START || this._rotateState == RotateState.SHOWING)
            {
                this._lastState = this._rotateState;
                this._endWaitTime = ConstWaitDelayTime;
                this._rotateState = RotateState.FINISHED;
                this._closeWait(this._endWaitTime);
            }
            if (this.showLabel != null)
            {
                this.showLabel.text = "";
                this.showLabel.gameObject.SetActive(false);
            }
        }
        else
        {
            // todo 测试环境打开showlabel

            if (this._rotateState == RotateState.FINISHED)
            {
                this._rotateState = this._lastState;
                if (this._rotateState != RotateState.START)
                {
                    this._recoverWait();
                }
            }
        }
    }

    private void _showWait(float showTime)
    {
        var time = showTime - ConstDruationRestrain;
        if (this.maskNode != null)
        {
            this.maskNode.GetComponent<CanvasGroup>().alpha = ConstNodeOpacityMinVal;
            this.maskNode.SetActive(true);
            iTween.FadeTo(this.maskNode, iTween.Hash("time", time, "alpha", ConstNodeOpacityMinVal));
        }

        if (this.rotateNode != null)
        {
            this.rotateNode.GetComponent<CanvasGroup>().alpha = 1;
            this.rotateNode.SetActive(true);
            iTween.FadeTo(this.rotateNode, iTween.Hash("time", time, "alpha", ConstNodeOpacityMinVal));
        }
    }

    /// <summary>
    /// 关闭转圈显示
    /// </summary>
    private void _closeWait(float endtime)
    {
        float time = (float)(endtime * 0.5) - ConstDruationRestrain;
        if (this.maskNode != null && this.maskNode.activeSelf)
        {
            iTween.Stop(this.maskNode, "FadeTo");
            UnityUtils.DelayFuc(() =>
            {
                iTween.FadeTo(this.maskNode, iTween.Hash("time", time, "alpha", ConstNodeOpacityMinVal));
                UnityUtils.DelayFuc(() =>
                {
                    this.maskNode.SetActive(false);
                }, time);
            }, time);
        }
        if (this.rotateNode != null)
        {
            iTween.Stop(this.rotateNode, "FadeTo");
            UnityUtils.DelayFuc(() =>
           {
               iTween.FadeTo(this.rotateNode, iTween.Hash("time", time, "alpha", ConstNodeOpacityMinVal));
               UnityUtils.DelayFuc(() =>
               {
                   this.rotateNode.SetActive(false);
               }, time);
           }, time);
        }
        if (this.showLabel != null)
        {
            this.showLabel.text = "";
            this.showLabel.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 恢复转圈显示
    /// </summary>
    private void _recoverWait()
    {
        if (this.maskNode != null)
        {
            this.maskNode.SetActive(true);
            iTween.Stop(this.maskNode, "FadeTo");
            this.maskNode.GetComponent<CanvasGroup>().alpha = ConstNodeOpacityMaxVal;
        }

        if (this.rotateNode != null)
        {
            this.rotateNode.SetActive(true);
            iTween.Stop(this.rotateNode, "FadeTo");
            this.rotateNode.GetComponent<CanvasGroup>().alpha = ConstNodeOpacityFullVal;
        }
    }

    private void _adaptePopup()
    {
        if (this.adaptMask)
        {
            var visibleSize = UIAdapter.I.visibleSize;
            var maskNode = this.transform.Find("mask");
            if (maskNode != null)
            {
                maskNode.GetComponent<RectTransform>().sizeDelta = new Vector2(visibleSize.x * 2 , visibleSize.y * 2);
            }
        }
    }

    public bool getRotateShowStatus()
    {
        return this.rotateNode.activeSelf;
    }

}
