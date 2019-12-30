using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeaccelerationBullet : Bullet
{
    public float accelRate;

    private Rigidbody2D m_rigidbody;
    private Vector2 m_Speedvector;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_Speedvector = m_rigidbody.velocity;
    }

    void FixedUpdate()
    {
        var m_vector = new Vector2(m_Speedvector.x * accelRate * Time.deltaTime, m_Speedvector.y * accelRate * Time.deltaTime);
        m_rigidbody.velocity = m_rigidbody.velocity - new Vector2(m_vector.x, m_vector.y);
    }
}
