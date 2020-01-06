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

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Debug.Log(m_rigidbody.velocity.magnitude);
        if (m_useMaxSpeed && m_rigidbody.velocity.magnitude >= m_maxSpeed) return;
        if (m_useMinSpeed && m_rigidbody.velocity.magnitude <= m_minSpeed) return;
        Vector2 newVelocity = m_rigidbody.velocity * (m_accelRate * Time.deltaTime);
        m_rigidbody.velocity += newVelocity;
    }
}
