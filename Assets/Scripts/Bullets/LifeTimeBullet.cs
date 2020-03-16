using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTimeBullet : Bullet
{
    [SerializeField]
    private float m_lifeTime = 5f;

    private float m_time = 0f;

    void OnEnable()
    {
        m_time = 0f;
        transform.localScale = Vector3.one;
    }

    void FixedUpdate()
    {
        m_time += Time.deltaTime;

        if (m_lifeTime < m_time)
        {
            Blow();
            PoolManager.instance.PushToPool(gameObject);
        }

        float scaleMultiplier = Mathf.Max(1f - m_time / m_lifeTime, 0.3f);
        transform.localScale = Vector3.one * scaleMultiplier;
    }
}
