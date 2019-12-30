using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    public float speed = 5f;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TimeControlArea"))
        {
            speed *= collision.GetComponent<TimeControlArea>().velocityMultiplier;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("TimeControlArea"))
        {
            speed *= collision.GetComponent<TimeControlArea>().reverseMultiplier;
        }
    }
}
