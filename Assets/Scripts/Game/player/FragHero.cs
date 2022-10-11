using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragHero : MonoBehaviour
{
    public SpriteRenderer heroRenderer;
    public Rigidbody2D heroRigidbody2D;

    public BoxCollider2D collider2D;
    public Animator fragAnim;

    [HideInInspector]
    public bool isGround = false;
    [HideInInspector]
    public bool isDrop = false;

    public Game_Direction direction = Game_Direction.Right;

    private bool _isReady = false;
    
    public bool IsReady
    {
        get { return _isReady; }
        set
        {
            _isReady = value;
        }
    }
    

    IBaseState _state;

    //public FragHore()
    //{
    //    _state = new StandingState(this);
    //    Debug.Log(_state);
    //}
    private void Awake()
    {
        _state = new StandingState(this);
        _isReady = true;
    }


    public void SetHeroineState(IBaseState newState)
    {
        _state = newState;
    }

    public void HandleInput()
    {

    }



    public void Update()
    {
        
    }

    private void Start()
    {
        //CollisionListerner.onCollisionEnter2D.AddListener(delegate (GameObject g1, GameObject g2)
        //{
        //    if (g2.tag == "platform")
        //    {

        //    }
        //    Debug.LogFormat("{0}??????¡Á?{1}", g1.name, g2.name);
        //});
        //CollisionListerner.onCollisionStay2D.AddListener(delegate (GameObject g1, GameObject g2)
        //{
        //    Debug.LogFormat("{0}??¡Á???{1}", g1.name, g2.name);
        //    if (g2.tag == "platform")
        //    {

        //    }
        //});
        //CollisionListerner.onCollisionExit2D.AddListener(delegate (GameObject g1, GameObject g2)
        //{
        //    Debug.LogFormat("{0}?¨¢????¡Á?{1}", g1.name, g2.name);
        //    if (g2.tag == "platform")
        //    {

        //    }
        //});
        EventCenter.AddListener<Game_Direction>(Game_Event.FragGameDirection, this.OnFragDirection);

        EventCenter.AddListener<float>(Game_Event.FragGameJump,this.OnFragJump);
        EventCenter.AddListener(Game_Event.FragGameCharge, this.OnFragCharge);
        EventCenter.AddListener(Game_Event.FragGameChargeCancel, this.OnFragChargeCancel);
    }


    private void OnDestroy()
    {
        EventCenter.RemoveListener<Game_Direction>(Game_Event.FragGameDirection, this.OnFragDirection);
        EventCenter.RemoveListener<float>(Game_Event.FragGameDirection, this.OnFragJump);
        EventCenter.RemoveListener(Game_Event.FragGameDirection, this.OnFragCharge);
        EventCenter.RemoveListener(Game_Event.FragGameDirection, this.OnFragChargeCancel);
    }
    
    

    void FixedUpdate()
    {
        // Debug.DrawRay(new Vector3(heroRigidbody2D.transform.position.x, heroRigidbody2D.transform.position.y - this.collider2D.size.y/2*this.transform.localScale.y, heroRigidbody2D.transform.position.z), Vector2.down * 0.11f, Color.red);
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
        //Debug.Log(" isGroud : " + isGround + "  isDrop: " + isDrop);

        _state.HandleInput();
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dir">0 left  1 right</param>
    private void OnFragDirection(Game_Direction dir)
    {
        if (!this._isReady || dir == direction)
        {
            return;
        }
        direction = dir;
        this.heroRenderer.flipX = dir == Game_Direction.Left;
        // this.heroRigidbody2D.transform.localScale = new Vector3((float)dir,1,1);
    }

    private void OnFragCharge()
    {
        if (!this._isReady)
        {
            return;
        }
        SetHeroineState(new ChargeState(this));
    }
    private void OnFragChargeCancel()
    {
        if (!this._isReady)
        {
            return;
        }
        SetHeroineState(new StandingState(this));
    }
    private void OnFragJump(float chargeTime)
    {
        if (!this._isReady)
        {
            return;
        }

        this._isReady = false;
        SetHeroineState(new JumpingState(this, chargeTime));
    }
}
