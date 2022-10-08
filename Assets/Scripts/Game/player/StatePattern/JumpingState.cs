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
        float force = (float)(300 * chargeTime);
        _fragHore.heroRigidbody2D.AddForce(Vector2.up * force);
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