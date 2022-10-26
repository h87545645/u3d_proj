using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PanelBase : MonoBehaviour
{
    private Dictionary<string, List<UIBehaviour>> dict_allUI = new Dictionary<string, List<UIBehaviour>>();

    protected void Awake()
    {
        FindAllControl();
    }

    /// <summary>
    /// 查找所有子节点控件
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
    /// 得到对应名字的对应控件脚本
    /// </summary>
    /// <param name="sControlName">控件名字</param>
    /// <typeparam name="T">泛型, 指UI控件</typeparam>
    /// <returns>返回对应的ui控件, 不存在则返回null值</returns>
    protected T GetControl<T>(string sControlName) where T : UIBehaviour
    {
        if (dict_allUI.ContainsKey(sControlName))
        {
            for (int i = 0; i < dict_allUI[sControlName].Count; i++)
            {
                //对应字典的值（是个集合）中，符合要求的类型的
                //则返回出去，这样外部就
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

    #region 让子类重写（覆盖）此方法，来实现UI的隐藏与出现
    /// <summary>
    /// ???ui????
    /// </summary>
    public virtual void ShowUI()
    {
        transform.localScale = new Vector3(0,0,0);
        transform.DOScale(1, 0.5f);
    }

    /// <summary>
    /// ????UI
    /// </summary>
    public virtual void HideUI()
    {
        transform.DOScale(0, 0.5f);
    }
    #endregion

}
