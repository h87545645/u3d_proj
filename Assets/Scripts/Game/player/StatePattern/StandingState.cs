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
        EventCenter.PostEvent(Game_Event.FragStanding);
        Debug.Log("------------------------FragHore in StandingState~");
    }

    public void Update()
    {

    }

    public void HandleInput()
    {
        // if (_fragHore.IsReady)
        // {
            // if (Input.GetKeyDown(KeyCode.Space))
            // {
            //     _fragHore.SetHeroineState(new ChargeState(_fragHore));
            // }

            // if (Input.GetKeyDown(KeyCode.LeftArrow))
            // {
            //     EventCenter.PostEvent<Game_Direction,bool>(Game_Event.FragGameDirection, Game_Direction.Left,false);
            //     Debug.Log("left down");
            // }
            // if (Input.GetKeyDown(KeyCode.RightArrow))
            // {
            //     EventCenter.PostEvent<Game_Direction,bool>(Game_Event.FragGameDirection, Game_Direction.Right,false);
            //     Debug.Log("right down");
            // }
        // }
        
        // AnimatorClipInfo[] info = _fragHore.fragAnim.GetCurrentAnimatorClipInfo(0);
        // Debug.Log("walk state anim "+ info[0].clip.name);
    }
}
