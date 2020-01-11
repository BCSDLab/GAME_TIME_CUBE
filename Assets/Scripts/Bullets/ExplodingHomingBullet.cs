using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingHomingBullet : Bullet
{
    public GameObject target;
    public float speed = 5f;
    [SerializeField]
    private float m_explodeDistance = 5f;
    [SerializeField]
    private float m_alpha = 200f;
    //[SerializeField]
    //private int m_hp = 70;

    private BulletPattern m_bulletPattern;
    private Rigidbody2D m_rigidbody;

    void Awake()
    {
        m_bulletPattern = GetComponent<RadialMulti>();
    }

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

        if (Vector2.Distance(transform.position, target.transform.position) < m_explodeDistance)
        {
            StartCoroutine("BulletDestroy");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TimeControlArea"))
        {
            speed *= collision.GetComponent<TimeControlArea>().velocityMultiplier;
        }
        //else if (collision.CompareTag("PlayerBullet"))
        //{
        //    m_hp -= collision.GetComponent<PlayerBullet>().damage;
        //    PoolManager.instance.PushToPool(collision.gameObject);

        //    if (m_hp <= 0)
        //    {
        //        PoolManager.instance.PushToPool(gameObject);
        //    }
        //}
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("TimeControlArea"))
        {
            speed *= collision.GetComponent<TimeControlArea>().reverseMultiplier;
        }
    }

    IEnumerator BulletDestroy()
    {
        m_bulletPattern.StartPattern();
        yield return new WaitForSeconds(0.01f);
        PoolManager.instance.PushToPool(this.gameObject);
    }
}
