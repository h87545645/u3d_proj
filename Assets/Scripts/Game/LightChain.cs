using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightChain : MonoBehaviour
{
    public Rigidbody2D rigidbody2D;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if ( Math.Abs(rigidbody2D.velocity.y) <= 0.05 && Math.Abs(rigidbody2D.velocity.x) >1 && Math.Abs(rigidbody2D.velocity.x) <= 4)
        {
            float flag = rigidbody2D.velocity.x < 0 ? -1 : 1;
            rigidbody2D.velocity = new Vector2(flag*4 , rigidbody2D.velocity.y);
            // Debug.Log(rigidbody2D.velocity);
        }
    }
}
