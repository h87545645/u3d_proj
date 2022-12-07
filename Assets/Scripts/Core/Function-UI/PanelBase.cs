using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEditor;

public class PanelBase : MonoBehaviour
{
    [SerializeField, SetProperty("IsBlock")]
    [Tooltip("is need generate block node")]
    private bool isBlock = false;
    public bool IsBlock
    {
        get => isBlock;
        set
        {
            isBlock = value;
            EditorGenBlock();
        }
    }
    
    
    private Dictionary<string, List<UIBehaviour>> dict_allUI = new Dictionary<string, List<UIBehaviour>>();

    protected virtual void Awake()
    {
        
        FindAllControl();
    }
    
    void Reset()
    {
#if !UNITY_EDITOR
        return;
#endif
        // if (EditorApplication.isPlaying)
        // {
        //     return;
        // }
        GameObject content = transform.Find("content").gameObject;
        if (content == null)
        {
            content = new GameObject("content"); 
            content.transform.SetParent(transform);
            content.transform.position = new Vector3(0, 0, 0);
        }
    }

    /// <summary>
    /// ??????????????
    /// </summary>
    protected virtual void FindAllControl()
    {
        FindChildControl<Button>();
        FindChildControl<Image>();
        FindChildControl<Text>();
        FindChildControl<Toggle>();
        FindChildControl<ScrollRect>();
        FindChildControl<Slider>();
    }

    /// <summary>
    /// ???????????????????
    /// </summary>
    /// <param name="sControlName">???????</param>
    /// <typeparam name="T">????, ?UI???</typeparam>
    /// <returns>????????ui???, ?????????null?</returns>
    protected T GetControl<T>(string sControlName) where T : UIBehaviour
    {
        if (dict_allUI.ContainsKey(sControlName))
        {
            for (int i = 0; i < dict_allUI[sControlName].Count; i++)
            {
                //???????????????????��??????????????
                //???????????????
                if (dict_allUI[sControlName][i] is T)
                {
                    return dict_allUI[sControlName][i] as T;
                }
            }
        }
        return null;
    }


    /// <summary>
    /// ?????????UI???????????????
    /// </summary>
    /// <typeparam name="T">????, ?UI???</typeparam>
    private void FindChildControl<T>() where T : UIBehaviour
    {
        T[] arr_control = this.GetComponentsInChildren<T>();
        string sObjName;
        for (int i = 0; i < arr_control.Length; i++)
        {
            sObjName = arr_control[i].gameObject.name;
            if (dict_allUI.ContainsKey(sObjName))
            {
                dict_allUI[sObjName].Add(arr_control[i]);
            }
            else
            {
                dict_allUI.Add(sObjName, new List<UIBehaviour>() { arr_control[i] });
            }
        }
    }
    
    private void EditorGenBlock()
    {
#if UNITY_EDITOR
        // if (EditorApplication.isPlaying)
        // {
        //     return;
        // }
        GameObject blockNode = transform.Find("PopupMask(Clone)") == null ? null : transform.Find("PopupMask(Clone)").gameObject;
        if (isBlock)
        {
           
            if (blockNode == null)
            {
                // blockNode = PrefabLoadMgr.I.LoadSync("popupmask", transform);
                GameObject go = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/GameAssets/Prefab/UI/PopupMask.prefab",typeof(GameObject));
                blockNode = GameObject.Instantiate(go);
                blockNode.transform.SetParent(transform);
                RectTransform uiRect = blockNode.GetComponent<RectTransform>();
                uiRect.localScale = Vector3.one;
                uiRect.localPosition = Vector3.zero;
                uiRect.sizeDelta = transform.GetComponent<RectTransform>().sizeDelta;
                blockNode.transform.SetAsFirstSibling();
            }
            
        }
        else
        {
            if (blockNode == null)
            {
                return;
            }
            MonoBehaviour.DestroyImmediate(blockNode);
            blockNode = null;
        }
        
#endif
    
    }

    #region 子类继承
    /// <summary>
    /// ???ui????
    /// </summary>
    public virtual void ShowUI()
    {
        GameObject content = transform.Find("content") == null ?  gameObject : transform.Find("content").gameObject;
        content.transform.localScale = new Vector3(0,0,0);
        content.transform.DOScale(1, 0.5f);
    }

    /// <summary>
    /// ????UI
    /// </summary>
    public virtual void HideUI()
    {
        GameObject content = transform.Find("content") == null ?  gameObject : transform.Find("content").gameObject;
        content.transform.DOScale(0, 0.5f);
    }
    #endregion
    
    

}
