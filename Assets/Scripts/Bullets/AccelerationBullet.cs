using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationBullet : Bullet
{
    [SerializeField]
    private float m_accelRate = 1.0f;

    private Rigidbody2D m_rigidbody;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 newVelocity = m_rigidbody.velocity * (m_accelRate * Time.deltaTime);
        m_rigidbody.velocity += newVelocity;
    }
}
