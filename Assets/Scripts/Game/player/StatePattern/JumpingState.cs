using System;
using UnityEngine;
using System.Collections;

public class JumpingState : IBaseState
{

    private FragHero _fragHore;

    private float _jumpDelay = 0; 
    
    public JumpingState(FragHero frag, double chargeTime)
    {
        _jumpDelay = 0;
        _fragHore = frag;
        _fragHore.fragAnim.SetTrigger("jump-up");
        _fragHore.fragAnim.SetBool("standing", false);
        _fragHore.heroRigidbody2D.constraints = RigidbodyConstraints2D.None;
        _fragHore.heroRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        float chargeValueY =  (float)(_fragHore.jumpVaryY * Math.Max(chargeTime - 0.2 , 0) + _fragHore.jumpStaticY); 
        float chargeValueX = (float)(_fragHore.jumpVaryX * chargeTime)+_fragHore.jumpStaticX;
        float yValue = Mathf.Clamp(chargeValueY, 500,1900);
        float xValue = Mathf.Clamp(chargeValueX, 500, 900);
        // chargeValue = Mathf.Clamp(chargeValue, 500,1800);
        //float dir = (float)frag.direction * force;
        Vector2 force = new Vector2((float)frag.lastDirection * xValue , yValue);
        _fragHore.heroRigidbody2D.AddForce(force);
        if (chargeValueY < 700)
        {
            _fragHore.fragAnim.speed = 1.9f;
        }
        else
        {
            _fragHore.fragAnim.speed = 1.3f;
        }
        //_fragHore.heroRigidbody2D.AddForce(frag.direction == Game_Direction.Left ? Vector2.left * chargeVaule :  Vector2.right * chargeVaule);
        //_fragHore.heroRigidbody2D.velocity = force;
        Debug.Log("------------------------Heroine in JumpingState~!(½øÈëÌøÔ¾×´Ì¬£¡)");
    }

    public void Update()
    {

    }

    public void HandleInput()
    {
        _jumpDelay += Time.deltaTime;
        if (_jumpDelay > 0.1)
        {
            if(_fragHore.isDrop)
            {
                // Debug.Log("jump-down bool ");
            
                _fragHore.SetHeroineState(new FallingState(_fragHore));
            }else if (_fragHore.isGround)
            {
                _fragHore.SetHeroineState(new FallingState(_fragHore));
            }
        }
       
  
        // AnimatorClipInfo[] info = _fragHore.fragAnim.GetCurrentAnimatorClipInfo(0);
        // Debug.Log("walk state anim "+ info[0].clip.name);
    }
}