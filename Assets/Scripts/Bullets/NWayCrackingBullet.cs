using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BulletPattern))]
public class NWayCrackingBullet : Bullet
{
    [SerializeField]
    private float m_crackTime = 3f;

    private BulletPattern m_bulletPattern;
    private float m_time = 0f;
    private bool m_isCracking = false;

    void Awake()
    {
        m_bulletPattern = GetComponent<BulletPattern>();
    }

    void OnEnable()
    {
        m_time = 0f;
        m_isCracking = false;
    }

    void FixedUpdate()
    {
        if (m_isCracking) return;

        m_time += Time.deltaTime;

        if (m_crackTime < m_time)
        {
            m_isCracking = true;
            StartCoroutine("BulletDestroy");
        }
    }

    IEnumerator BulletDestroy()
    {
        m_bulletPattern.StartPattern();
        yield return new WaitForSeconds(0.01f);
        m_bulletPattern.StopPattern();
        PoolManager.instance.PushToPool(gameObject);
    }
}
