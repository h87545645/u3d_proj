using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class RecordTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Setting setting = new Setting();
        setting.stringValue = "测试";
        setting.intValue = 10000;

        RecordUtil.Set("setting", JsonUtility.ToJson(setting));
//#if UNITY_EDITOR 
//        RecordUtil.Save();
//#endif

    }

    private Setting m_Setting = null;
    private void OnGUI()
    {
        if (GUI.Button(new Rect(300, 100, 200, 100), "<size=25>获取存档</size>"))
        {
            m_Setting = JsonUtility.FromJson<Setting>(RecordUtil.Get("setting"));
        }
        if (m_Setting != null)
        {
            GUILayout.Label(string.Format("<size=50> {0},{1}</size>", m_Setting.intValue, m_Setting.stringValue));
        }
    }

    //游戏切后台自动保存
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            RecordUtil.Save();
        }
    }

    [System.Serializable]
    class Setting
    {
        public string stringValue;
        public int intValue;
    }
}
