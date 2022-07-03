using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScriptableObjectTest : ScriptableObject
{

    [SerializeField]
    public List<PlayerInfo> m_PlayerInfo;

    [System.Serializable]
    public class PlayerInfo
    {
        public int id;
        public string name;
    }
}
