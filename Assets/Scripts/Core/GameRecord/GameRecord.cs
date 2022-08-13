using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRecord : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Record record = new Record();
        record.stringValue = "tsp";
        record.intValue = 28;
        record.names = new List<string>() { "kami", "yagami" };
        string json = JsonUtility.ToJson(record);
        try
        {
            PlayerPrefs.SetString("record", json);
        }
        catch (System.Exception err)
        {

            Debug.Log("got : " + err);
        }

        //∂¡»°¥Êµµ
        record = JsonUtility.FromJson<Record>(PlayerPrefs.GetString("record"));
        Debug.LogFormat("stringValue = {0} intValue = {1}",record.stringValue,record.intValue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //¥Êµµ∂‘œÛ
    [System.Serializable]
    public class Record
    {
        public string stringValue;
        public int intValue;
        public List<string> names;
    }
}
