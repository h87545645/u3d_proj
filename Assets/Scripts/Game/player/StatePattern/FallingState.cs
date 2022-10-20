using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingState : IBaseState
{
    private FragHero _fragHore;
    
    public FallingState(FragHero frag)
    {
        _fragHore = frag;
        _fragHore.fragAnim.SetBool("jump-down",true);
        Debug.Log("------------------------FragHore in FallingState~!");
    }

    public void Update()
    {
       
    }

    public void HandleInput()
    {
        if (/*_fragHore.isDrop &&*/ _fragHore.isGround)
        {
            Debug.Log("------------------------falling to landing!!!!");
            _fragHore.fragAnim.SetBool("jump-down",false);
            _fragHore.SetHeroineState(new LandingState(_fragHore));
        }
        
        // AnimatorClipInfo[] info = _fragHore.fragAnim.GetCurrentAnimatorClipInfo(0);
        // Debug.Log("walk state anim "+ info[0].clip.name);
    }
}
