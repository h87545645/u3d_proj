using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PrefabLightmap : MonoBehaviour
{
    public int lightmapIndex;
    public Vector4 lightmapScaleOffset;

    private void Awake()
    {
        //prefab 实例化后赋值
        Renderer renderer = GetComponent<Renderer>();
        if (renderer)
        {
            renderer.lightmapIndex = lightmapIndex;
            renderer.lightmapScaleOffset = lightmapScaleOffset;
        }
    }

#if UNITY_EDITOR
    [MenuItem("GameObject/Light/ToPrefab")]
    static void ToPrefab()
    {
        //确保选择Hierarchy视图下的一个游戏对象
        if (Selection.activeTransform)
        {
            Renderer renderer = Selection.activeTransform.GetComponent<Renderer>();
            //确保有renderer组件
            if (renderer)
            {
                PrefabLightmap prefabLightmap = Selection.activeTransform.GetComponent<PrefabLightmap>();
                if (!prefabLightmap)
                {
                    prefabLightmap = Selection.activeTransform.gameObject.AddComponent<PrefabLightmap>();
                }
                prefabLightmap.lightmapIndex = renderer.lightmapIndex;
                prefabLightmap.lightmapScaleOffset = renderer.lightmapScaleOffset;

                Object prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource<Object>(renderer.gameObject);
                //如果有prefab文件则更新，否者创建新的
                if (prefab)
                {
                    PrefabUtility.SaveAsPrefabAsset(Selection.activeTransform.gameObject, AssetDatabase.GetAssetPath(prefab));
                }
                else
                {
                    string localPath = "Assets/GameAssets/Prefabs/" + Selection.activeTransform.name + ".prefab";
                    localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
                    PrefabUtility.SaveAsPrefabAsset( Selection.activeTransform.gameObject, localPath);
                }
            }
        }
    }
#endif
}
