using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;

public class PelicanAnimController : MonoBehaviour
{
    public Animator animtor;
    // Start is called before the first frame update
    void Start()
    {
        if (animtor == null)
        {
            animtor = GetComponent<Animator>();
        }
        InvokeRepeating("RandomIdelAnim", 7, 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Speak()
    {
        animtor.SetBool("speak",true);
    }

    public void FinishSpeak()
    {
        animtor.SetBool("speak",false);
        animtor.SetBool("idle",true);
    }

    public void Fly()
    {
        animtor.SetBool("fly",true);
    }

    public void FlyEnd()
    {
        animtor.SetBool("fly",false);
        animtor.SetBool("idle",true);
    }

    public void RandomIdelAnim()
    {
        Random rd = new Random();
        string idleAnim = rd.Next(10) % 10 < 5 ? "idle1" : "idle2";
        animtor.SetTrigger(idleAnim);
    }
}
