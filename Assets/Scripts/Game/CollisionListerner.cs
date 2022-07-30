using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEvent : UnityEvent<GameObject, GameObject> { }

public class CollisionListerner : MonoBehaviour
{
    public static CollisionEvent onCollisionEnter2D = new CollisionEvent();
    public static CollisionEvent onCollisionStay2D = new CollisionEvent();
    public static CollisionEvent onCollisionExit2D = new CollisionEvent();

    //Å×³öÊÂ¼þ
    private void OnCollisionEnter2D(Collision2D collision)
    {
        onCollisionEnter2D.Invoke(gameObject, collision.collider.gameObject);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        onCollisionStay2D.Invoke(gameObject, collision.collider.gameObject);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        onCollisionExit2D.Invoke(gameObject, collision.collider.gameObject);
    }
}
