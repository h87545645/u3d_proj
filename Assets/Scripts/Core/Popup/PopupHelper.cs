using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using independent;


public class PopupHelper
{
    Vector3 ConstNodeScaleMinValV3 = new Vector3(0.5f, 0.5f, 0.5f);
    const float ConstNodeScaleMinVal = 0.5f;
    const float ConstNodeOpacityMinVal = 1.0f;
    const float ConstNodeOpacityMaxVal = 180f;

    const float ConstMaskFadeDuration = 0.175f;
    const float ConstActionOpenDuration = 0.35f;
    const float ConstActionCloseDuration = 0.25f;

    const float ConstDruationRestrain = 0.05f;



    private static PopupHelper _Instance = null;

    public static PopupHelper I
    {
        get
        {
            if (_Instance == null) _Instance = new PopupHelper();
            return _Instance;
        }
    }

    public void ready4Open(Popuper popup)
    {
        var popupMask = popup.transform.Find(PopuperConfig.stencil.popupMask);
        var popupNode = popup.transform.Find(PopuperConfig.stencil.popupNode);

        if (popupMask != null)
        {
            popupMask.gameObject.SetActive(false);
        }
        if (popupNode != null)
        {
            popupNode.gameObject.SetActive(false);
        }
    }

    public float popupOpen(Popuper popup)
    {
        var duration = 0.0f;
        switch (popup.popupType)
        {
            case PopupType.ANIMATION:
                {
                    // if (popup.openAnimName != '') {
                    // 	duration = this._popupAnimOpen(popup)
                    // } else {
                    // 	duration = this._popupActionOpen(popup)
                    // }
                    break;
                }
            case PopupType.POPUP:
                {
                    duration = this._popupActionOpen(popup);
                    break;
                }
            case PopupType.OPACITY:
                {
                    duration = this._popupOpacityOpen(popup);
                    break;
                }
        }
        if (popup.userInnerAudio)
        {
            // AudioPlayer.play('popup_all');
        }
        return duration + ConstDruationRestrain;
    }

    public float popupClose(Popuper popup)
    {
        var duration = 0.0f;
        switch (popup.popupType)
        {
            case PopupType.ANIMATION:
                {
                    // if (popup.openAnimName != '') {
                    // 	duration = this._popupAnimOpen(popup)
                    // } else {
                    // 	duration = this._popupActionOpen(popup)
                    // }
                    break;
                }
            case PopupType.POPUP:
                {
                    duration = this._popupActionClose(popup);
                    break;
                }
            case PopupType.OPACITY:
                {
                    duration = this._popupOpacityClose(popup);
                    break;
                }
        }
        if (popup.userInnerAudio)
        {
            // AudioPlayer.play('button_close');
        }
        return duration + ConstDruationRestrain;
    }

    private float _popupAnimOpen(Popuper popup)
    {
        return 0;
    }

    private float _popupAnimClose(Popuper popup)
    {
        return 0;
    }

    private float _popupActionOpen(Popuper popup)
    {
        var popupMask = popup.transform.Find(PopuperConfig.stencil.popupMask);
        var popupNode = popup.transform.Find(PopuperConfig.stencil.popupNode);
        if (popupNode != null)
        {
            iTween.Stop(popupNode.gameObject, "FadeTo");
            iTween.Stop(popupNode.gameObject, "ScaleTo");
            popupNode.gameObject.SetActive(true);
            popupNode.localScale = ConstNodeScaleMinValV3 * popup.nodeScale;
            _popActionOpenItween(popup, popupNode);
        }

        if (popupMask != null)
        {
            popupMask.gameObject.SetActive(true);

            iTween.Stop(popupMask.gameObject, "FadeTo");
            var r = popupMask.GetComponent<Renderer>().material.color.r;
            var g = popupMask.GetComponent<Renderer>().material.color.g;
            var b = popupMask.GetComponent<Renderer>().material.color.b;
            popupMask.GetComponent<Renderer>().material.color = new Color(r, g, b, ConstNodeOpacityMinVal);
            iTween.FadeTo(popupMask.gameObject, iTween.Hash("time", ConstMaskFadeDuration, "alpha", ConstNodeOpacityMaxVal));
        }


        return ConstActionOpenDuration;
    }

    private void _popActionOpenItween(Popuper popup, Transform popupNode)
    {
        var scaleDur1 = ConstActionOpenDuration * 0.7f;
        var scaleDur2 = ConstActionOpenDuration * 0.3f;
        float scale1 = popup.nodeScale * 1.05f;
        float scale2 = popup.nodeScale;
        iTween.FadeTo(popupNode.gameObject, iTween.Hash("time", ConstActionOpenDuration, "alpha", 255));
        iTween.ScaleTo(popupNode.gameObject, iTween.Hash("time", scaleDur1, "scale", new Vector3(scale1, scale1, scale1), "easeType", iTween.EaseType.easeOutSine));
        UnityUtils.DelayFuc(() =>
        {
            iTween.ScaleTo(popupNode.gameObject, iTween.Hash("time", scaleDur2, "scale", new Vector3(scale2, scale2, scale2), "easeType", iTween.EaseType.easeInSine));
        }, scaleDur1);

    }


    private float _popupActionClose(Popuper popup)
    {
        var popupMask = popup.transform.Find(PopuperConfig.stencil.popupMask);
        var popupNode = popup.transform.Find(PopuperConfig.stencil.popupNode);
        if (popupNode != null)
        {
            iTween.Stop(popupNode.gameObject, "FadeTo");
            iTween.Stop(popupNode.gameObject, "ScaleTo");
            iTween.FadeTo(popupNode.gameObject, iTween.Hash("time", ConstActionCloseDuration * 0.7f, "alpha", ConstNodeOpacityMinVal, "easeType", iTween.EaseType.easeInOutSine));
            iTween.ScaleTo(popupNode.gameObject, iTween.Hash("time", ConstActionCloseDuration * 0.7f, "scale", new Vector3(ConstNodeScaleMinVal * 1.4f, ConstNodeScaleMinVal * 1.4f, ConstNodeScaleMinVal * 1.4f), "easeType", iTween.EaseType.easeInSine));

        }

        if (popupMask != null)
        {
            iTween.Stop(popupMask.gameObject, "FadeTo");
            iTween.FadeTo(popupMask.gameObject, iTween.Hash("time", ConstMaskFadeDuration, "alpha", ConstNodeOpacityMinVal));

        }
        return ConstActionCloseDuration;
    }

    private float _popupOpacityOpen(Popuper popup)
    {
        var popupMask = popup.transform.Find(PopuperConfig.stencil.popupMask);
        var popupNode = popup.transform.Find(PopuperConfig.stencil.popupNode);
        if (popupNode != null)
        {
            var r = popupNode.GetComponent<Renderer>().material.color.r;
            var g = popupNode.GetComponent<Renderer>().material.color.g;
            var b = popupNode.GetComponent<Renderer>().material.color.b;
            popupNode.GetComponent<Renderer>().material.color = new Color(r, g, b, ConstNodeOpacityMinVal);

            popupNode.gameObject.SetActive(true);

            iTween.Stop(popupNode.gameObject, "FadeTo");

        }

        if (popupMask != null)
        {
            popupMask.gameObject.SetActive(true);

            iTween.Stop(popupMask.gameObject, "FadeTo");

        }

        iTween.FadeTo(popupNode.gameObject, iTween.Hash("time", ConstActionOpenDuration, "alpha", 255));
        if (popupMask != null)
        {
            var r = popupMask.GetComponent<Renderer>().material.color.r;
            var g = popupMask.GetComponent<Renderer>().material.color.g;
            var b = popupMask.GetComponent<Renderer>().material.color.b;
            popupMask.GetComponent<Renderer>().material.color = new Color(r, g, b, ConstNodeOpacityMinVal);
            iTween.FadeTo(popupMask.gameObject, iTween.Hash("time", ConstActionOpenDuration, "alpha", ConstNodeOpacityMaxVal));
        }
        return ConstActionOpenDuration;
    }

    private float _popupOpacityClose(Popuper popup)
    {
        var popupMask = popup.transform.Find(PopuperConfig.stencil.popupMask);
        var popupNode = popup.transform.Find(PopuperConfig.stencil.popupNode);
        if (popupNode != null)
        {
            iTween.Stop(popupNode.gameObject, "FadeTo");
            popupNode.gameObject.SetActive(true);
            iTween.FadeTo(popupNode.gameObject, iTween.Hash("time", ConstActionCloseDuration, "alpha", 0));

        }

        if (popupMask != null)
        {
            iTween.Stop(popupMask.gameObject, "FadeTo");
            popupMask.gameObject.SetActive(true);
            iTween.FadeTo(popupMask.gameObject, iTween.Hash("time", ConstMaskFadeDuration, "alpha", 0));

        }
        return ConstActionCloseDuration;
    }
}
