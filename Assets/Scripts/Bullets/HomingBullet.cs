using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 적탄
public class HomingBullet : Bullet
{
    public GameObject target;
    public float speed = 5f;
    [SerializeField]
    private float m_alpha = 200f;
    [SerializeField]
    private int m_hp = 70;

    private Rigidbody2D m_rigidbody;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector2 direction = transform.position - target.transform.position;
        direction.Normalize();
        float crossZ = Vector3.Cross(direction, transform.right).z;

        m_rigidbody.velocity = -transform.right * speed;
        m_rigidbody.angularVelocity = -m_alpha * crossZ;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet"))
        {
            m_hp -= collision.GetComponent<PlayerBullet>().damage;
            PoolManager.instance.PushToPool(collision.gameObject);

            if (m_hp <= 0)
            {
                PoolManager.instance.PushToPool(gameObject);
            }
        }
        else if (collision.CompareTag("TimeControlArea"))
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
