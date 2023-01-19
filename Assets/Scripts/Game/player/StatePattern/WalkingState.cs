using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WalkingState : IBaseState
{
    private FragHero _fragHero;
    public WalkingState(FragHero frag)
    {
        _fragHero = frag;
        // _fragHero.fragAnim.SetBool("walk", true);
        
        _fragHero.fragAnim.speed = 0.95f;
        Debug.Log("------------------------FragHero in WalkingState~!");
    }

    public void Update()
    {

    }

    public void HandleInput()
    {

        // AnimatorClipInfo[] info = _fragHero.fragAnim.GetCurrentAnimatorClipInfo(0);
        // Debug.Log("walk state anim "+ info[0].clip.name);
        if(_fragHero.isDrop)
        {
            Vector2 force = new Vector2((float)_fragHero.direction * 100 , 0);
            _fragHero.heroRigidbody2D.AddForce(force);
            _fragHero.IsReady = false;
            _fragHero.SetHeroineState(new FallingState(_fragHero));
            return;
        }
        if (_fragHero.direction == Game_Direction.None)
        {
            return;
        }

        Vector3 vt = new Vector3((int)_fragHero.direction,0,0);
        _fragHero.heroRigidbody2D.transform.Translate(vt*2*Time.deltaTime);
        
      
        
    }
}
