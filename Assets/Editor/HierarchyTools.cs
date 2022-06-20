using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HierarchyTools
{
    [MenuItem("GameObject/My Create/Cube",false,0)]
    static void CreateCube()
    {
        GameObject.CreatePrimitive(PrimitiveType.Cube);
    }

    [InitializeOnLoadMethod]
    static void InitializeOnLoadMethod()
    {
        EditorApplication.hierarchyWindowItemOnGUI = delegate (int instanceID, Rect selectionRect)
        {
            if (Selection.activeObject && instanceID == Selection.activeObject.GetInstanceID())
            {
                //设置拓展按钮区域
                float width = 50f;
                float height = 20f;
                selectionRect.x += (selectionRect.width - width);
                selectionRect.width = width;
                selectionRect.height = height;
                if (GUI.Button(selectionRect, AssetDatabase.LoadAssetAtPath<Texture>("Assets/unity.png")))
                {
                    Debug.LogFormat("click : {0}", Selection.activeObject.name);
                }
            }
        };
    }
}
