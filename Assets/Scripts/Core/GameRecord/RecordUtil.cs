using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;

public class RecordUtil 
{
    //游戏存档保存的根目录
    static string RecordRootPath
    {
        get
        {
#if (UNITY_EDITOR || UNITY_STANDALONE)
        return Application.dataPath + "/../Record/";
#else
    `   return Application.persistentDataPath + "/Record/";
#endif
        }
    }

    //游戏存档
    static Dictionary<string, string> recordDic = new Dictionary<string, string>();
    //标记某个游戏存档是否需要重新写入
    static List<string> recordDirty = new List<string>();
    //标记某个游戏存档是否要删除
    static List<string> deletaDirty = new List<string>();
    //表示某个游戏存档读取时需要重新从文件读取
    static List<string> readDirty = new List<string>();

    static private readonly UTF8Encoding UTF8 = new UTF8Encoding(false);

    static RecordUtil()
    {
        readDirty.Clear();
        if (Directory.Exists(RecordRootPath))
        {
            foreach (string file in Directory.GetFiles(RecordRootPath,"*.record",SearchOption.TopDirectoryOnly))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                if (!readDirty.Contains(name))
                {
                    readDirty.Add(name);
                }
            }
        }
    }

    //强制写入文件
    public static void Save()
    {
        foreach (string key in deletaDirty)
        {
            try
            {
                string path = Path.Combine(RecordRootPath, key + ".record");
                if (recordDirty.Contains(key))
                {
                    recordDirty.Remove(key);
                }
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
        deletaDirty.Clear();

        foreach (string key in recordDirty)
        {
            string value;
            if (recordDic.TryGetValue(key,out value))
            {
                if (!readDirty.Contains(key))
                {
                    readDirty.Add(key);
                }
                string path = Path.Combine(RecordRootPath, key + ".record");
                recordDic[key] = value;
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    File.WriteAllText(path, value, UTF8);
                }
                catch (Exception ex)
                {

                    Debug.LogError(ex.Message);
                }
            }
        }
    }

    public static void Set(string key, string value)
    {
        recordDic[key] = value;
        if (!recordDirty.Contains(key))
        {
            recordDirty.Add(key);
        }

#if (UNITY_EDITOR || UNITY_STANDALONE)
        Save();
#endif
    }

    public static string Get(string key)
    {
        return Get(key, string.Empty);
    }

    public static string Get(string key,string defaultValue)
    {
        if (readDirty.Contains(key))
        {
            string path = Path.Combine(RecordRootPath, key + ".record");
            try
            {
                string readStr = File.ReadAllText(path, UTF8);
                recordDic[key] = readStr;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
        string value;
        if (recordDic.TryGetValue(key,out value))
        {
            return value;
        }
        else
        {
            return defaultValue;
        }
    }

    public static void Delete(string key)
    {
        if (recordDic.ContainsKey(key))
        {
            recordDic.Remove(key);
        }
        if (!deletaDirty.Contains(key))
        {
            deletaDirty.Add(key);
        }

#if (UNITY_EDITOR || UNITY_STANDALONE)
        Save();
#endif

    }
}
