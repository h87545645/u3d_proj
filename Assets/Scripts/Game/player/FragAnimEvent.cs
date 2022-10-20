using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragAnimEvent : MonoBehaviour
{
    public FragHero fragHero;
    public void OnLandingFinish()
    {
        fragHero.direction = Game_Direction.None;
        StartCoroutine(UnityUtils.DelayFuc(() =>
        {
            Debug.Log("frag ready");
            fragHero.IsReady = true;
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                Game_Direction direction = Input.GetKey(KeyCode.LeftArrow) ? Game_Direction.Left : Game_Direction.Right;
                // fragHero.SetHeroineState(new WalkingState(fragHero));
                // fragHero.fragAnim.SetBool("walk", true);
                EventCenter.PostEvent<Game_Direction,bool>(Game_Event.FragGameDirection, direction,false);
            }
            else
            {
                fragHero.fragAnim.SetBool("standing", true);
                // fragHero.fragAnim.SetBool("jump-down",true);
                fragHero.SetHeroineState(new StandingState(fragHero));
            }
        },0.05f));
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // foreach (ContactPoint2D contact in collision.contacts)
        // {
        //     Debug.DrawLine(contact.point, transform.position, Color.red);
        //     var direction = transform.InverseTransformPoint(contact.point);
        //     if (direction.x > 0f)
        //     {
        //         print("right collision");
        //     }
        //     if (direction.x < 0f)
        //     {
        //         print("left collision");
        //     }
        //     if (direction.y > 0f)
        //     {
        //         print("up collision");
        //     }
        //     if (direction.y < 0f)
        //     {
        //         print("down collision");
        //     }
        // }
        if (collision.collider.tag == "wall")
        {
            if (fragHero.GetState().GetType() == typeof(WalkingState))
            {
                return;
            }
            Debug.Log("wall");
            Game_Direction dir = fragHero.direction == Game_Direction.Left ? Game_Direction.Right : Game_Direction.Left;
            fragHero.OnFragDirection(dir , true);
            // heroRigidbody2D.velocity.x = -heroRigidbody2D.velocity.x;
        }
    }
}
