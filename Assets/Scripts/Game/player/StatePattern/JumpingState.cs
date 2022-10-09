using UnityEngine;
using System.Collections;

public class JumpingState : BaseState
{

    private FragHore _fragHore;


    
    public JumpingState(FragHore frag, double chargeTime)
    {
        _fragHore = frag;
        _fragHore.fragAnim.SetTrigger("jump-up");
        _fragHore.fragAnim.SetBool("standing", false);
        float chargeVaule = (float)(300 * chargeTime);
        chargeVaule = Mathf.Clamp(chargeVaule, 100,1000);
        //float dir = (float)frag.direction * force;
        Vector2 force = new Vector2((float)frag.direction * chargeVaule, chargeVaule);
        _fragHore.heroRigidbody2D.AddForce(Vector2.up * chargeVaule);
        _fragHore.heroRigidbody2D.AddForce(frag.direction == Game_Direction.Left ? Vector2.left * chargeVaule :  Vector2.right * chargeVaule);
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