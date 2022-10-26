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
    /// ���������ӽڵ�ؼ�
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
    /// �õ���Ӧ���ֵĶ�Ӧ�ؼ��ű�
    /// </summary>
    /// <param name="sControlName">�ؼ�����</param>
    /// <typeparam name="T">����, ָUI�ؼ�</typeparam>
    /// <returns>���ض�Ӧ��ui�ؼ�, �������򷵻�nullֵ</returns>
    protected T GetControl<T>(string sControlName) where T : UIBehaviour
    {
        if (dict_allUI.ContainsKey(sControlName))
        {
            for (int i = 0; i < dict_allUI[sControlName].Count; i++)
            {
                //��Ӧ�ֵ��ֵ���Ǹ����ϣ��У�����Ҫ������͵�
                //�򷵻س�ȥ�������ⲿ��
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

    #region ��������д�����ǣ��˷�������ʵ��UI�����������
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
