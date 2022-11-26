using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragAnimEvent : MonoBehaviour
{
    public FragHero fragHero;

    private bool _hasTurn = false;
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
                Debug.Log("landing over to walk dir : " +direction);
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
        bool isUp = false;
        bool isRight = false;
        bool isLeft = false;
        bool isDown = false;
        if (Math.Round(collision.contacts[0].normal.y) == -1) {
            Debug.Log("++++++++++++++���Ϸ���ײ");
            isUp = true;
        } else if (Math.Round(collision.contacts[0].normal.y) == 1)
        {
            isDown = true;
            Debug.Log("++++++++++++++���·���ײ");
        }else if (Math.Round(collision.contacts[0].normal.x) == 1)
        {
            isLeft = true;
            Debug.Log("++++++++++++++�������ײ");
        }else if (Math.Round(collision.contacts[0].normal.x) == -1)
        {
            isRight = true;
            Debug.Log("++++++++++++++���ұ���ײ");
        }
    
        // foreach (ContactPoint2D contact in collision.contacts)
        // {
        //     // Debug.DrawLine(contact.point, transform.position, Color.red);
        //     var direction = transform.InverseTransformPoint(contact.point);
        //     // if (direction.x > 0f)
        //     // {
        //     //     print("right collision");
        //     // }
        //     // if (direction.x < 0f)
        //     // {
        //     //     print("left collision");
        //     // }
        //     if (contact.normal.y == -1)
        //     {
        //         print("up collision");
        //         if (collision.collider.tag == "wall")
        //         {
        //             isUp = true;
        //         }
        //     }
        //     if (direction.y < 0f)
        //     {
        //         print("down collision");
        //         if (collision.collider.tag == "platform")
        //         {
        //             isDown = true;
        //         }
        //     }
        // }

        if (!_hasTurn)
        {
            _hasTurn = true;
            if (!isDown)
            {
                if (isUp)
                {
                    // var direction = transform.InverseTransformPoint(collision.contacts[0].point);
                    int flag = fragHero.heroRenderer.flipX  ? -1 : 1;
                    fragHero.heroRigidbody2D.velocity = new Vector2(fragHero.chargeTime * 5 * flag,fragHero.heroRigidbody2D.velocity.y);
                }
                else
                {
                    Game_Direction dir = isLeft  ? Game_Direction.Right : Game_Direction.Left;
                    if (fragHero.direction == Game_Direction.None)
                    {
                        dir = fragHero.heroRenderer.flipX ? Game_Direction.Right : Game_Direction.Left;
                    }
                    fragHero.OnFragDirection(dir , true);
                    fragHero.heroRigidbody2D.velocity = new Vector2(fragHero.chargeTime * 5 * (int)dir,fragHero.heroRigidbody2D.velocity.y);
                }
            }
        }

        
        
        if (collision.collider.tag == "final")
        {
            EventCenter.PostEvent(Game_Event.FragGameFinish);
        } 
 
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        _hasTurn = false;
    }
}
