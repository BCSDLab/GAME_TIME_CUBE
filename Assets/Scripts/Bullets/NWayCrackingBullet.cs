using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BulletPattern))]
public class NWayCrackingBullet : Bullet
{
    [SerializeField]
    private float m_crackTime = 3f;
    [SerializeField]
    private GameObject m_redMask = null;

    private BulletPattern m_bulletPattern;
    private float m_time = 0f;
    private float m_blinkTime;
    private bool m_isBlinking = false;
    private bool m_isCracking = false;

    void Awake()
    {
        m_bulletPattern = GetComponent<BulletPattern>();
        m_blinkTime = m_crackTime - 0.5f;
    }

    void OnEnable()
    {
        m_time = 0f;
        m_isBlinking = false;
        m_isCracking = false;
    }

    void FixedUpdate()
    {
        if (m_isCracking) return;

        m_time += Time.deltaTime;

        if (!m_isBlinking && m_blinkTime < m_time)
        {
            m_isBlinking = true;
            StartCoroutine("RedBlink");
        }
        else
        {
            m_redMask.SetActive(false);
        }

        if (!m_isCracking && m_crackTime < m_time)
        {
            m_isCracking = true;
            StartCoroutine("BulletDestroy");
        }
    }

    IEnumerator RedBlink()
    {
        while (true)
        {
            m_redMask.SetActive(true);
            yield return new WaitForSeconds(0.05f);
            m_redMask.SetActive(false);
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator BulletDestroy()
    {
        StopCoroutine("RedBlink");
        m_bulletPattern.StartPattern();
        yield return new WaitForSeconds(0.01f);
        m_redMask.SetActive(false);
        m_bulletPattern.StopPattern();
        PoolManager.instance.PushToPool(gameObject);
    }
}
