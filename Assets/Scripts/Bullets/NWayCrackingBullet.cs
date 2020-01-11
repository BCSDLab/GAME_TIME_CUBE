using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DirectionalAimedNWay))]
public class NWayCrackingBullet : Bullet
{
    private BulletPattern m_bulletPattern;
    private float m_time = 0f;
    [SerializeField]
    private float m_crackTime = 3f;

    void Awake()
    {
        m_bulletPattern = GetComponent<DirectionalAimedNWay>();
    }

    void OnEnable()
    {
        m_time = 0f;
    }

    void Update()
    {
        m_time += Time.deltaTime;

        if (m_crackTime < m_time)
        {
            StartCoroutine("BulletDestroy");
        }
    }

    IEnumerator BulletDestroy()
    {
        m_bulletPattern.StartPattern();
        yield return new WaitForSeconds(0.01f);
        PoolManager.instance.PushToPool(gameObject);
    }
}
