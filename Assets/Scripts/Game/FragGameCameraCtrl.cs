using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragGameCameraCtrl : MonoBehaviour
{

    public double screenHeight = 14;

    [HideInInspector] public int currentCameraIndex = 0;

    private void Start()
    {
        this.OnBecameInvisible();
    }

    private void OnBecameVisible()
    {
       
        // print("�������Ұ��");
    }
 
    private void OnBecameInvisible()
    {
        // print("���������Ұ��");
        currentCameraIndex = (int)Math.Floor((transform.position.y + screenHeight*0.5) / screenHeight);
        currentCameraIndex = Math.Max(currentCameraIndex, 0);
        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(0,(float)(currentCameraIndex * screenHeight),-10);
        }
    }
}
