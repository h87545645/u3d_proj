using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LightmapCopy : MonoBehaviour
{
    [MenuItem("Tools/DuplicateGameObject %#d")]
    static void DuplicateGameObject()
    {
        if (Selection.activeTransform)
        {
            Dictionary<string, Renderer> save = new Dictionary<string, Renderer>();
            //根据相对路径保存renderer
            foreach (var renderer in Selection.activeTransform.GetComponentsInChildren<Renderer>())
            {
                string path = AnimationUtility.CalculateTransformPath(renderer.transform, Selection.activeTransform);
                save[path] = renderer;
            }
            //执行赋值
            EditorApplication.ExecuteMenuItem("Edit/Duplicate");
            //还原烘焙信息
            foreach (var renderer in Selection.activeTransform.GetComponentsInChildren<Renderer>())
            {
                string path = AnimationUtility.CalculateTransformPath(renderer.transform, Selection.activeTransform);
                if (save.ContainsKey(path))
                {
                    renderer.lightmapIndex = save[path].lightmapIndex;
                    renderer.lightmapScaleOffset = save[path].lightmapScaleOffset;
                }
            }
        }
    }
}
