﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackingBullet_NWay : Bullet
{
    private BulletPattern m_bulletPattern;
    private float m_time = 0f;
    [SerializeField]
    private float m_endTime = 0f;

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

        if (m_endTime < m_time)
        {
            StartCoroutine("BulletDestroy");
        }
    }

    IEnumerator BulletDestroy()
    {
        m_bulletPattern.StartPattern();
        yield return new WaitForSeconds(0.01f);
        PoolManager.instance.PushToPool(this.gameObject);
    }
}
