using UnityEngine;
using System.Collections;

public class StandingState : IBaseState
{
    private FragHero _fragHore;
    public StandingState(FragHero frag)
    {
        _fragHore = frag;
        _fragHore.fragAnim.SetBool("standing", true);
        _fragHore.fragAnim.speed = 1f;
        Debug.Log("------------------------FragHore in StandingState~!£¨½øÈëÕ¾Á¢×´Ì¬£¡£©");
    }

    public void Update()
    {

    }

    public void HandleInput()
    {
        if (_fragHore.isGround)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _fragHore.SetHeroineState(new ChargeState(_fragHore));
            }
        }
    }
}
