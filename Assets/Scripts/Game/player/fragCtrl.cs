using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fragCtrl : MonoBehaviour
{
    public SpriteRenderer heroRenderer;
    public Rigidbody2D heroRigidbody2D;
    public Animator fragAnim;

    private void Start()
    {
        //CollisionListerner.onCollisionEnter2D.AddListener(delegate (GameObject g1, GameObject g2) {
        //    Debug.LogFormat("{0}¿ªÊ¼Åö×²{1}", g1.name, g2.name);
        //});
        //CollisionListerner.onCollisionStay2D.AddListener(delegate (GameObject g1, GameObject g2) {
        //    Debug.LogFormat("{0}Åö×²ÖÐ{1}", g1.name, g2.name);
        //});
        //CollisionListerner.onCollisionExit2D.AddListener(delegate (GameObject g1, GameObject g2) {
        //    Debug.LogFormat("{0}½áÊøÅö×²{1}", g1.name, g2.name);
        //});
    }


    private void LateUpdate()
    {
        //if (Input.GetKey(KeyCode.W))
        //{
        //    Run(Vector2.up);
        //}
        //else if (Input.GetKey(KeyCode.S))
        //{
        //    Run(Vector2.down);
        //}
        //else if (Input.GetKey(KeyCode.A))
        //{
        //    Run(Vector2.left, true);
        //}
        //else if (Input.GetKey(KeyCode.D))
        //{
        //    Run(Vector2.right, false);
        //}
        if (Input.GetKey(KeyCode.Space))
        {
            Jump(Vector2.up);
        }
    }

    void Jump(Vector2 position)
    {
        fragAnim.SetBool("jump", true);
        heroRigidbody2D.AddForce(position);
    }

    void Run(Vector2 position, bool flipx = false)
    {
        heroRenderer.flipX = flipx;
        heroRigidbody2D.position += (position * 0.1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            Debug.DrawLine(contact.point, transform.position, Color.red);
            var direction = transform.InverseTransformPoint(contact.point);
            if (direction.x > 0f)
            {
                print("right collision");
            }
            if (direction.x < 0f)
            {
                print("left collision");
            }
            if (direction.y > 0f)
            {
                print("up collision");
            }
            if (direction.y < 0f)
            {
                print("down collision");
            }
        }
    }
}
