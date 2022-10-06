using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragHore : MonoBehaviour
{
    public SpriteRenderer heroRenderer;
    public Rigidbody2D heroRigidbody2D;

    public BoxCollider2D collider2D;
    public Animator fragAnim;

    [HideInInspector]
    public bool isGround = false;
    [HideInInspector]
    public bool isDrop = false;

    private double powerTime = 0.0f;

    BaseState _state;

    //public FragHore()
    //{
    //    _state = new StandingState(this);
    //    Debug.Log(_state);
    //}
    private void Awake()
    {
        _state = new StandingState(this);
    }

    public void SetHeroineState(BaseState newState)
    {
        _state = newState;
    }

    public void HandleInput()
    {

    }



    public void Update()
    {
        _state.HandleInput();
    }

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
         Debug.DrawRay(new Vector3(heroRigidbody2D.transform.position.x, heroRigidbody2D.transform.position.y - this.collider2D.size.y/2*this.transform.localScale.y, heroRigidbody2D.transform.position.z), Vector2.down * 0.11f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(new Vector3(heroRigidbody2D.transform.position.x, heroRigidbody2D.transform.position.y , heroRigidbody2D.transform.position.z) , Vector2.down, 0.11f + this.collider2D.size.y / 2 * this.transform.localScale.y, 1 << 3);
        if (hit.collider != null)
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }

        this.isDrop = heroRigidbody2D.velocity.y < -0.05;
        Debug.Log(" isGroud : " + isGround + "  isDrop: " + isDrop);
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
