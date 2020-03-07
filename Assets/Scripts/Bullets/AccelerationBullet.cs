using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationBullet : Bullet
{
    [SerializeField]
    private float m_accelRate = 0.5f;
    [SerializeField]
    private bool m_useMaxSpeed = true;
    [SerializeField]
    private float m_maxSpeed = 10;
    [SerializeField]
    private bool m_useMinSpeed = true;
    [SerializeField]
    private float m_minSpeed = 1f;

    private Rigidbody2D m_rigidbody;
    private float m_accelData = 0;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (m_useMaxSpeed && m_rigidbody.velocity.magnitude >= m_maxSpeed) return;
        if (m_useMinSpeed && m_rigidbody.velocity.magnitude <= m_minSpeed) return;
        Vector2 newVelocity = m_rigidbody.velocity * (m_accelRate * Time.deltaTime);
        m_rigidbody.velocity += newVelocity;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TimeControlArea"))
        {
            m_accelData = m_accelRate;
            m_accelRate = 0;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("TimeControlArea"))
        {
            m_accelRate = m_accelData;
        }
    }
}
