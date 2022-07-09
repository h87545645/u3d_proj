using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLogAlert : MonoBehaviour
{
    //��������
    private List<String> m_logEntries = new List<String>();
    //�Ƿ���ʾ���󴰿�
    private bool m_IsVisible = false;
    //������ʾ����
    private Rect m_WindowRect = new Rect(0, 0, Screen.width, Screen.height);
    //���ڹ�������
    private Vector2 m_scrollPositionText = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        //��������
        Application.logMessageReceived += (condition, stackTrace, type) =>
        {
            if (type == LogType.Exception || type == LogType.Error)
            {
                if (!m_IsVisible)
                {
                    m_IsVisible = true;
                }
                m_logEntries.Add(string.Format("{0}\n{1}", condition, stackTrace));
            }
        };

        ////�����쳣�Լ����� test
        //for (int i = 0; i < 10; i++)
        //{
        //    Debug.LogError("error alert");
        //}
        //int[] a = null;
        //a[1] = 100;
    }

    private void OnGUI()
    {
        if (m_IsVisible)
        {
            m_WindowRect = GUILayout.Window(0, m_WindowRect, ConsoleWindow, "Console");
        }
    }

    //��־����
    void ConsoleWindow(int windowID)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("clear", GUILayout.MaxWidth(200)))
        {
            m_logEntries.Clear();
        }
        if (GUILayout.Button("close", GUILayout.MaxWidth(200)))
        {
            m_IsVisible = false;
        }
        GUILayout.EndHorizontal();
        m_scrollPositionText = GUILayout.BeginScrollView(m_scrollPositionText);
        foreach (var entry in m_logEntries)
        {
            Color currentColor = GUI.contentColor;
            GUI.contentColor = Color.red;
            GUILayout.TextArea(entry);
            GUI.contentColor = currentColor;
        }
        GUILayout.EndScrollView();
    }
}
