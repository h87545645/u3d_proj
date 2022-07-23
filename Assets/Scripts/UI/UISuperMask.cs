using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Image))]
[AddComponentMenu("UI/UISuperMask")]

public class UISuperMask : Mask
{
    private float m_LastminX = -1f, m_LastminY = -1f, m_LastmaxX = -1f, m_LastmaxY = -1f;
    private float m_MinX = 0f, m_MinY = 0f, m_MaxX = 0f, m_MaxY = 0f;
    private Vector3[] m_Corners = new Vector3[4];
    private Image m_Image;

    void GetWorldCorners()
    {
        //避免每次都计算
        if (!Mathf.Approximately(m_LastminX,m_MinX )||
            !Mathf.Approximately(m_LastminY, m_MinY )||
            !Mathf.Approximately(m_LastmaxX, m_MaxX) ||
            !Mathf.Approximately(m_LastmaxX, m_MaxY))
        {
            RectTransform rectTransform = transform as RectTransform;
            rectTransform.GetWorldCorners(m_Corners);

            m_LastminX = m_MinX;
            m_LastminY = m_MinY;
            m_LastmaxX = m_MaxX;
            m_LastmaxY = m_MaxY;

            m_MinX = m_Corners[0].x;
            m_MinY = m_Corners[0].y;
            m_MaxX = m_Corners[2].x;
            m_MaxY = m_Corners[2].y;
        }

    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        Refresh();
    }

    public void Refresh()
    {
        GetWorldCorners();
        if (Application.isPlaying)
        {
            foreach (ParticleSystemRenderer system in transform.GetComponentsInChildren<ParticleSystemRenderer>(true))
            {
                SetRenderer(system);
            }
        }
    }

    void SetRenderer(Renderer renderer)
    {
        if (renderer.sharedMaterial)
        {
            Shader shader = Resources.Load<Shader>("SuperMask/Alpha Blended Premultiply");
            renderer.material.shader = shader;
            Material m = renderer.material;
            m.SetFloat("_MinX", m_MinX);
            m.SetFloat("_MinY", m_MinY);
            m.SetFloat("_MaxX", m_MaxX);
            m.SetFloat("_MaxY", m_MaxY);
        }
    }
}


//--------add----------
//_MinX("Min X", Float) = -10
//_MaxX("Max X", Float) = 10
//_MinY("Min Y", Float) = -10
//_MaxY("Max Y", Float) = 10
//--------add----------