using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FragHero : MonoBehaviour
{
    public SpriteRenderer heroRenderer;
    public Rigidbody2D heroRigidbody2D;

    public BoxCollider2D collider2D;
    public Animator fragAnim;

    // public Tilemap tilemap;

    [HideInInspector]
    public bool isGround = false;
    [HideInInspector]
    public bool isDrop = false;

    public Game_Direction direction = Game_Direction.None;
    
    /// <summary>
    /// 缓存0.1秒前的方向状态
    /// </summary>
    public Game_Direction lastDirection = Game_Direction.None;

    private Vector2 _lastPosition = Vector2.zero;
    public Vector2 LastPosition
    {
        get { return _lastPosition; }
        set
        {
            _lastPosition = value;
        }
    }
    
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
        Debug.DrawRay(new Vector3(heroRigidbody2D.transform.position.x + this.collider2D.size.x * 0.49f * this.heroRigidbody2D.transform.localScale.x, heroRigidbody2D.transform.position.y - 
            this.collider2D.size.y/2*this.heroRigidbody2D.transform.localScale.y, heroRigidbody2D.transform.position.z), Vector2.down * 0.11f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(new Vector3(heroRigidbody2D.transform.position.x + this.collider2D.size.x * 0.5f * this.heroRigidbody2D.transform.localScale.x, heroRigidbody2D.transform.position.y , heroRigidbody2D.transform.position.z) ,
            Vector2.down, 0.11f + this.collider2D.size.y / 2 * this.heroRigidbody2D.transform.localScale.y, 1 << 3);
        RaycastHit2D hit2 = Physics2D.Raycast(new Vector3(heroRigidbody2D.transform.position.x - this.collider2D.size.x * 0.5f * this.heroRigidbody2D.transform.localScale.x, heroRigidbody2D.transform.position.y , heroRigidbody2D.transform.position.z) ,
            Vector2.down, 0.11f + this.collider2D.size.y / 2 * this.heroRigidbody2D.transform.localScale.y, 1 << 3);
        if (hit.collider != null  || hit2.collider != null)
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }

        this.isDrop = heroRigidbody2D.velocity.y < -0.05;
        // Debug.Log("velocity : " + heroRigidbody2D.velocity.y);
        // Debug.Log(" isGroud : " + isGround + "  isDrop: " + isDrop + " heroRigidbody2D.velocity: " +heroRigidbody2D.velocity);

        _state.HandleInput();
        // AnimatorClipInfo[] info = fragAnim.GetCurrentAnimatorClipInfo(0);
        // Debug.Log("walk state anim "+ info[0].clip.name);
    }

    private void Start()
    {
        //CollisionListerner.onCollisionEnter2D.AddListener(delegate (GameObject g1, GameObject g2)
        //{
        //    if (g2.tag == "platform")
        //    {

        //    }
        //    Debug.LogFormat("{0}??????×?{1}", g1.name, g2.name);
        //});
        //CollisionListerner.onCollisionStay2D.AddListener(delegate (GameObject g1, GameObject g2)
        //{
        //    Debug.LogFormat("{0}??×???{1}", g1.name, g2.name);
        //    if (g2.tag == "platform")
        //    {

        //    }
        //});
        //CollisionListerner.onCollisionExit2D.AddListener(delegate (GameObject g1, GameObject g2)
        //{
        //    Debug.LogFormat("{0}?á????×?{1}", g1.name, g2.name);
        //    if (g2.tag == "platform")
        //    {

        //    }
        //});
        EventCenter.AddListener<Game_Direction,bool>(Game_Event.FragGameDirection, this.OnFragDirection);

        EventCenter.AddListener<float>(Game_Event.FragGameJump,this.OnFragJump);
        EventCenter.AddListener(Game_Event.FragGameCharge, this.OnFragCharge);
        EventCenter.AddListener(Game_Event.FragGameChargeCancel, this.OnFragChargeCancel);
    }


    private void OnDestroy()
    {
        EventCenter.RemoveListener<Game_Direction,bool>(Game_Event.FragGameDirection, this.OnFragDirection);
        EventCenter.RemoveListener<float>(Game_Event.FragGameDirection, this.OnFragJump);
        EventCenter.RemoveListener(Game_Event.FragGameDirection, this.OnFragCharge);
        EventCenter.RemoveListener(Game_Event.FragGameDirection, this.OnFragChargeCancel);
    }
    
    

    void FixedUpdate()
    {
        
        
       
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dir"></param>
    public void OnFragDirection(Game_Direction dir , bool force = false)
    {
        if (!force && (!this._isReady || dir == direction))
        {
            return;
        }
        
        switch (dir)
        {
            case Game_Direction.None:
                
                break;
            case Game_Direction.Right:
                this.heroRenderer.flipX = false;
                break;
            case Game_Direction.Left:
                this.heroRenderer.flipX = true;
                break;
        }

        if (force)
        {
            direction = dir;
        }
        else
        {
            if (dir == Game_Direction.None)
            {
                StartCoroutine(UnityUtils.DelayFuc(() =>
                {
                    lastDirection = direction;
                    // Debug.Log("dir is up!!!!!!!");
                },0.1f));
                direction = dir;
                if ( isGround && _state.GetType() != typeof(ChargeState))
                {
                    // Debug.Log("dir is up!!!!!!!");
                    SetHeroineState(new StandingState(this));
                    fragAnim.SetBool("walk", false);
                }
            }
            else
            {
                direction = dir;
                lastDirection = direction;
                if ( isGround && _state.GetType() != typeof(ChargeState))
                {
                    
                    
                    fragAnim.SetBool("standing", false);
                    fragAnim.SetBool("walk", true);
                    SetHeroineState(new WalkingState(this));
                }
            }
        }
 
       
      
        
        // this.heroRigidbody2D.transform.localScale = new Vector3((float)dir,1,1);

        // if (!force && isGround && _state.GetType() != typeof(ChargeState))
        // {
        //     if (dir != Game_Direction.None )
        //     {
        //             fragAnim.SetBool("standing", false);
        //             fragAnim.SetBool("walk", true);
        //             SetHeroineState(new WalkingState(this));
        //     }
        //     else
        //     {
        //         if (_state.GetType() != typeof(WalkingState))
        //         {
        //             Debug.Log("walk to standing state!!!!!!!");
        //             SetHeroineState(new StandingState(this));
        //             fragAnim.SetBool("walk", false);
        //         }
        //         
        //     }
        // }
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
        if (!this._isReady )
        {
            return;
        }
        SetHeroineState(new StandingState(this));
    }
    private void OnFragJump(float chargeTime)
    {
        if (!this._isReady || _state.GetType() != typeof(ChargeState))
        {
            return;
        }

        this._isReady = false;
        fragAnim.SetBool("walk", false);
        SetHeroineState(new JumpingState(this, chargeTime));
    }

    public IBaseState GetState()
    {
        return this._state;
    }
}
