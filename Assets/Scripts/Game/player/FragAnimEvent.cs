using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragAnimEvent : MonoBehaviour
{
    public FragHero fragHero;
    public void OnLandingFinish()
    {
        fragHero.fragAnim.SetBool("standing", true);
        fragHero.SetHeroineState(new StandingState(fragHero));
        StartCoroutine(UnityUtils.DelayFuc(() =>
        {
            Debug.Log("frag ready");
            fragHero.IsReady = true;
        },0.15f));
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
            Debug.Log("wall");
            Game_Direction dir = fragHero.direction == Game_Direction.Left ? Game_Direction.Right : Game_Direction.Left;
            fragHero.OnFragDirection(dir , true);
            // heroRigidbody2D.velocity.x = -heroRigidbody2D.velocity.x;
        }
    }
}
