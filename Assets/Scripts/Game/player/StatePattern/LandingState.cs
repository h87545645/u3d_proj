using UnityEngine;
using System.Collections;

public class LandingState : IBaseState
{
    private FragHero _fragHore;
    public LandingState(FragHero frag)
    {
        _fragHore = frag;
        _fragHore.fragAnim.SetBool("landing", true);
        _fragHore.heroRigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX;
        Debug.Log("------------------------FragHore in LandingState~!");
    }

    public void Update()
    {

    }

    public void HandleInput()
    {

    }
}
