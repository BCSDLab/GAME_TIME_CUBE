using System.Collections;

using System.Collections.Generic;
using UnityEngine;

public class SlowStopBullet : Bullet
{
    [SerializeField]
    private float m_accel = 1f;

    private float m_time = 0f;
    [SerializeField]
    private float m_lifeTime = 3f;
    private Rigidbody2D m_rigidbody;
    private Vector2 m_Speedvector;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_time = 0f;
    }

    void Update()
    {
        m_time += Time.deltaTime;

        if (m_lifeTime < m_time)
        {
            PoolManager.instance.PushToPool(gameObject);
        }
    }

    void FixedUpdate()
    {
        m_Speedvector = m_rigidbody.velocity;
        var m_vector = new Vector2(m_Speedvector.x * m_accel * Time.deltaTime, m_Speedvector.y * m_accel * Time.deltaTime);
        m_rigidbody.velocity = m_rigidbody.velocity - new Vector2(m_vector.x, m_vector.y);
    }

    void OnDisable()
    {
        m_time = 0f;
    }
}
