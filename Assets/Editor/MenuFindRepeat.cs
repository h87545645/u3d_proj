using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Security.Cryptography;
using System;

public class MenuFindRepeat 
{
    [MenuItem("Tools/Report/�����ظ���ͼ")]
    static void ReportTexture()
    {
        Dictionary<string, string> md5dic = new Dictionary<string, string>();
        string[] paths = AssetDatabase.FindAssets("t:prefab", new string[] {"Assets/Resources" });
        foreach (var prefabGuid in paths)
        {
            string prefabAssetPath = AssetDatabase.GUIDToAssetPath(prefabGuid);
            string[] depend = AssetDatabase.GetDependencies(prefabAssetPath, true);
            for (int i = 0; i < depend.Length; i++)
            {
                string assetPath = depend[i];
                AssetImporter importer = AssetImporter.GetAtPath(assetPath);
                //������ͼ��ģ����Դ
                if (importer is TextureImporter || importer is ModelImporter)
                {
                    string md5 = GetMD5Hash(Path.Combine(Directory.GetCurrentDirectory(), assetPath));
                    string path;
                    if (!md5dic.TryGetValue(md5,out path))
                    {
                        md5dic[md5] = assetPath;
                    }
                    else
                    {
                        if (path != assetPath)
                        {
                            Debug.LogFormat("{0}{1} ��Դ�����ظ��� ", path, assetPath);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// ��ȡ�ļ�MD5
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    static string GetMD5Hash(string filePath)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        return BitConverter.ToString(md5.ComputeHash(File.ReadAllBytes(filePath))).Replace("-", "").ToLower();
    }
}
