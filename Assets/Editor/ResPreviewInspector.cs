using System.IO;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(ResPreview))]
public class ResPreviewInspector : Editor
{
    [MenuItem("GameObject/ResPreview", false, 12)]
    static void LoadResPreview()
    {
        ResPreview resPreview = new GameObject("ResPreview").AddComponent<ResPreview>();
        resPreview.transform.SetParent(Selection.activeTransform, false);
        Selection.activeTransform = resPreview.transform;
    }

    private void OnEnable()
    {
        if (target != null)
        {
            if (!Application.isPlaying)
            {
                ClearHierarchy();
                (target as ResPreview).Load();
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GUILayout.Label("请把Resouce目录下的prefab拖入");
        string loadPath = serializedObject.FindProperty("m_LoadPath").stringValue;
        EditorGUI.BeginChangeCheck();
        GameObject prefab = Resources.Load<GameObject>(loadPath);
        GameObject newPrefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject)) as GameObject;
        if (EditorGUI.EndChangeCheck())
        {
            string resPath;
            bool isResFolder = IsResourcesFolder(newPrefab, out resPath);
            if (!isResFolder)
            {
                EditorUtility.DisplayDialog("提示", "必须拖拽Resources下的prefab", "ok");
                ClearHierarchy();
            }
            serializedObject.FindProperty("m_LoadPath").stringValue = resPath;
            if (isResFolder)
            {
                serializedObject.ApplyModifiedProperties();
                if (!Application.isPlaying)
                {
                    (target as ResPreview).Load();
                }
            }
        }

        serializedObject.FindProperty("m_IsInitLoad").boolValue = EditorGUILayout.Toggle("是否 Awake 加载", serializedObject.FindProperty("m_IsInitLoad").boolValue);
        GUILayout.Space(18f);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Refresh", GUILayout.Width(80)))
        {
            (target as ResPreview).Load();
        }
        if (GUILayout.Button("Clear", GUILayout.Width(80)))
        {
            ClearHierarchy();
        }
        GUILayout.EndHorizontal();
        serializedObject.ApplyModifiedProperties();
    }

    protected bool IsResourcesFolder(Object o, out string resPath)
    {
        if (o)
        {
            string path = AssetDatabase.GetAssetPath(o);
            bool beFirst = true;
            string tmp = string.Empty;
            DirectoryInfo dir = new DirectoryInfo(path);
            while (dir != null)
            {
                if (dir.Name == "Resources")
                {
                    resPath = tmp;
                    return true;
                }
                tmp = tmp.Insert(0, beFirst ? Path.GetFileNameWithoutExtension(dir.Name) : dir.Name + "/");
                if (beFirst)
                {
                    beFirst = false;
                }
                dir = dir.Parent;
            }
        }
        resPath = string.Empty;
        return false;
    }

    private void ClearHierarchy()
    {
        Transform transform = (target as ResPreview).transform;
        if (transform != null)
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
    }
}
