using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool
{
    private GameObject m_ObjectContainer;
    private readonly Dictionary<string, Stack<GameObject>> m_AssetObjects;

    public GameObjectPool(string name)
    {
        m_AssetObjects = new Dictionary<string, Stack<GameObject>>();
        m_ObjectContainer = new GameObject("#POOL#" + name);
        m_ObjectContainer.SetActive(false);
    }

    //获取游戏对象
    public GameObject GetObject(string assetPath)
    {
        GameObject go = GetObjectFromPool(assetPath);
        if (go == null)
        {
            go = Object.Instantiate<GameObject>(Resources.Load<GameObject>(assetPath));
            go.AddComponent<PoolObject>().value = assetPath;
        }
        return go;
    }

    //释放游戏对象
    public void ReleaseObject(GameObject go)
    {
        var comp = go.GetComponent<PoolObject>();
        if (comp != null && comp.value != null)
        {
            Stack<GameObject> objects;
            if (m_AssetObjects.TryGetValue(comp.value,out objects))
            {
                objects.Push(go);
                go.transform.SetParent(m_ObjectContainer.transform, false);
                return;
            }
        }
        Object.Destroy(go);
    }

    private GameObject GetObjectFromPool(string assetPath)
    {
        Stack<GameObject> objects;
        if (!m_AssetObjects.TryGetValue(assetPath,out objects))
        {
            objects = new Stack<GameObject>();
            m_AssetObjects[assetPath] = objects;
        }
        return objects.Count > 0 ? objects.Pop() : null;
    }

    class PoolObject : MonoBehaviour
    {
        public string value;
    }
}
