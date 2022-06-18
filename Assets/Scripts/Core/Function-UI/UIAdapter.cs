using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using independent;
using UnityEngine.UI;
using System;

public class UIAdapter
{

    private float _adaptScale = 1;
    public float adaptScale
    {
        get { return _adaptScale; }
    }


    private Vector2 _visibleSize = new Vector2(Screen.width, Screen.height);
    public Vector2 visibleSize
    {
        get { return _visibleSize; }
    }

    private float _bangsWidth = ConstBangsWidth.width;
    public float bangsWidth
    {
        get { return _bangsWidth; }
        set
        {
            if (value >= 0)
            {
                this._bangsWidth = value;
            }
            else
            {
                this._bangsWidth = ConstBangsWidth.width;
            }
        }
    }


    private static UIAdapter _Instance = null;

    public static UIAdapter I
    {
        get
        {
            if (_Instance == null) _Instance = new UIAdapter();
            return _Instance;
        }
    }

    /// <summary>
    /// 更新适配器参数
    /// /// </summary>
    public void updateAdapter(ref CanvasScaler canvas)
    {
        canvas.matchWidthOrHeight = 1.0f;

        var whRateFrame = Screen.width / Screen.height;
        var whRateDesign = canvas.referenceResolution.x / canvas.referenceResolution.y;
        var scale = 1.0f;

        if (whRateFrame > whRateDesign)
        {
            // 屏幕尺寸比设计尺寸宽
            scale = whRateFrame / whRateDesign;

            this._visibleSize.x = canvas.referenceResolution.x * scale;

        }
        else
        {
            // 屏幕尺寸比设计尺寸窄
            scale = whRateFrame / whRateDesign;

            this._visibleSize.x = canvas.referenceResolution.x * scale;

        }
        this._adaptScale = (float)Math.Round(scale, 2);
    }


    public void adaptUI(Transform node, AdaptionType type)
    {
        var scale = this._adaptScale;
        switch (type)
        {
            case AdaptionType.Center:
                var visibleSize = this._visibleSize;
                for (var i = 0; i < node.childCount; i++)
                {
                    var child = node.GetChild(i);
                    var bgNode = child.Find(PopuperConfig.ConstAdaptBg);
                    if (bgNode != null)
                    {
                        if (!bgNode.GetComponent<RectTransform>())
                        {
                            bgNode.gameObject.AddComponent<RectTransform>();
                        }
                        bgNode.GetComponent<RectTransform>().sizeDelta = visibleSize;
                    }
                    // 内容宽度适配
                    var contentNode = child.Find(PopuperConfig.ConstAdaptContent);
                    if (contentNode)
                    {
                        if (!contentNode.GetComponent<RectTransform>())
                        {
                            contentNode.gameObject.AddComponent<RectTransform>();
                        }
                        contentNode.GetComponent<RectTransform>().sizeDelta = visibleSize;
                        for (var j = 0; j < contentNode.childCount; j++)
                        {
                            var nd = contentNode.GetChild(j);
                            this._adaptNodeScale(nd, scale);
                        }
                    }
                    if (bgNode && contentNode)
                    {
                        this._adaptNodeScale(child, scale);
                    }
                }
                break;
            default:

                break;
        }
    }

    public void adapt2VisibleSize(RectTransform node)
    {
        node.sizeDelta = this._visibleSize;
    }

    /// <summary>
    /// 适配内容节点的子节点的位置
    /// </summary>
    public void adaptContentNodePos(Transform node, Vector3 position)
    {
        if (this._adaptScale >= 1) return; // 只有适配屏幕变小才会做位置适配
        node.position += position;
    }

    private void _adaptNodeScale(Transform node, float adaptScale)
    {
        if (adaptScale <= 1)
        {
            node.localScale *= adaptScale;
        }
    }

    /// <summary>
    /// 做横竖屏幕适配
    /// </summary>
    /// <param name="direction?"></param>
    public void adaptOrientation(ScreenOrientation direction = ScreenOrientation.LandscapeLeft)
    {

    }
}
