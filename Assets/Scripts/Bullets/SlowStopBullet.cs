using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowStopBullet : Bullet
{
    public float accelRate;

    private float m_time = 0f;
    [SerializeField]
    private float m_endTime = 0f;
    private Rigidbody2D m_rigidbody;
    private Vector2 m_Speedvector;

    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        m_time += Time.deltaTime;

        if (m_endTime < m_time)
        {
            PoolManager.instance.PushToPool(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        m_Speedvector = m_rigidbody.velocity;
        var m_vector = new Vector2(m_Speedvector.x * accelRate * Time.deltaTime, m_Speedvector.y * accelRate * Time.deltaTime);
        m_rigidbody.velocity = m_rigidbody.velocity - new Vector2(m_vector.x, m_vector.y);
    }

    void OnDisable()
    {
        m_time = 0f;
    }
}
