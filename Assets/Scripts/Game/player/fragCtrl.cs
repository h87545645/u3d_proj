using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fragCtrl : MonoBehaviour
{
    public SpriteRenderer heroRenderer;
    public Rigidbody2D heroRigidbody2D;
    public Animator fragAnim;


    private double powerTime = 0.0f;
    private bool isGround = false;


    private void Start()
    {
        //CollisionListerner.onCollisionEnter2D.AddListener(delegate (GameObject g1, GameObject g2)
        //{
        //    if (g2.tag == "platform")
        //    {

        //    }
        //    Debug.LogFormat("{0}¿ªÊ¼Åö×²{1}", g1.name, g2.name);
        //});
        //CollisionListerner.onCollisionStay2D.AddListener(delegate (GameObject g1, GameObject g2)
        //{
        //    Debug.LogFormat("{0}Åö×²ÖÐ{1}", g1.name, g2.name);
        //    if (g2.tag == "platform")
        //    {

        //    }
        //});
        //CollisionListerner.onCollisionExit2D.AddListener(delegate (GameObject g1, GameObject g2)
        //{
        //    Debug.LogFormat("{0}½áÊøÅö×²{1}", g1.name, g2.name);
        //    if (g2.tag == "platform")
        //    {

        //    }
        //});
    }

    void FixedUpdate()
    {
         Debug.DrawRay(transform.position, Vector2.down * 0.11f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(heroRigidbody2D.transform.position, Vector2.down, 0.15f, 1 << 3);
        if (hit.collider != null)
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }
        Debug.Log("isGround " + this.isGround);
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
        if (fragAnim.GetBool("power"))
        {
            this.powerTime += Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Power();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            if (this.powerTime > 0.1)
            {
                Debug.Log("power time " + this.powerTime);
                Jump(this.powerTime);
                this.powerTime = 0;
            }
        }

    }

    void Power()
    {
        fragAnim.SetBool("power",true);
    }

    void Jump(double powerTime)
    {
        fragAnim.SetTrigger("jump-up");
        float force = (float)(100 * powerTime);
        heroRigidbody2D.AddForce(Vector2.up * force);
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
