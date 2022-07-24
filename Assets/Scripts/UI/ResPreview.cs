using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResPreview : MonoBehaviour
{
    [SerializeField]
    private string m_LoadPath = string.Empty;
    [SerializeField]
    private bool m_IsInitLoad = true;

    public bool IsLoad { get; private set; }

    private void Awake()
    {
        IsLoad = false;
        if (m_IsInitLoad)
        {
            Load();
        }
    }

    public void Load()
    {
        GameObject prefab = Resources.Load<GameObject>(m_LoadPath);
        if (prefab)
        {
            GameObject go = Instantiate<GameObject>(prefab);
            go.transform.SetParent(transform, false);
            go.name = prefab.name;
#if UNITY_EDITOR
            foreach (Transform t in go.GetComponentsInChildren<Transform>())
            {
                t.gameObject.hideFlags = HideFlags.NotEditable | HideFlags.DontSave;
            }
#endif
        }
        IsLoad = true;
    }
}
