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
            //�������·������renderer
            foreach (var renderer in Selection.activeTransform.GetComponentsInChildren<Renderer>())
            {
                string path = AnimationUtility.CalculateTransformPath(renderer.transform, Selection.activeTransform);
                save[path] = renderer;
            }
            //ִ�и�ֵ
            EditorApplication.ExecuteMenuItem("Edit/Duplicate");
            //��ԭ�決��Ϣ
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
