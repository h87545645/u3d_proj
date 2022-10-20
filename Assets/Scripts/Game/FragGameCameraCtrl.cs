using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragGameCameraCtrl : MonoBehaviour
{

    public double screenHeight = 13;
    
    private void OnBecameVisible()
    {
       
        // print("�������Ұ��");
    }
 
    private void OnBecameInvisible()
    {
        // print("���������Ұ��");
        int index = (int)Math.Floor((transform.position.y + screenHeight*0.5) / screenHeight);
        index = Math.Max(index, 0);
        Camera.main.transform.position = new Vector3(0,(float)(index * screenHeight),-10);
    }
}
