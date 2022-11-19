using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragGameRecord : SingletonBase<FragGameRecord>
{
    public PlayerRecord reocrd;

    public FragGameRecord()
    {
        string recordStr = RecordUtil.Get("FragGameRecord");
        if (recordStr != "")
        {
            PlayerRecord record = JsonUtility.FromJson<PlayerRecord>(RecordUtil.Get("FragGameRecord"));
            reocrd = record;
        }
        else
        {
            reocrd = new PlayerRecord();
        }
    }

    public void SetRecordItem()
    {
        RecordUtil.Set("FragGameRecord",JsonUtility.ToJson(reocrd));
    }
    
    
    [System.Serializable]
    public class PlayerRecord
    {
        public Vector2 playerPosition = new Vector2(0,0);
        public float playerTotalTime = 0;
        public bool playerAlreadyGuide = false;
        public float heightRecord = 0;
        public int jumpCnt = 0;
        public bool isCompleted = false;
    }
}
