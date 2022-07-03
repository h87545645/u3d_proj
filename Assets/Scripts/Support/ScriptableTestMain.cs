using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableTestMain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ScriptableObjectTest script = Resources.Load<ScriptableObjectTest>("New Scriptable Object Test");
        Debug.LogFormat("name : {0} id :  {1}", script.m_PlayerInfo[0].name, script.m_PlayerInfo[0].id);
    }

}
