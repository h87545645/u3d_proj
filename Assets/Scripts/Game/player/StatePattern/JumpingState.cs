using UnityEngine;
using System.Collections;

public class JumpingState : IBaseState
{

    private FragHero _fragHore;

  
    
    public JumpingState(FragHero frag, double chargeTime)
    {
        _fragHore = frag;
        _fragHore.fragAnim.SetTrigger("jump-up");
        _fragHore.fragAnim.SetBool("standing", false);
        _fragHore.heroRigidbody2D.constraints = RigidbodyConstraints2D.None;
        float chargeValue = (float)(400 * chargeTime);
        // float yValue = Mathf.Clamp(chargeValue, 100,600);
        // float xValue = Mathf.Clamp(chargeValue, 100, 150);
        chargeValue = Mathf.Clamp(chargeValue, 170,700);
        //float dir = (float)frag.direction * force;
        Vector2 force = new Vector2((float)frag.direction * chargeValue * 0.5f , chargeValue);
        _fragHore.heroRigidbody2D.AddForce(force);
        if (chargeValue < 300)
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
        if(_fragHore.isDrop && !_fragHore.fragAnim.GetBool("jump-down"))
        {
            _fragHore.fragAnim.SetTrigger("jump-down");
        }
        if (_fragHore.isDrop && _fragHore.isGround)
        {
            _fragHore.SetHeroineState(new LandingState(_fragHore));
        }
    }
}