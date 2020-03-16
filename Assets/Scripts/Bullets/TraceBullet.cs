using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 지나간 자리에 탄을 생성하는 탄
public class TraceBullet : Bullet
{
    [SerializeField]
    private float m_traceTime = 1f;
    [SerializeField]
    private float m_traceLifeTime = 3f;
    [SerializeField]
    private GameObject m_traceBullet = null;

    private Rigidbody2D m_rigidbody;

    private float m_time = 0f;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        m_time = 0f;
    }

    void FixedUpdate()
    {
        m_time += Time.deltaTime;

        if (m_traceTime < m_time)
        {
            LeaveTrace();
        }
    }

    void LeaveTrace()
    {
        m_time = 0f;

        GameObject bulletInst = PoolManager.instance.PopFromPool(m_traceBullet.name);
        bulletInst.transform.position = transform.position;
        Destroy(bulletInst, m_traceLifeTime);
        bulletInst.SetActive(true);
    }
}
