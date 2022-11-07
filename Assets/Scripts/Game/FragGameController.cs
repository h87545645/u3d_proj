using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FragGameController : MonoBehaviour
{
    public FragGameCameraCtrl fragGameCameraCtrl;

    public FragHero fragHero;

    public Pelican pelican;

    public Light2D globalLight;

    private void Awake()
    {
        EventCenter.AddListener<int>(Game_Event.FragGameCameraMove,OnCameraMove);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCameraMove(int index)
    {
        switch (index)
        {
            case 6:
            {
                globalLight.intensity = 0.2f;
                break;
            }
        }
    }
}
