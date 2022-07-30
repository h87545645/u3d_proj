using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public SpriteRenderer heroRenderer;
    public Rigidbody2D heroRigidbody2D;

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Run(Vector2.up);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Run(Vector2.down);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Run(Vector2.left,true);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Run(Vector2.right,false);
        }
    }

    void Run(Vector2 position,bool flipx = false)
    {
        heroRenderer.flipX = flipx;
        heroRigidbody2D.position += (position * 0.1f);
    }
}
