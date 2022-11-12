using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FragGameMainUI : PanelBase
{
    public Transform root;
    protected override void Awake()
    {
        if (root == null)
        {
            root = transform.Find("Root");
        }
        EventCenter.AddListener<bool>(Game_Event.FragActiveAllUI,OnActiveAllUI);
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        //GetControl<Button>("JumpButton").OnPointerDown = () =>
        //{
        //    EventCenter.PostEvent(Game_Event.FragGameJump);
        //}
        //GetControl<ScrollRect>("DirectButton").onClick.AddListener(() =>
        //{
        //    EventCenter.PostEvent(Game_Event.FragGameJump);
        //});
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnActiveAllUI(bool active)
    {
        root.gameObject.SetActive(active);
    }
}
