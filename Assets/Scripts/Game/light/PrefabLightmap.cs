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
        //prefab ʵ������ֵ
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
        //ȷ��ѡ��Hierarchy��ͼ�µ�һ����Ϸ����
        if (Selection.activeTransform)
        {
            Renderer renderer = Selection.activeTransform.GetComponent<Renderer>();
            //ȷ����renderer���
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
                //�����prefab�ļ�����£����ߴ����µ�
                if (prefab)
                {
                    PrefabUtility.SaveAsPrefabAsset(Selection.activeTransform.gameObject, AssetDatabase.GetAssetPath(prefab));
                }
                else
                {
                    string localPath = "Assets/GameAssets/Prefab/" + Selection.activeTransform.name + ".prefab";
                    localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
                    PrefabUtility.SaveAsPrefabAsset( Selection.activeTransform.gameObject, localPath);
                    //PrefabUtility.ReplacePrefab(Selection.activeGameObject,prefab);
                }
            }
        }
    }
#endif
}
