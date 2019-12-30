using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationBullet : Bullet
{
    public float accelRate;

    private Rigidbody2D m_rigidbody;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        var m_vector = m_rigidbody.velocity;
        m_vector = new Vector2(m_vector.x * accelRate * Time.deltaTime, m_vector.y * accelRate * Time.deltaTime);
        m_rigidbody.velocity = m_rigidbody.velocity + new Vector2(m_vector.x, m_vector.y);
    }
}
